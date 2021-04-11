using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Boost : PU_Base
{
    private int numExtraActionPoints = 2;

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        player.AddActionPoints(numExtraActionPoints, true);
    }
}
