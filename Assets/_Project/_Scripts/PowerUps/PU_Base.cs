using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PU_Base : MonoBehaviour
{
    [SerializeField]
    public Sprite inventoryImage;
    protected bool isActive;

    private void Start()
    {
        isActive = true;
    }

    protected virtual void OnPowerUpGet(AbstractPlayer player)
    {
        if (isActive)
        {
            isActive = false;
            player.Inventory.AddItem(this);
        }
    }

    public abstract void OnPowerUpUse(AbstractPlayer player);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPowerUpGet(other.gameObject.GetComponent<AbstractPlayer>());
        }
    }
}
