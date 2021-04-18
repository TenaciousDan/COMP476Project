using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public const int maxInventorySize = 3;

    [SerializeField] private AbstractPlayer player;

    public List<Scriptable_Base> items
    {
        get; private set;
    }

    private void Awake()
    {
        player = GetComponent<AbstractPlayer>();
    }

    private void Start()
    {
        items = new List<Scriptable_Base>();
        for (int i = 0; i < maxInventorySize; i++)
        {
            items.Add(null);
        }
    }
    // TODO remove this hard coded way of selecting items once we decide how we want to select an item
    private int tempIndex = 0;
    private void Update()
    {
		/*
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseItem(tempIndex);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tempIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            tempIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            tempIndex = 2;
        }
		*/
    }

    public void AddItem(Scriptable_Base newItem)
    {
        int availableSlotIndex = items.IndexOf(null);
        // if there is no more room in the inventory, remove the first item
        if (InventoryIsFull())
        {
            availableSlotIndex = maxInventorySize - 1;
            ShiftItems(newItem);
        }
        items[availableSlotIndex] = newItem;
    }

    public void RemoveItem(int itemIndex)
    {
        items[itemIndex] = null;
    }

    private void ShiftItems(Scriptable_Base newItem)
    {
        for(int i = 0; i < items.Count; i++)
        {
            // shift items to the left unless it is the last index, in which case place the newItem there
            if(i + 1 != items.Count)
            {
                items[i] = items[i + 1];
            }
            else
            {
                items[i] = newItem;
            }
        }
    }

    public void UseItem(int itemIndex)
    {
        if(items[itemIndex] != null)
        {
            print($"Used {items[itemIndex].name}");
            items[itemIndex].OnPowerUpUse(player);
            RemoveItem(itemIndex);
        }
    }

    private bool InventoryIsFull()
    {
        foreach(Scriptable_Base item in items)
        {
            if(item == null)
            {
                return false;
            }
        }
        return true;
    }
}
