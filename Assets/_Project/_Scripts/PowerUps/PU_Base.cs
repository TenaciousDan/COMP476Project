using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PU_Base : MonoBehaviour
{
    protected bool isActive;

    private void Start()
    {
        isActive = true;
    }

    protected virtual void OnPowerUpGet(GameObject player)
    {
        if (isActive)
        {
            isActive = false;
            player.GetComponent<Inventory>().AddItem(this);
        }
    }

    protected abstract void OnPowerUpUse(GameObject player);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPowerUpGet(other.gameObject);
        }
    }
}
