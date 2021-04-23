using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpillManager : MonoBehaviour
{
    private float numActionPointsToRemove = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HumanPlayer") || other.CompareTag("AIPlayer"))
        {
            other.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove);
            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
