using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int maxActionPoints;
    public int currentActionPoints
    {
        get { return currentActionPoints; }
        private set { currentActionPoints = value; }
    }

    // incomplete, position will not be an int. should probably rename to something else so it's not confused with transform.position
    public int position
    {
        get { return position; }
        private set { position = value; }
    }

    // offset so multiple players can be on the same tile
    public Vector3 positionOffset
    {
        get { return positionOffset; }
        private set { positionOffset = value; }
    }

    // incomplete, parameter will not be an int
    public void Move(int newPosition)
    {
        position = newPosition;
        // need to get the world position of the new node
        Vector3 newWorldPosition = Vector3.zero;
        transform.position = newWorldPosition + positionOffset;
    }

    // assign values to member variables (probably called after players choose their vehicle)
    public void InitializePlayer(int _maxActionPoints, Vector3 _positionOffset)
    {
        maxActionPoints = currentActionPoints = _maxActionPoints;
        positionOffset = _positionOffset;
    }

    public void SpendActionPoints(int numActionPoints)
    {
        if(numActionPoints > currentActionPoints)
        {
            Debug.LogWarning("Not enough action points for this action.");
        }
        else
        {
            currentActionPoints -= numActionPoints;
        }
    }

    // probably called at the start of a turn
    public void FillActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    // in case we want to add action points
    public void AddActionPoints(int numActionPoints, bool exceedLimit = false)
    {
        currentActionPoints += numActionPoints;
        if (!exceedLimit && currentActionPoints > maxActionPoints)
        {
            currentActionPoints = maxActionPoints;
        }
    }
}
