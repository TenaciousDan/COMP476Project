using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Tenacious.Collections;

public abstract class AbstractPlayer : MonoBehaviourPunCallbacks
{
    public enum EPlayerPhase { None, Standby, Main, End, PassTurn }
    public enum EPlayerState { Waiting, Busy }
    public EPlayerPhase Phase { get; set; }
    public EPlayerState State { get; set; }
    private float maxActionPoints;
    private float moveSpeed = 10;
    private float rotationSpeed = 10;
    private bool hasShield = false;
    private float costPerMovement = 1;

    public Inventory Inventory
    {
        get; private set;
    }

    public float CurrentActionPoints
    {
        get; private set;
    }

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
    }

    // assign values to member variables
    [PunRPC]
    public virtual void InitializePlayer(float _maxActionPoints, Vector3 _positionOffset, MBGraphNode _startingNode)
    {
        maxActionPoints = CurrentActionPoints = _maxActionPoints;
        PositionOffset = _positionOffset;
        PositionNode = _startingNode;

        Vector3 newWorldPosition = _startingNode.transform.position + _positionOffset;
        transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);

        
    }

    /// <summary>
    /// Will be called by GameManager in the Update() as long as Phase = Standby
    /// </summary>
    public virtual void StandbyPhaseUpdate()
    {
        Phase = EPlayerPhase.Standby;

        // TODO: do standby phase things
        AddActionPoints(1, false);

        Phase = EPlayerPhase.Main;
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

    public void RemoveActionPoints(float numActionPoints)
    {
        CurrentActionPoints -= numActionPoints;
        if (CurrentActionPoints < 0)
        {
            CurrentActionPoints = 0;
        }
        print("Removed action points. Player now has " + CurrentActionPoints + " action points.");
    }

    public void ActivateShield()
    {
        hasShield = true;
        print("shield activated");
    }

    public void GetHit(float numActionPoints)
    {
        if (hasShield)
        {
            hasShield = false;
        }
        else
        {
            RemoveActionPoints(numActionPoints);
        }
    }

    protected IEnumerator CRMove(List<GraphNode<GameObject>> path)
    {
        if (path == null) path = new List<GraphNode<GameObject>>();
        float cost = path.Count * CostPerMovement;
        if (path.Count > 0 && SpendActionPoints(cost))
        {
            foreach (GraphNode<GameObject> node in path)
            {
                PositionNode = node.Data.GetComponent<MBGraphNode>();
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
}
