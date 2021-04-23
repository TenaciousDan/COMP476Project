using UnityEngine;

using System;
using System.Collections.Generic;

using Game.AI;
using Photon.Pun;
using Tenacious;
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

    private bool isCRTurnUpdateRunning;
    
    private bool isLoadingPlayers = true;

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
                GameObject playerObj = Instantiate(HumanPlayerPrefab, playersParentTransform);
                HumanPlayer player = playerObj.GetComponent<HumanPlayer>();
                player.InitializePlayer(maxActionPoints, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode.nodeId, playerDescriptors[i].name, i);
                players.Add(player);
            }

            // Initialize AI Players
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
        // Initialize all players through the network and pass player list to game manager
        if (usingNetwork && isLoadingPlayers && NetworkManager.Instance.humanPlayers.Length == NetworkManager.Instance.humanPlayerCount && PhotonNetwork.IsMasterClient)
        {
            isLoadingPlayers = false;
            
            photonView.RPC("SpawnPlayers", RpcTarget.All);
            sharedHud.photonView.RPC("Initialize", RpcTarget.All);
        }

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
                //Players[currentPlayer].Phase = AbstractPlayer.EPlayerPhase.None;
                photonView.RPC("UpdateCurrentPlayer", RpcTarget.All);
            }
        }
        else
        {
            photonView.RPC("ResetCurrentPlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SpawnPlayers()
    {
        var index = 0;
            
        for (int i = 0; i < NetworkManager.Instance.aiPlayerCount; i++)
        {
            NetworkManager.Instance.aiPlayers[i].InitializePlayer(99, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode.nodeId, "AI " + i, index);
            players.Add(NetworkManager.Instance.aiPlayers[i]);
            index++;
        }

        foreach (var player in NetworkManager.Instance.humanPlayers)
        {
            NetworkManager.Instance.InitializeHumanPlayer(player, maxActionPoints, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode.nodeId, playerDescriptors[index].name, index);
            players.Add(player);
            index++;
        }
    }

    [PunRPC]
    private void UpdateCurrentPlayer()
    {
        Players[currentPlayer].Phase = AbstractPlayer.EPlayerPhase.None;
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
