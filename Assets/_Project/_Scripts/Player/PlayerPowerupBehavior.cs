using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerupBehavior : MonoBehaviour
{
    [SerializeField]
    private List<IConsumable> _consumables;

    private void Start()
    {
        this._consumables = new List<IConsumable>();
        _consumables.Add(new Powerup("TestPowerup"));
    }

    private void Update()
    {
        UsePowerup();
    }

    private void UsePowerup()
    {
        foreach (IConsumable consumable in _consumables)
        {
            consumable.Use();
            consumable.SetUsed(true);
        }

        RemoveUsedPowerups();
    }

    private void RemoveUsedPowerups()
    {
        int length = this._consumables.Count;

        for (int i = 0; i < length; i++)
        {
            if (this._consumables[i].IsUsed())
            {
                this._consumables.RemoveAt(i);
            }
        }
    }

    // TODO
    private void PickPowerup()
    {
        throw new NotImplementedException();
    }
}
