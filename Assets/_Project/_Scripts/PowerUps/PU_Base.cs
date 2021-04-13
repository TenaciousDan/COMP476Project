using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PU_Base : MonoBehaviour
{
    [SerializeField]
    public Sprite inventoryImage;

    [SerializeField]
    private Scriptable_Base powerUpScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            powerUpScript.OnPowerUpGet(other.gameObject.GetComponent<AbstractPlayer>());
            Destroy(gameObject);
        }
    }
}
