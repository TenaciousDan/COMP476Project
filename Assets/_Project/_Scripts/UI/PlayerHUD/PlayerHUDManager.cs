using Game.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tenacious;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHUDManager : MonoBehaviourPunCallbacks
{
    public List<PlayerHUD> huds = new List<PlayerHUD>();
    public int numToUse = 0;
    public Transform playerHudParent;
    [SerializeField] private GameObject hudPrefab;

    private static PlayerHUDManager instance;
    private static bool reinitializationPermitted = false;
    private static bool destroyed = false;
    private static object mutex = new object();

    [Tooltip("Allow this object to be re-initialized after being destroyed.")]
    [SerializeField] private bool allowReinitialization = false;
    [Tooltip("Destroy this object when a new scene is loaded.")]
    [SerializeField] private bool destroyOnLoad = false;

    public static PlayerHUDManager Instance
    {
        get
        {
            if (destroyed)
            {
                string log_message = "NetworkManager has already been destroyed";

                if (reinitializationPermitted)
                {
                    Debug.LogWarning(log_message + " - Creating new instance.");
                    destroyed = false;
                }
                else
                {
                    Debug.LogWarning(log_message + " - Returning null.");
                    return null;
                }
            }

            return CreateInstance(null);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            if (destroyed && !allowReinitialization)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                CreateInstance(this.gameObject);
            }
        }
        else
        {
            // an instance already exists and this is a duplicate
            Destroy(this.gameObject);
            destroyed = false; // the "true" instance has not yet been destroyed
            return;
        }

        reinitializationPermitted = allowReinitialization;

        if (!DestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (numToUse > 0)
        {
            for (int i = 0; i < numToUse; i++)
            {
                photonView.RPC("ActivateHud", RpcTarget.All, i);
            }
        }
    }

    [PunRPC]
    private void ActivateHud(int index)
    {
        huds[index].gameObject.SetActive(huds[index].player.Phase == AbstractPlayer.EPlayerPhase.Main);
    }

    public void InitializeUI(HumanPlayer player)
    {
        var hudObj = PhotonNetwork.Instantiate("PlayerHUD", Vector3.zero, Quaternion.identity);
        hudObj.transform.parent = playerHudParent;
        hudObj.transform.position = Vector3.zero;
        var rectTransform = hudObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.hud = hudObj.GetComponent<PlayerHUD>();
        huds.Add(player.hud);
        player.hud.player = player;
    }

    #region SINGLETON
    private static PlayerHUDManager CreateInstance(GameObject game_object)
    {
        lock (mutex)
        {
            // Create new instance if one doesn't already exist
            if (instance == null)
            {
                if (game_object == null)
                {
                    // create the GameObject that will house the singleton instance
                    game_object = new GameObject("PlayerHUDManager");
                    game_object.AddComponent<PlayerHUDManager>();
                }

                instance = game_object.GetComponent<PlayerHUDManager>();
            }

            return instance;
        }
    }
    public bool DestroyOnLoad
    {
        get { return destroyOnLoad; }
        set
        {
            destroyOnLoad = value;
            if (destroyOnLoad)
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
            else
                DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        // When Unity quits it destroys objects in a random order and this can create issues for singletons. 
        // So we prevent reinitialization and access to the instance when the application quits to prevent problems.
        reinitializationPermitted = false;
        destroyed = true; // pretend its already destroyed
    }

    private void OnDestroy()
    {
        instance = null;
        destroyed = true;
    }
    #endregion
}
