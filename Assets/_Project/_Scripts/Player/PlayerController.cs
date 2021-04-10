using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int maxActionPoints;
    private float moveSpeed = 10.0f;
    private float rotationSpeed = 10.0f;
    private bool hasShield = false;

    public int currentActionPoints
    {
        get; private set;
    }
    
    public MBGraphNode nodePosition
    {
        get; private set;
    }

    // offset so multiple players can be on the same tile
    public Vector3 positionOffset
    {
        get; private set;
    }

    public void Move(List<GraphNode<GameObject>> nodes, int cost)
    {
        if (SpendActionPoints(cost))
        {
            StartCoroutine(SmoothMove(nodes));
        }
    }

    // assign values to member variables
    public void InitializePlayer(int _maxActionPoints, Vector3 _positionOffset)
    {
        maxActionPoints = currentActionPoints = _maxActionPoints;
        positionOffset = _positionOffset;
    }

    public void Spawn(MBGraphNode node)
    {
        nodePosition = node;
        Vector3 newWorldPosition = node.transform.position + positionOffset;
        transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
    }

    public bool SpendActionPoints(int numActionPoints)
    {
        if(numActionPoints > currentActionPoints)
        {
            Debug.LogWarning("Not enough action points for this action.");
            return false;
        }
        else
        {
            currentActionPoints -= numActionPoints;
            return true;
        }
    }
    
    public void FillActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }
    
    public void AddActionPoints(int numActionPoints, bool exceedLimit = false)
    {
        currentActionPoints += numActionPoints;
        if (!exceedLimit && currentActionPoints > maxActionPoints)
        {
            currentActionPoints = maxActionPoints;
        }
        print("Added action points. Player now has " + currentActionPoints + " action points.");
    }
    
    public void RemoveActionPoints(int numActionPoints)
    {
        currentActionPoints -= numActionPoints;
        if (currentActionPoints < 0)
        {
            currentActionPoints = 0;
        }
        print("Removed action points. Player now has " + currentActionPoints + " action points.");
    }

    public void ActivateShield()
    {
        hasShield = true;
        print("shield activated");
    }

    private IEnumerator SmoothMove(List<GraphNode<GameObject>> nodes)
    {
        float step = moveSpeed * Time.deltaTime;
        float rotationStep = rotationSpeed * Time.deltaTime;

        foreach (GraphNode<GameObject> node in nodes)
        {
            nodePosition = node.Data.GetComponent<MBGraphNode>();
            Vector3 newWorldPosition = node.Data.transform.position + positionOffset;
            newWorldPosition = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);

            Vector3 targetDirection = (node.Data.transform.position + positionOffset) - transform.position;
            while(transform.position != newWorldPosition)
            {
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
                transform.position = Vector3.MoveTowards(transform.position, newWorldPosition, moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(newDirection);
                yield return null;
            }
        }
    }
}
