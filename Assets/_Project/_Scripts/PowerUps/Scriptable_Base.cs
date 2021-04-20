using System;
using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;

public class Scriptable_Base : ScriptableObject
{
    public MBGraphNode PositionNode
    {
        get; set;
    }

    public string powerUpName;

    [SerializeField]
    public Sprite inventoryImage;

    public virtual void OnPowerUpGet(AbstractPlayer player)
    {
        player.Inventory.AddItem(this);
    }

    public virtual void OnPowerUpUse(AbstractPlayer player, GameObject target = null)
    {
        throw new NotImplementedException("This power up does not have an OnPowerUpUse implemented.");
    }
}
