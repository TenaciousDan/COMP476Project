using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public const int maxInventorySize = 3;
    public int currentItemCount = 0;

    [SerializeField] private AbstractPlayer player;

    public List<Scriptable_Base> items
    {
        get; private set;
    }

    private void Awake()
    {
        player = GetComponent<AbstractPlayer>();
    }

    public void InitializeList()
    {
        items = new List<Scriptable_Base>();
        for (int i = 0; i < maxInventorySize; i++)
        {
            items.Add(null);
        }
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
        // Increase item count if Inventory is not full
        else
        {
            currentItemCount++;
        }

        print($"{player.Name} picked up {newItem}");
        items[availableSlotIndex] = newItem;
    }

    public void RemoveItem(int itemIndex)
    {
        currentItemCount--;
        items[itemIndex] = null;
    }

    public void UseItem(int itemIndex, GameObject target = null)
    {
        if (items[itemIndex] != null)
        {
            print($"{player.Name} used {items[itemIndex].name}");
            items[itemIndex].OnPowerUpUse(player, target);
            //RemoveItem(itemIndex);
            if (PhotonNetwork.IsMasterClient)
            {
                player.photonView.RPC("RemoveItemFromInventory", RpcTarget.All, itemIndex);
            }
        }
    }

    public Scriptable_Base GetItemFromIndex(int itemIndex)
    {
        return items[itemIndex];
    }

    public int GetItemIndex(string name)
    {
        //print(name);
        int index = -1;

        for(int i = 0; i < items.Count; ++i)
        {
            if (items[i] != null && items[i].powerUpName.Equals(name))
            {
                index = i;
            }
        }
        return index;
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
