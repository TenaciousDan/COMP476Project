using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpillManager : MonoBehaviour
{
    private float numActionPointsToRemove = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove);
            //Destroy(gameObject);
        }
    }
}
