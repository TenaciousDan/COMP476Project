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

        private AbstractPlayer player;
        public AbstractPlayer Player { get => player == null ? playerPrefab.GetComponent<AbstractPlayer>() : player; }
    }

    public List<PlayerDescriptor> playerDescriptors;

    [SerializeField] private Transform playersParent;

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
        if (currentPlayer < playerDescriptors.Count)
        {
            if (playerDescriptors[currentPlayer].Player.Phase == AbstractPlayer.EPlayerPhase.Standby)
                playerDescriptors[currentPlayer].Player.StandbyPhaseUpdate();
            else if (playerDescriptors[currentPlayer].Player.Phase == AbstractPlayer.EPlayerPhase.Main)
                playerDescriptors[currentPlayer].Player.MainPhaseUpdate();
            else if (playerDescriptors[currentPlayer].Player.Phase == AbstractPlayer.EPlayerPhase.End)
                playerDescriptors[currentPlayer].Player.EndPhaseUpdate();
        }
    }

    private void InitializePlayers()
    {
        for (int i = 0; i < playerDescriptors.Count; ++i)
        {
            playerDescriptors[i].Player.InitializePlayer(99, playerDescriptors[i].positionOffset);
            SpawnPlayer(playerDescriptors[i].playerPrefab, playerDescriptors[i].startNode, playerDescriptors[i].positionOffset);
        }
    }

    private void SpawnPlayer(GameObject playerPrefab, MBGraphNode startingnode, Vector3 positionOffset)
    {
        GameObject playerObj = Instantiate(playerPrefab);
        AbstractPlayer playerComponent = playerObj.GetComponent<AbstractPlayer>();
        playerComponent.PositionNode = startingnode;
        Vector3 newWorldPosition = startingnode.transform.position + positionOffset;
        playerObj.transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
    }
}
