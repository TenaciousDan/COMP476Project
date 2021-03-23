using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private MBGraph mbGraph;
    [SerializeField] private MBGraphNode startNode;
    [SerializeField] private MBGraphNode goalNode;

    [SerializeField] List<Player> players;

    private Vector3[] positionOffsets = new[] {
        new Vector3(0.5f, 0.0f, 0.5f),
        new Vector3(0.5f, 0.0f, -0.5f),
        new Vector3(-0.5f, 0.0f, 0.5f),
        new Vector3(-0.5f, 0.0f, -0.5f)
    };

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayers();
        List<MBGraphNode> testList = new List<MBGraphNode>();
        testList.Add(startNode);
        testList.Add(goalNode);
        foreach (Player player in players)
        {
            player.Move(testList, 1);
        }
    }

    private void InitializePlayers()
    {
        int offsetIndex = 0;
        foreach(Player player in players)
        {
            player.GetComponent<Player>().InitializePlayer(3, positionOffsets[offsetIndex]);
            offsetIndex++;
        }
    }
}
