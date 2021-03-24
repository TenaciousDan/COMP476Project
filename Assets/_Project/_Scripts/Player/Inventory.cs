using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly int maxInventorySize = 3;
    // might not be a list of GameObjects
    public List<GameObject> items
    {
        get { return items; }
    }

    public void AddItem(GameObject newItem)
    {
        // if there is no more room in the inventory, remove the first item
        if(items.Count == maxInventorySize)
        {
            items.RemoveAt(0);
        }
        items.Add(newItem);
    }

    public void RemoveItem(GameObject itemToRemove)
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
