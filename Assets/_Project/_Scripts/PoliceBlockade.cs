using UnityEngine;

using Photon.Pun;

using System.Collections.Generic;

public class PoliceBlockade : MonoBehaviourPun
{
    [SerializeField] private List<Animator> copCarAnimators = new List<Animator>();

    private void OnTriggerStay(Collider other)
    {
        AbstractPlayer player = other.GetComponent<AbstractPlayer>();
        if (player != null)
        {
            if (GameplayManager.Instance.Players[GameplayManager.Instance.currentPlayer] == player)
            {
                photonView.RPC("LetMePass", RpcTarget.All, true);
            }
            else
            {
                photonView.RPC("LetMePass", RpcTarget.All, false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        photonView.RPC("LetMePass", RpcTarget.All, false);
    }

    [PunRPC]
    private void LetMePass(bool pass)
    {
        foreach (Animator animator in copCarAnimators)
        {
            animator.SetBool("CanLeave", pass);
        }
    }
}
