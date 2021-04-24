using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Photon.Pun;
using Tenacious.Collections;
using Game.UI;
using Tenacious.Audio;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviourPunCallbacks
{
    [Serializable]
    public class PlayerDescriptor
    {
        public string name = "Test";
        public MBGraphNode startNode;
        public Vector3 positionOffset = new Vector3(2.5f, 0.0f, 2.5f);
        public List<Checkpoint> checkpoints;
    }

    [SerializeField] public MBGraph gridGraph;

    public GameplayCameraRig cameraRig;
    public List<Transform> goalPlatforms;

    public List<MBGraphNode> blockedOffNodes;

    public Transform playersParentTransform;
    public List<PlayerDescriptor> playerDescriptors;

    [SerializeField] private float maxActionPoints;

    public List<AbstractPlayer> players = new List<AbstractPlayer>();

    public List<AbstractPlayer> Players { get => players; }

    public int currentPlayer = 0;
    public int totalPlayersInGame = 0;
    public bool hasInitializedPlayers = false;

    private bool isCRTurnUpdateRunning;
    
    // Network bools that ensure order is followed when starting game
    private bool isLoaded;
    private bool isSpawningPlayers = true;
    private bool isInitializingPlayers = true;
    private bool finishedPregameSetup;

    [Header("Debug Settings")] 
    public bool usingNetwork = true;
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
        AudioManager.Instance.PlayMusic("Gameplay1");

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
        // Update PlayerCount if Playing Online
        if (usingNetwork)
        {
            photonView.RPC("LoadedIntoScene", RpcTarget.AllBuffered);
        }
        // Offline Mode Setup
        else
        {
            for (var i = 0; i < debugHumanCount; i++)
            {
                GameObject playerObj = Instantiate(HumanPlayerPrefab, playersParentTransform);
                HumanPlayer player = playerObj.GetComponent<HumanPlayer>();
                player.InitializePlayer(maxActionPoints, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode.nodeId, playerDescriptors[i].name, i);
                players.Add(player);
            }
            
            for (var i = 0 + debugHumanCount; i < debugAICount + debugHumanCount; i++)
            {
                GameObject aiObj = Instantiate(AIPlayerPrefab, playersParentTransform);
                AIPlayer player = aiObj.GetComponent<AIPlayer>();
                player.InitializePlayer(maxActionPoints, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode.nodeId, playerDescriptors[i].name, i);
                players.Add(player);
            }

            sharedHud.Initialize();
        }
    }

    private void Update()
    {
        // =================================================
        // SPAWNING AND INITIALIZING PLAYERS / AI ON NETWORK
        // =================================================
        
        // Instantiate Players and Initialize Them Across the Network
        if (totalPlayersInGame == PhotonNetwork.CurrentRoom.PlayerCount && !isLoaded)
        {
            GameObject playerObj = PhotonNetwork.Instantiate("HumanPlayer", new Vector3(0,0,0), Quaternion.identity);
            HumanPlayer playerScript = playerObj.GetComponent<HumanPlayer>();

            playerScript.photonView.RPC("InitializePlayerOnNetwork", RpcTarget.All, PhotonNetwork.LocalPlayer);

            isLoaded = true;
        }
        
        // Instantiate AI Across the Network After Ensuring All Players Are In
        if (usingNetwork && isSpawningPlayers && playersParentTransform.childCount == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient)
        {
            isSpawningPlayers = false;

            // Spawn in AI
            for (var i = 0; i < NetworkManager.Instance.aiPlayerCount; i++)
            {
                GameObject playerObj = PhotonNetwork.Instantiate("AIPlayer", new Vector3(0, 0, 0), Quaternion.identity);
                playerObj.SetActive(true);
            }
        }

        // When All GameObjects are Built - Initialize All Players and HUD Across the Network
        if (usingNetwork && isInitializingPlayers && playersParentTransform.childCount == PhotonNetwork.PlayerList.Length + NetworkManager.Instance.aiPlayerCount && PhotonNetwork.IsMasterClient)
        {
            isInitializingPlayers = false;
            
            photonView.RPC("InitializePlayers", RpcTarget.All);
            sharedHud.photonView.RPC("Initialize", RpcTarget.All);
            photonView.RPC("InitializationComplete", RpcTarget.All);
        }

        // ===================
        // MAIN GAMEPLAY LOOP
        // ===================
        
        if (finishedPregameSetup)
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
                    // Update Turn Order (ONLY FOR AI) - Turn updates for Human Players on End Turn Press
                    if (PhotonNetwork.IsMasterClient && Players[currentPlayer].CompareTag("AIPlayer"))
                    {
                        photonView.RPC("UpdateCurrentPlayer", RpcTarget.All);
                    }
                }
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                { 
                    photonView.RPC("ResetCurrentPlayer", RpcTarget.All);
                }
            }
        }

        // **Emergency failsafe** Please avoid pressing this key during the demo as it may break things (this is only if the AI freezes up for some reason)
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.F12))
            photonView.RPC("UpdateCurrentPlayer", RpcTarget.All);
    }

    [PunRPC]
    private void InitializationComplete()
    {
        hasInitializedPlayers = true;
    }

    [PunRPC]
    private void InitializePlayers()
    {
        // Set the Size of the List
        foreach (Transform player in playersParentTransform)
        {
            Players.Add(null);
        }
        
        var index = 0;
        var aiID = 1;
        
        // Initialize Players first since their id starts from index 0
        foreach (Transform player in playersParentTransform)
        {
            if (player.gameObject.CompareTag("HumanPlayer"))
            {
                HumanPlayer humanPlayer = player.GetComponent<HumanPlayer>();
                
                var id = (int)NetworkManager.Instance.spawnIndices[player.name];

                players[id] = humanPlayer;
                humanPlayer.GetComponent<MeshFilter>().mesh = humanPlayer.VehicleSkins[index];
                humanPlayer.InitializePlayer(maxActionPoints, playerDescriptors[id].positionOffset, playerDescriptors[id].startNode.nodeId, player.name, id);
                index++;   
            }
        }
        
        // Initialize AI afterwards
        foreach (Transform player in playersParentTransform)
        { 
            if (player.gameObject.CompareTag("AIPlayer"))
            {
                AIPlayer aiPlayer = player.GetComponent<AIPlayer>();
                aiPlayer.GetComponent<MeshFilter>().mesh = aiPlayer.VehicleSkins[index];
                aiPlayer.InitializePlayer(maxActionPoints, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode.nodeId, "AI " + aiID, index);
                players[index] = aiPlayer;
                index++;
                aiID++;
            }
        }
        
        finishedPregameSetup = true;
    }
    
    [PunRPC]
    public void LoadedIntoScene()
    {
        totalPlayersInGame++;
    }

    [PunRPC]
    private void UpdateCurrentPlayer()
    {
        currentPlayer++;
    }

    [PunRPC]
    private void ResetCurrentPlayer()
    {
        currentPlayer = 0;
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
