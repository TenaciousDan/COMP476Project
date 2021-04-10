using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int maxActionPoints;
    private float speed = 0.2f;
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

    public void Move(List<MBGraphNode> nodes, int cost)
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

    private IEnumerator SmoothMove(List<MBGraphNode> nodes)
    {
        float step = speed * Time.deltaTime;

        foreach (MBGraphNode node in nodes)
        {
            nodePosition = node;
            Vector3 newWorldPosition = node.transform.position + positionOffset;
            newWorldPosition = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
            while(transform.position != newWorldPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, newWorldPosition, step);
                yield return null;
            }
        }
    }
}
