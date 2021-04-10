using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Shield : PU_Base
{
    public override void OnPowerUpUse(GameObject player)
    {
        player.GetComponent<PlayerController>().ActivateShield();
    }
}
