using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Random : PU_Base
{
    private int randomIndex;

    protected override void OnPowerUpGet(GameObject player)
    {
        if (isActive)
        {
            isActive = false;
            int maxIndexRange = transform.childCount;
            randomIndex = Random.Range(0, maxIndexRange);
            player.GetComponent<Inventory>().AddItem(transform.GetChild(randomIndex).GetComponent<PU_Base>());
        }
    }

    public override void OnPowerUpUse(GameObject player)
    {
        transform.GetChild(randomIndex).GetComponent<PU_Base>().OnPowerUpUse(player);
    }
}
