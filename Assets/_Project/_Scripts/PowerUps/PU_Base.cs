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

    public abstract void OnPowerUpGet(PlayerController player);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPowerUpGet(other.GetComponent<PlayerController>());
        }
    }
}
