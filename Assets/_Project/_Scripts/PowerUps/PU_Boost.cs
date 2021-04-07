using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Boost : PU_Base
{
    private int numExtraActionPoints = 2;

    protected override void OnPowerUpGet(GameObject player)
    {
        if (isActive)
        {
            isActive = false;
            player.GetComponent<PlayerController>().AddActionPoints(numExtraActionPoints, true);
        }
    }

    protected override void OnPowerUpUse(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
