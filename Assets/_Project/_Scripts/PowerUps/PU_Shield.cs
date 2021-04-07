using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Shield : PU_Base
{
    public override void OnPowerUpGet(PlayerController player)
    {
        if (isActive)
        {
            isActive = false;
            player.ActivateShield();
        }
    }
}
