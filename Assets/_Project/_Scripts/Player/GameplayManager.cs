using UnityEngine;

using System;
using System.Collections.Generic;

using Game.AI;

using Tenacious.Collections;

[RequireComponent(typeof(Pathfinding))]
public class GameplayManager : MonoBehaviour
{
    [SerializeField] private List<AbstractPlayer> players;
    [SerializeField] private MBGraphNode startNode;
    [SerializeField] private MBGraphNode goalNode;

    private Pathfinding pathfinding;
    private int currentPlayer = 0;

    private bool isCRTurnUpdateRunning;

    private Vector3[] positionOffsets = new Vector3[] {
        new Vector3(2.5f, 0.0f, 2.5f),
        new Vector3(2.5f, 0.0f, -2.5f),
        new Vector3(-2.5f, 0.0f, 2.5f),
        new Vector3(-2.5f, 0.0f, -2.5f)
    };

    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();

        InitializePlayers();
    }

    private void Start()
    {
        List<GraphNode<GameObject>> movePath = pathfinding.FindPath(startNode.nodeId, goalNode.nodeId);
    }

    private void Update()
    {
        if (players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Standby)
            players[currentPlayer].StandbyPhase();
        else if (players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.Main)
            players[currentPlayer].MainPhase();
        else if (players[currentPlayer].Phase == AbstractPlayer.EPlayerPhase.End)
            players[currentPlayer].EndPhase();
    }

    private void InitializePlayers()
    {
        int offsetIndex = 0;
        foreach(AbstractPlayer player in players)
        {
            player.Controller.InitializePlayer(99, positionOffsets[offsetIndex]);
            offsetIndex++;
            player.Controller.Spawn(startNode);
        }
    }
}
