using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilSpillManager : MonoBehaviourPunCallbacks
{
    private float numActionPointsToRemove = 2;
    public string parent = null;
    public bool parentSet = false;

    private void Update()
    {
        if (parent != null && parentSet)
        {
            photonView.RPC("InstantiateOilSpill", RpcTarget.AllBuffered, parent);
            parentSet = false;
        }
    }
    [PunRPC]
    public void InstantiateOilSpill(string parent)
    {
        transform.parent = GameplayManager.Instance.gridGraph.graph[parent].Data.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HumanPlayer") || other.CompareTag("AIPlayer"))
        {
            other.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove, true);
            
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
