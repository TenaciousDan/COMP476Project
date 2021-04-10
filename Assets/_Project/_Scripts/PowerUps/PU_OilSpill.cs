using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_OilSpill : PU_Base
{
    private int numActionPointsToRemove = 2;

    protected override void OnPowerUpGet(GameObject player)
    {
        if (isActive)
        {
            isActive = false;
            player.GetComponent<PlayerController>().RemoveActionPoints(numActionPointsToRemove);
        }
    }

    public override void OnPowerUpUse(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
