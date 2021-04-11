using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;
using Game.AI;

[RequireComponent(typeof(Pathfinding))]
public class GameplayManager : MonoBehaviour
{
    [SerializeField] private List<AbstractPlayer> players;
    [SerializeField] private MBGraphNode startNode;
    [SerializeField] private MBGraphNode goalNode;

    private Pathfinding pathfinding;
    private int currentPlayer = 0;

    private Vector3[] positionOffsets = new[] {
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
