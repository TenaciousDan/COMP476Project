using UnityEngine;

using System;
using System.Collections.Generic;


using Game.AI;
using Photon.Pun;
using Tenacious;
using Tenacious.Collections;
using Game.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameplayManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Serializable]
    public class PlayerDescriptor
    {
        public string name = "Test";
        public MBGraphNode startNode;
        public Vector3 positionOffset = new Vector3(2.5f, 0.0f, 2.5f);
    }

    [SerializeField] public MBGraph gridGraph;

    public List<PlayerDescriptor> playerDescriptors;

    [SerializeField] private Transform playersParent;
    [SerializeField] private float maxActionPoints;

    private List<AbstractPlayer> players = new List<AbstractPlayer>();

    public List<AbstractPlayer> Players { get => players; }

    [SerializeField]
    private int currentPlayer = 0;

    private bool isCRTurnUpdateRunning;
    
    private bool isLoadingPlayers = true;

    [Header("Debug Settings")] 
    [SerializeField] private bool usingNetwork = true;
    [SerializeField] private int debugHumanCount;
    [SerializeField] private int debugAICount;
    [SerializeField] private GameObject HumanPlayerPrefab;
    [SerializeField] private GameObject AIPlayerPrefab;
    [SerializeField] private SharedHUD sharedHud;

    private static GameplayManager instance;
    private static bool reinitializationPermitted = false;
    private static bool destroyed = false;
    private static object mutex = new object();
    [Tooltip("Allow this object to be re-initialized after being destroyed.")]
    [SerializeField] private bool allowReinitialization = false;
    [Tooltip("Destroy this object when a new scene is loaded.")]
    [SerializeField] private bool destroyOnLoad = false;

    public static GameplayManager Instance
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
	
    private void Start()
    {
        // Inform all players after loading into scene
        if (usingNetwork)
        {
            NetworkManager.Instance.humanPlayers = new HumanPlayer[PhotonNetwork.PlayerList.Length];
            NetworkManager.Instance.LoadedIntoGame();
        }
        // Instantiate all players on local machine
        else
        {
            // Initialize Human Players
            for (var i = 0; i < debugHumanCount; i++)
            {
                GameObject playerObj = Instantiate(HumanPlayerPrefab);
                HumanPlayer player = playerObj.GetComponent<HumanPlayer>();
                player.InitializePlayer(maxActionPoints, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode.nodeId, playerDescriptors[i].name);
                players.Add(player);
            }

            // Initialize AI Players
            for (var i = 0 + debugHumanCount; i < debugAICount + debugHumanCount; i++)
            {
                GameObject aiObj = Instantiate(AIPlayerPrefab);
                AIPlayer player = aiObj.GetComponent<AIPlayer>();
                player.InitializePlayer(maxActionPoints, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode.nodeId, playerDescriptors[i].name);
                players.Add(player);
            }

            sharedHud.InitializeTest();
        }
    }

    private void Update()
    {
        // Initialize all players through the network and pass player list to game manager
        if (usingNetwork && isLoadingPlayers && NetworkManager.Instance.humanPlayers.Length == NetworkManager.Instance.humanPlayerCount && PhotonNetwork.IsMasterClient)
        {
            isLoadingPlayers = false;

            var index = 0;
            
            for (int i = 0; i < NetworkManager.Instance.aiPlayerCount; i++)
            {
                NetworkManager.Instance.aiPlayers[i].InitializePlayer(99, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode.nodeId, "AI " + i);
                players.Add(NetworkManager.Instance.aiPlayers[i]);
                index++;
            }
            
            foreach (var player in NetworkManager.Instance.humanPlayers)
            {
                NetworkManager.Instance.InitializeHumanPlayer(player, maxActionPoints, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode.nodeId, player.name);
                players.Add(player);
                index++;
            }

            photonView.RPC("SetNumberOfUI", RpcTarget.All, NetworkManager.Instance.humanPlayers.Length);

            for (int i = 0; i < NetworkManager.Instance.humanPlayers.Length; i++)
            {
                photonView.RPC("InitializeUI", RpcTarget.All, i);
            }
        }

        photonView.RPC("ShowUI", RpcTarget.All, currentPlayer);

        if (currentPlayer < Players.Count)
        {
            if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Standby || Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.None)
                Players[currentPlayer].StandbyPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Main)
                Players[currentPlayer].MainPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.End)
                Players[currentPlayer].EndPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.PassTurn)
            {
                Players[currentPlayer].Phase = AbstractPlayer.EPlayerPhase.None;
                //++currentPlayer;
                photonView.RPC("UpdateCurrentPlayer", RpcTarget.All);
            }
        }
        else
        {
            //currentPlayer = 0;
            photonView.RPC("ResetCurrentPlayer", RpcTarget.All);
        }

        //photonView.RPC("UpdatePlayerTurns", RpcTarget.All);
    }

    public void EndPlayerTurn(string name)
    {
        photonView.RPC("EndTurn", RpcTarget.All, name);
    }

    [PunRPC]
    private void EndTurn(string name)
    {
        var player = players.Find(p => p.Name.Equals(name));
        player.Phase = AbstractPlayer.EPlayerPhase.End;
    }

    [PunRPC]
    private void ShowUI(int index)
    {
        PlayerHUDManager.Instance.huds[index].gameObject.SetActive(true);
    }

    [PunRPC]
    private void SetNumberOfUI(int num)
    {
        PlayerHUDManager.Instance.numToUse = num;
        //PlayerHUDManager.Instance.huds[0].gameObject.SetActive(true);
    }

    [PunRPC]
    private void InitializeUI(int index)
    {
        PlayerHUDManager.Instance.huds[index].player = NetworkManager.Instance.humanPlayers[index];
    }

    [PunRPC]
    private void ResetCurrentPlayer()
    {
        currentPlayer = 0;
    }

    [PunRPC]
    private void UpdateCurrentPlayer()
    {
        ++currentPlayer;
    }

    [PunRPC]
    private void UpdatePlayerTurns()
    {
        if (currentPlayer < Players.Count)
        {
            if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Standby || Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.None)
                Players[currentPlayer].StandbyPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Main)
                Players[currentPlayer].MainPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.End)
                Players[currentPlayer].EndPhaseUpdate();
            else if (Players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.PassTurn)
            {
                Players[currentPlayer].Phase = AbstractPlayer.EPlayerPhase.None;
                ++currentPlayer;
            }
        }
        else
        {
            currentPlayer = 0;
        }

        //print($"Current player {currentPlayer}");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentPlayer);
        }
        else if (stream.IsReading)
        {
            currentPlayer = (int)stream.ReceiveNext();
        }
    }

    #region SINGLETON
    private static GameplayManager CreateInstance(GameObject game_object)
    {
        lock (mutex)
        {
            // Create new instance if one doesn't already exist
            if (instance == null)
            {
                if (game_object == null)
                {
                    // create the GameObject that will house the singleton instance
                    game_object = new GameObject("GameplayManager");
                    game_object.AddComponent<GameplayManager>();
                }

                instance = game_object.GetComponent<GameplayManager>();
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
