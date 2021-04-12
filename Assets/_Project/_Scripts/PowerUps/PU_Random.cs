using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_Random : PU_Base
{
    private int randomIndex;

    [SerializeField] private List<PU_Base> powerUps;

    protected override void OnPowerUpGet(AbstractPlayer player)
    {
        if (isActive)
        {
            isActive = false;
            randomIndex = Random.Range(0, powerUps.Count);
            player.Inventory.AddItem(powerUps[randomIndex]);
        }
    }

    public override void OnPowerUpUse(AbstractPlayer player)
    {
        transform.GetChild(randomIndex).GetComponent<PU_Base>().OnPowerUpUse(player);
    }
}
