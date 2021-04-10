using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerManagerTemp : Game.AI.Pathfinding
{
    [SerializeField] private List<PlayerController> players;
    [SerializeField] private MBGraphNode startNode;
    [SerializeField] private MBGraphNode goalNode;

    private Vector3[] positionOffsets = new[] {
        new Vector3(2.5f, 0.0f, 2.5f),
        new Vector3(2.5f, 0.0f, -2.5f),
        new Vector3(-2.5f, 0.0f, 2.5f),
        new Vector3(-2.5f, 0.0f, -2.5f)
    };
    
    private void Start()
    {
        InitializePlayers();

        List<GraphNode<GameObject>> movePath = FindPath(mbGraph.graph, startNode.nodeId, goalNode.nodeId);

        foreach (PlayerController player in players)
        {
            player.Move(movePath, movePath.Count);
        }
    }

    private void InitializePlayers()
    {
        int offsetIndex = 0;
        foreach(PlayerController player in players)
        {
            player.GetComponent<PlayerController>().InitializePlayer(99, positionOffsets[offsetIndex]);
            offsetIndex++;
            player.Spawn(startNode);
        }
    }
}
