using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_OilSpill : PU_Base
{
    private int numActionPointsToRemove = 2;

    public override void OnPowerUpGet(PlayerController player)
    {
        if (isActive)
        {
            isActive = false;
            player.RemoveActionPoints(numActionPointsToRemove);
        }
    }
}
