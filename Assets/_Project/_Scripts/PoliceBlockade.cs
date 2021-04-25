using UnityEngine;

using Photon.Pun;

using System.Collections.Generic;

public class PoliceBlockade : MonoBehaviourPun
{
    [SerializeField] private Animator copCarAnimator;

    private void OnTriggerStay(Collider other)
    {
        AbstractPlayer player = other.GetComponent<AbstractPlayer>();
        if (player != null)
        {
            if (GameplayManager.Instance.Players[GameplayManager.Instance.currentPlayer] == player && player.checkpoints.Count <= 1)
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
        copCarAnimator.SetBool("CanLeave", pass);
    }
}
