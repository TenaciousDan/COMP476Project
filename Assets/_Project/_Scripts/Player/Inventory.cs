using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private int maxInventorySize;
    [SerializeField] public GameObject _inventorySlots;
    private List<GameObject> inventorySlots;
    private int selectedIndex;

    [SerializeField] private AbstractPlayer player;

    public List<PU_Base> items
    {
        get; private set;
    }

    private void Awake()
    {
        //
    }

    private void Start()
    {
        if (_inventorySlots != null)
        {
            items = new List<PU_Base>();
            inventorySlots = new List<GameObject>();
            int children = _inventorySlots.transform.childCount;
            for (int i = 0; i < children; i++)
            {
                inventorySlots.Add(_inventorySlots.transform.GetChild(i).gameObject);
                items.Add(null);
            }
            maxInventorySize = inventorySlots.Count;
            selectedIndex = 0;
        }
    }
    // TODO remove this hard coded way of selecting items once we decide how we want to select an item
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeSelectedIndex(0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeSelectedIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeSelectedIndex(2);
        }
    }

    public void AddItem(PU_Base newItem)
    {
        int availableSlotIndex = items.IndexOf(null);
        // if there is no more room in the inventory, remove the first item
        if (InventoryIsFull())
        {
            availableSlotIndex = inventorySlots.Count - 1;
            ShiftItems(newItem);
        }
        items[availableSlotIndex] = newItem;
        AddSlotImage(newItem, availableSlotIndex);
    }

    public void RemoveItem(int itemIndex)
    {
        items[itemIndex] = null;
        RemoveSlotImage(itemIndex);
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

    private void ShiftItems(PU_Base newItem)
    {
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            if(i + 1 != inventorySlots.Count)
            {
                inventorySlots[i].GetComponent<Image>().sprite = inventorySlots[i + 1].GetComponent<Image>().sprite;
                items[i] = items[i + 1];
            }
            else
            {
                inventorySlots[i].GetComponent<Image>().sprite = newItem.inventoryImage;
                items[i] = newItem;
            }
        }
    }

    private void UseItem()
    {
        if(inventorySlots[selectedIndex].GetComponent<Image>().sprite != null)
        {
            items[selectedIndex].OnPowerUpUse(player);
            RemoveItem(selectedIndex);
        }
    }

    private void ChangeSelectedIndex(int newIndex)
    {
        if(newIndex > -1 && newIndex < inventorySlots.Count)
        {
            selectedIndex = newIndex;
        }
    }

    private bool InventoryIsFull()
    {
        foreach(PU_Base item in items)
        {
            if(item == null)
            {
                return false;
            }
        }
        return true;
    }
}
