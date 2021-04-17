using UnityEngine;

using System;
using System.Collections.Generic;


using Game.AI;
using Photon.Pun;
using Tenacious;
using Tenacious.Collections;

public class GameplayManager : MBSingleton<GameplayManager>
{
    [Serializable]
    public class PlayerDescriptor
    {
        public MBGraphNode startNode;
        public Vector3 positionOffset = new Vector3(2.5f, 0.0f, 2.5f);
    }

    public List<PlayerDescriptor> playerDescriptors;

    [SerializeField] private Transform playersParent;
    [SerializeField] private float maxActionPoints;

    private List<AbstractPlayer> players = new List<AbstractPlayer>();

    public List<AbstractPlayer> Players { get => players; }

    private int currentPlayer = 0;

    private bool isCRTurnUpdateRunning;
    
    private bool isLoadingPlayers = true;

    [Header("Debug Settings")] 
    [SerializeField] private bool usingNetwork = true;
    [SerializeField] private int debugHumanCount;
    [SerializeField] private int debugAICount;
    [SerializeField] private GameObject HumanPlayerPrefab;
    [SerializeField] private GameObject AIPlayerPrefab;
    
    protected override void Awake()
    {
        base.Awake();
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
                player.InitializePlayer(99, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode);
                players.Add(player);
            }

            // Initialize AI Players
            for (var i = 0 + debugHumanCount; i < debugAICount + debugHumanCount; i++)
            {
                GameObject aiObj = Instantiate(AIPlayerPrefab);
                AIPlayer player = aiObj.GetComponent<AIPlayer>();
                player.InitializePlayer(99, playerDescriptors[i].positionOffset, playerDescriptors[i].startNode);
                players.Add(player);
            }
            
        }
    }

    private void Update()
    {
        // Initialize all players through the network and pass player list to game manager
        if (usingNetwork && isLoadingPlayers && NetworkManager.Instance.humanPlayers.Length == NetworkManager.Instance.humanPlayersInGame)
        {
            isLoadingPlayers = false;

            var index = 0;
            
            foreach (var player in NetworkManager.Instance.humanPlayers)
            {
                player.InitializePlayer(99, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode);
                players.Add(player);
                index++;
            }
            
            // TODO: Instantiate AI Players
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
                Players[currentPlayer].Phase = AbstractPlayer.EPlayerPhase.None;
                ++currentPlayer;
            }
        }
        else
        {
            currentPlayer = 0;
        }
    }

    //private void InitializePlayers()
    //{
    //    for (int i = 0; i < playerDescriptors.Count; ++i)
    //    {
    //        AbstractPlayer player = SpawnPlayer(playerDescriptors[i].playerPrefab, playerDescriptors[i].startNode, playerDescriptors[i].positionOffset);
    //        Players.Add(player);

    //        if (player is HumanPlayer humanPlayer)
    //        {
    //            PlayerHUDManager.Instance.InitializeUI(humanPlayer);
    //        }
    //    }
    //}

    //private AbstractPlayer SpawnPlayer(GameObject playerPrefab, MBGraphNode startingnode, Vector3 positionOffset)
    //{
    //    GameObject playerObj = Instantiate(playerPrefab);
    //    playerObj.transform.parent = playersParent;
    //    AbstractPlayer playerComponent = playerObj.GetComponent<AbstractPlayer>();
    //    playerComponent.InitializePlayer(maxActionPoints, positionOffset);
    //    playerComponent.Spawn(startingnode);

    //    return playerComponent;
    //}
}
