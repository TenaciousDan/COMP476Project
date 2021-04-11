using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_OilSpill : PU_Base
{
    private int numActionPointsToRemove = 2;

    protected override void OnPowerUpGet(AbstractPlayer player)
    {
        if (isActive)
        {
            isActive = false;
            player.RemoveActionPoints(numActionPointsToRemove);
        }
    }

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        throw new System.NotImplementedException();
    }
}
