using UnityEngine;

using System;
using System.Collections.Generic;

using Game.AI;

using Tenacious.Collections;

[RequireComponent(typeof(Pathfinding))]
public class GameplayManager : MonoBehaviour
{
    [Serializable]
    public class PlayerDescriptor
    {
        public GameObject playerPrefab;
        public MBGraphNode startNode;
        public Vector3 positionOffset = new Vector3(2.5f, 0.0f, 2.5f);
    }

    public List<PlayerDescriptor> playerDescriptors;

    [SerializeField] private Transform playersParent;

    private List<AbstractPlayer> players = new List<AbstractPlayer>();
    public List<AbstractPlayer> Players { get => players; }

    private Pathfinding pathfinding;
    private int currentPlayer = 0;

    private bool isCRTurnUpdateRunning;

    private void Awake()
    {
        InitializePlayers();
    }

    private void Start()
    {
        //
    }

    private void Update()
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
    }

    private void InitializePlayers()
    {
        for (int i = 0; i < playerDescriptors.Count; ++i)
        {
            AbstractPlayer player = SpawnPlayer(playerDescriptors[i].playerPrefab, playerDescriptors[i].startNode, playerDescriptors[i].positionOffset);
            player.InitializePlayer(99, playerDescriptors[i].positionOffset);
            Players.Add(player);
        }
    }

    private AbstractPlayer SpawnPlayer(GameObject playerPrefab, MBGraphNode startingnode, Vector3 positionOffset)
    {
        GameObject playerObj = Instantiate(playerPrefab);
        AbstractPlayer playerComponent = playerObj.GetComponent<AbstractPlayer>();
        playerComponent.PositionNode = startingnode;
        Vector3 newWorldPosition = startingnode.transform.position + positionOffset;
        playerObj.transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);

        return playerComponent;
    }
}
