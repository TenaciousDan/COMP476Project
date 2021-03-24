using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<PlayerController> players;

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
        foreach (PlayerController player in players)
        {
            player.Move(testList, 1);
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
