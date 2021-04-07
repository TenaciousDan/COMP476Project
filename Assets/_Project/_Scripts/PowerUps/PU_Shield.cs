using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Shield : PU_Base
{
    protected override void OnPowerUpUse(GameObject player)
    {
        player.GetComponent<PlayerController>().ActivateShield();
        player.GetComponent<Inventory>().RemoveItem(this);
    }
}
