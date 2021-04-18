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

    //public virtual string PowerUpName
    //{
    //    get { return null; }
    //}

    public string powerUpName;

    [SerializeField]
    public Sprite inventoryImage;

    public virtual void OnPowerUpGet(AbstractPlayer player)
    {
        player.Inventory.AddItem(this);
    }

    public virtual void OnPowerUpUse(AbstractPlayer player)
    {
        throw new NotImplementedException("This power up does not have an OnPowerUpUse implemented.");
    }

    public virtual void OnPowerUpUse(AbstractPlayer player, AbstractPlayer target)
    {
        throw new NotImplementedException("This power up does not have an OnPowerUpUse implemented.");
    }
}
