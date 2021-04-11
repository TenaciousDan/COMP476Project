using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tenacious.Collections;

[RequireComponent(typeof(MBGraph))]
public class GameGrid : MonoBehaviour
{
    [SerializeField] private GameObject gridSquarePrefab;

    private MBGraph mbGraph;

    private void Awake()
    {
        mbGraph = GetComponent<MBGraph>();

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < mbGraph.transform.childCount; ++i)
        {
            GameObject obj = Instantiate(gridSquarePrefab, mbGraph.transform.GetChild(i));
            obj.SetActive(false);
        }
    }
}
