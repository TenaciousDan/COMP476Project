using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scriptable_Base : ScriptableObject
{
    public virtual void OnPowerUpGet(AbstractPlayer player)
    {
        player.Inventory.AddItem(this);
    }

    public virtual void OnPowerUpUse(AbstractPlayer player)
    {
        throw new NotImplementedException("This power up does not have an OnPowerUpUse implemented.");
    }
}
