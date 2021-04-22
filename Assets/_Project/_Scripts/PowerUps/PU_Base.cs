using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PU_Base : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Scriptable_Base powerUpScript;

    [SerializeField]
    private MBGraphNode positionNode;

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
    private void SelectRandomItem(int index)
    {
        var randomScript = (PU_Random)powerUpScript;
        randomScript.randomIndex = index;
    }

    private void Start()
    {
        powerUpScript.PositionNode = positionNode;
        transform.position = new Vector3(positionNode.transform.position.x, transform.position.y, positionNode.transform.position.z);

        if (powerUpScript is PU_Random && PhotonNetwork.IsMasterClient)
        {
            var randomScript = (PU_Random)powerUpScript;
            int index = Random.Range(0, randomScript.powerUps.Count);
            photonView.RPC("SelectRandomItem", RpcTarget.All, index);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            powerUpScript.OnPowerUpGet(other.gameObject.GetComponent<AbstractPlayer>());
            Destroy(gameObject);
        }
    }
}
