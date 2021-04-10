using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<PlayerController> players;
    [SerializeField] List<MBGraphNode> testList = new List<MBGraphNode>();

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
        foreach (PlayerController player in players)
        {
            player.Move(testList, 2);
        }
    }

    private void InitializePlayers()
    {
        int offsetIndex = 0;
        foreach(PlayerController player in players)
        {
            player.GetComponent<PlayerController>().InitializePlayer(3, positionOffsets[offsetIndex]);
            offsetIndex++;
        }
    }
}
