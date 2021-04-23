﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Tenacious.Collections;
using Tenacious.Audio;

public abstract class AbstractPlayer : MonoBehaviourPunCallbacks
{
    public enum EPlayerPhase { None, Standby, Main, End, PassTurn }
    public enum EPlayerState { Waiting, Busy }
    public EPlayerPhase Phase { get; set; }
    public EPlayerState State { get; set; }
    public string Name { get; set; }
    [SerializeField]
    protected GameObject shieldObject;

    private float maxActionPoints;
    private float moveSpeed = 10;
    private float rotationSpeed = 10;
    private bool hasShield = false;
    private float costPerMovement = 1;
    private float pointsDeficit = 0;

    public List<Checkpoint> checkpoints;

    public bool gameIsOver;

    public int ID
    {
        get; protected set;
    }

    public Inventory Inventory
    {
        get; private set;
    }

    public float CurrentActionPoints;

    public float MaxActionPoints
    {
        get
        {
            return maxActionPoints;
        }
    }

    public float CostPerMovement { get => costPerMovement; set => costPerMovement = value; }

    public MBGraphNode PositionNode
    {
        get; set;
    }

    // offset so multiple players can be on the same tile
    public Vector3 PositionOffset
    {
        get; private set;
    }

    protected virtual void Awake()
    {
        Inventory = GetComponent<Inventory>();
        transform.parent = GameplayManager.Instance.playersParentTransform;
    }

    // assign values to member variables
    [PunRPC]
    public virtual void InitializePlayer(float _maxActionPoints, Vector3 _positionOffset, string _startingNodeId, string name, int playerIndex)
    {
        MBGraphNode startingNode = GameplayManager.Instance.gridGraph.graph[_startingNodeId].Data.GetComponent<MBGraphNode>();
        
        Name = name;
        maxActionPoints = CurrentActionPoints = _maxActionPoints;
        PositionOffset = _positionOffset;
        PositionNode = startingNode;
        Inventory.InitializeList();

        this.checkpoints = GameplayManager.Instance.playerDescriptors[playerIndex].checkpoints;
        
        Vector3 newWorldPosition = startingNode.transform.position + _positionOffset;
        transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Standby
    /// </summary>
    public virtual void StandbyPhaseUpdate()
    {
        Phase = EPlayerPhase.Standby;

        // TODO: do standby phase things
        photonView.RPC("MainTurn", RpcTarget.All);

        Phase = EPlayerPhase.Main;
    }

    [PunRPC]
    public void MainTurn()
    {
        FillActionPoints();
        RemoveActionPoints(pointsDeficit);
        pointsDeficit = 0;
        DeactivateShield();
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Main
    /// </summary>
    public abstract void MainPhaseUpdate();

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = End
    /// </summary>
    public virtual void EndPhaseUpdate()
    {
        Phase = EPlayerPhase.End;

        // TODO: do end phase things

        Phase = EPlayerPhase.PassTurn;
    }

    public bool SpendActionPoints(float numActionPoints)
    {
        if (numActionPoints > CurrentActionPoints)
        {
            Debug.LogWarning("Not enough action points for this action.");
            return false;
        }
        else
        {
            CurrentActionPoints -= numActionPoints;
            return true;
        }
    }

    [PunRPC]
    public void AddItemToInventory(Scriptable_Base item)
    {
        Inventory.AddItem(item);
    }

    [PunRPC]
    public void RemoveItemFromInventory(int index)
    {
        Inventory.RemoveItem(index);
    }

    [PunRPC]
    public void FillActionPoints()
    {
        CurrentActionPoints = maxActionPoints;
    }

    public void AddActionPoints(float numActionPoints, bool exceedLimit = false)
    {
        CurrentActionPoints += numActionPoints;
        if (!exceedLimit && CurrentActionPoints > maxActionPoints)
        {
            CurrentActionPoints = maxActionPoints;
        }
        print("Added action points. Player now has " + CurrentActionPoints + " action points.");
    }

    [PunRPC]
    public void RemoveActionPoints(float numActionPoints)
    {
        CurrentActionPoints -= numActionPoints;
        if (CurrentActionPoints < 0)
        {
            CurrentActionPoints = 0;
        }
        //print("Removed " + numActionPoints + " action points. Player now has " + CurrentActionPoints + " action points.");
    }

    public void ActivateShield()
    {
        hasShield = true;
        shieldObject.SetActive(true);
        //print("shield activated");
    }

    public void DeactivateShield()
    {
        hasShield = false;
        shieldObject.SetActive(false);
        //print("shield deactivated");
    }

    public void GetHit(float numActionPoints)
    {
        if (hasShield)
        {
            hasShield = false;
        }
        else
        {
            AddPointsDeficit(numActionPoints);
        }
    }

    public void AddPointsDeficit(float numActionPoints)
    {
        pointsDeficit += numActionPoints;
    }

    public virtual void Move(List<GraphNode<GameObject>> path)
    {
        if (tag.Equals("HumanPlayer") || PhotonNetwork.IsMasterClient)
            StartCoroutine(CRMove(path));
    }

    protected IEnumerator CRMove(List<GraphNode<GameObject>> path)
    {
        if (path == null) path = new List<GraphNode<GameObject>>();
        float cost = path.Count * CostPerMovement;
        if (path.Count > 0 && SpendActionPoints(cost))
        {
            foreach (GraphNode<GameObject> node in path)
            {
                if (photonView.IsMine)
                {
                    photonView.RPC("UpdatePositionNode", RpcTarget.All, node.Id);
                }

                Vector3 newWorldPosition = node.Data.transform.position + PositionOffset;
                newWorldPosition = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);

                Vector3 targetDirection = (node.Data.transform.position + PositionOffset) - transform.position;
                while (transform.position != newWorldPosition)
                {
                    transform.position = Vector3.MoveTowards(transform.position, newWorldPosition, moveSpeed * Time.deltaTime);

                    Quaternion newRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed * Time.deltaTime);
                    transform.rotation = newRotation;
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    yield return null;
                }
            }
        }
    }

    [PunRPC]
    public void UpdatePositionNode(string nodeId)
    {
        PositionNode = GameplayManager.Instance.gridGraph.graph[nodeId].Data.GetComponent<MBGraphNode>();
    }

    [PunRPC]
    public void ChangeMusicTrack(string trackName)
    {
        AudioManager.Instance.PlayMusic(trackName);
    }

    private void CheckpointReached(Checkpoint cp)
    {
        if (cp.isGoal)
        {
            // This player has won the game
            gameIsOver = true;
            if (GameplayManager.Instance.usingNetwork)
                photonView.RPC("ChangeMusicTrack", RpcTarget.All, "ReachedTheGoal");

            transform.position = GameplayManager.Instance.goalPlatforms[0].position;
            GameplayManager.Instance.cameraRig.target = transform;
        }

        print("cpoints contains cp :" + checkpoints.Contains(cp));
        if (checkpoints.Contains(cp))
            checkpoints.Remove(cp);

        if (checkpoints.Count == 1 && GameplayManager.Instance.usingNetwork)
            photonView.RPC("ChangeMusicTrack", RpcTarget.All, "VictoryIsWithinReach");
    }

    private void OnTriggerEnter(Collider other)
    {
        Checkpoint cp = other.GetComponent<Checkpoint>();
        if (cp != null)
        {
            CheckpointReached(cp);
        }
    }
}
