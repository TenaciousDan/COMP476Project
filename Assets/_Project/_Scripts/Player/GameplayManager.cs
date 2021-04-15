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

    private List<AbstractPlayer> players = new List<AbstractPlayer>();
    public List<AbstractPlayer> Players { get => players; }

    private int currentPlayer = 0;

    private bool isCRTurnUpdateRunning;
    
    private bool isLoadingPlayers = true;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Inform all players after loading into scene
        NetworkManager.Instance.humanPlayers = new HumanPlayer[PhotonNetwork.PlayerList.Length];
        NetworkManager.Instance.LoadedIntoGame();
    }

    private void Update()
    {
        // Only run when all players are loaded, pass player list to Gameplay Manager and initialize all players
        if (NetworkManager.Instance.humanPlayers.Length == NetworkManager.Instance.humanPlayersInGame && isLoadingPlayers)
        {
            isLoadingPlayers = false;

            var index = 0;
            
            foreach (var player in NetworkManager.Instance.humanPlayers)
            {
                player.InitializePlayer(99, playerDescriptors[index].positionOffset, playerDescriptors[index].startNode);
                players.Add(player);
                index++;
            }
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
    }
}
