using System;
using System.Collections.Generic;
using Game.AI;
using Photon.Pun;
using Photon.Realtime;
using Tenacious.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public const int MaxRoomSize = 4;
    public int humanPlayerCount = 0;
    public int aiPlayerCount = 0;
    
    public HumanPlayer[] humanPlayers;
    public List<AIPlayer> aiPlayers = new List<AIPlayer>();

    private static NetworkManager instance;
    private static bool reinitializationPermitted = false;
    private static bool destroyed = false;
    private static object mutex = new object();

    [Tooltip("Allow this object to be re-initialized after being destroyed.")]
    [SerializeField] private bool allowReinitialization = false;
    [Tooltip("Destroy this object when a new scene is loaded.")]
    [SerializeField] private bool destroyOnLoad = false;

    [SerializeField]
    private NetworkPrefabPool networkPrefabPool;
    
    // Instance
    public static NetworkManager Instance
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
        networkPrefabPool = GetComponent<NetworkPrefabPool>();

        if(instance == null)
        {
            if (destroyed && !allowReinitialization)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                CreateInstance(this.gameObject); 
                PhotonNetwork.PrefabPool = networkPrefabPool;
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

    public override void OnConnectedToMaster()
    {
        print("Connected to master server!");
    }

    #region ROOMS
    public void CreateRoom(string roomName, int aiPlayerCount)
    {
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.MaxPlayers = Convert.ToByte(MaxRoomSize - aiPlayerCount);
        
        PhotonNetwork.CreateRoom(roomName, newRoomOptions);
    }

    public override void OnCreatedRoom()
    {
        print($"Created room: {PhotonNetwork.CurrentRoom.Name}");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
    }
    #endregion

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

    #region SINGLETON
    private static NetworkManager CreateInstance(GameObject game_object)
    {
        lock (mutex)
        {
            // Create new instance if one doesn't already exist
            if (instance == null)
            {
                if (game_object == null)
                {
                    // create the GameObject that will house the singleton instance
                    game_object = new GameObject("NetworkManager");
                    game_object.AddComponent<NetworkManager>();
                }

                instance = game_object.GetComponent<NetworkManager>();
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

    #region GAMEPLAY

    public void LoadedIntoGame()
    {
        photonView.RPC("UpdateHumanPlayerCount", RpcTarget.AllBuffered);
    }
    
    [PunRPC]
    public void UpdateHumanPlayerCount()
    {
        humanPlayerCount++;

        // After all players are loaded in, set their networking parameters
        if (humanPlayerCount == PhotonNetwork.PlayerList.Length)
        {
            SpawnHumanPlayer();

            // Host will handle all AI Spawning
            if (PhotonNetwork.IsMasterClient)
            {
                for (var i = 0; i < aiPlayerCount; i++)
                {
                    SpawnAIPlayer();
                }
            }
            
        }
    }

    public void SpawnHumanPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate("HumanPlayer", new Vector3(0,0,0), Quaternion.identity);
        playerObj.SetActive(true);

        HumanPlayer playerScript = playerObj.GetComponent<HumanPlayer>();
        //PlayerHUDManager.Instance.InitializeUI(playerScript);

        playerScript.photonView.RPC("InitializePlayerOnNetwork", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public void SpawnAIPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate("AIPlayer", new Vector3(0, 0, 0), Quaternion.identity);
        playerObj.SetActive(true);

        AIPlayer playerScript = playerObj.GetComponent<AIPlayer>();

        aiPlayers.Add(playerScript);
    }
    
    public void InitializeHumanPlayer(HumanPlayer player, float _maxActionPoints, Vector3 _offset, string _startNodeId, string _name)
    {
        player.photonView.RPC("InitializePlayer", RpcTarget.All, _maxActionPoints, _offset, _startNodeId, _name);
    }

    #endregion
}