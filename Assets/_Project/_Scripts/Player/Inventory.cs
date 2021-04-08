using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private int maxInventorySize;
    [SerializeField]
    private GameObject _inventorySlots;
    private List<GameObject> inventorySlots;
    private int selectedIndex;
    
    public List<PU_Base> items
    {
        get; private set;
    }

    private void Start()
    {
        items = new List<PU_Base>();
        inventorySlots = new List<GameObject>();
        int children = _inventorySlots.transform.childCount;
        for (int i = 0; i < children; i++)
        {
            inventorySlots.Add(_inventorySlots.transform.GetChild(i).gameObject);
        }
        maxInventorySize = inventorySlots.Count;
        selectedIndex = 0;
    }

    public void AddItem(PU_Base newItem)
    {
        // if there is no more room in the inventory, remove the first item
        if(items.Count == maxInventorySize)
        {
            items.RemoveAt(0);
            ShiftSlotImages(newItem);
        }
        items.Add(newItem);
        AddSlotImage(newItem, items.Count - 1);
    }

    public void RemoveItem(PU_Base itemToRemove)
    {
        if (itemToRemove != null && items.Count > 0)
        {
            int index = items.IndexOf(itemToRemove);
            items.RemoveAt(index);
            RemoveSlotImage(index);
        }
        else
        {
            Debug.LogWarning("Item to remove is null or inventory is empty.");
        }
    }

    private void AddSlotImage(PU_Base newItem, int index)
    {
        if(newItem.inventoryImage != null)
        {
            inventorySlots[index].GetComponent<Image>().sprite = newItem.inventoryImage;
        }
    }

    private void RemoveSlotImage(int index)
    {
        inventorySlots[index].GetComponent<Image>().sprite = null;
    }

    private void ShiftSlotImages(PU_Base newItem)
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            if(i + 1 != inventorySlots.Count)
            {
                inventorySlots[i].GetComponent<Image>().sprite = inventorySlots[i + 1].GetComponent<Image>().sprite;
            }
            else
            {
                inventorySlots[i].GetComponent<Image>().sprite = newItem.inventoryImage;
            }
        }
    }
}
