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
    private int gasTankSize;
    public int gasAvailable
    {
        get { return gasAvailable; }
        private set { gasAvailable = value; }
    }

    // incomplete, position will not be an int
    public int position
    {
        get { return position; }
        private set { position = value; }
    }

    // incomplete, parameter will not be an int
    public void Move(int newPosition)
    {

    }

    // assign values to member variables (probably called after players choose their vehicle)
    public void InitializePlayer(int _maxActionPoints, int _gasTankSize)
    {
        maxActionPoints = currentActionPoints = _maxActionPoints;
        gasTankSize = gasAvailable = _gasTankSize;
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
    public void ResetActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    public void FillGasTank()
    {
        gasAvailable = gasTankSize;
    }
}
