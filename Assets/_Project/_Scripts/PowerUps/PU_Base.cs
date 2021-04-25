using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PU_Base : MonoBehaviourPunCallbacks
{
    public Scriptable_Base powerUpScript;
    private int randomIndex = -1;

    //private void Awake()
    //{
    //    if (powerUpScript is PU_Random && PhotonNetwork.IsMasterClient)
    //    {
    //        var randomScript = (PU_Random)powerUpScript;
    //        int index = Random.Range(0, randomScript.powerUps.Count);
    //        photonView.RPC("SelectRandomItem", RpcTarget.All, index);
    //    }
    //}

    [PunRPC]
    private void SelectRandomItem()
    {
        var randomScript = (PU_Random)powerUpScript;
        randomIndex = Random.Range(0, randomScript.powerUps.Count);
    }

    private void Awake()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.tag == "Node")
            {
                MBGraphNode positionNode = hitCollider.gameObject.GetComponent<MBGraphNode>();
                powerUpScript.PositionNode = positionNode;
                transform.position = new Vector3(positionNode.transform.position.x, transform.position.y, positionNode.transform.position.z);
                transform.parent = positionNode.transform;
            }
        }
    }

    private void Start()
    {
        if (powerUpScript is PU_Random && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SelectRandomItem", RpcTarget.All);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HumanPlayer") || other.CompareTag("AIPlayer"))
        {
            powerUpScript.OnPowerUpGet(other.gameObject.GetComponent<AbstractPlayer>(), randomIndex);
            Destroy(gameObject);
        }
    }
}
