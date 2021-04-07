using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly int maxInventorySize = 3;
    
    public List<PU_Base> items
    {
        get; private set;
    }

    private void Start()
    {
        items = new List<PU_Base>();
    }

    public void AddItem(PU_Base newItem)
    {
        // if there is no more room in the inventory, remove the first item
        if(items.Count == maxInventorySize)
        {
            items.RemoveAt(0);
        }
        items.Add(newItem);
    }

    public void RemoveItem(PU_Base itemToRemove)
    {
        if (itemToRemove != null && items.Count > 0)
        {
            items.Remove(itemToRemove);
        }
        else
        {
            Debug.LogWarning("Item to remove is null or inventory is empty.");
        }
    }
}
