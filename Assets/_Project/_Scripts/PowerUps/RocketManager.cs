using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float moveSpeed = 40;
    private float rotationSpeed = 20;
    private float numActionPointsToRemove = 2;

    [SerializeField] private GameObject explosionPrefab;

    public void FireRocket(AbstractPlayer player, GameObject target)
    {
        if (player is Game.AI.AIPlayer)
        {
            ((Game.AI.AIPlayer)player).EnqueueAction(MoveRocket(player, target));
        }
        else
        {
            StartCoroutine(MoveRocket(player, target));
        }
    }

    private IEnumerator MoveRocket(AbstractPlayer player, GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y + 1, targetPosition.z);

        Vector3 targetDirection = (target.transform.position) - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = toRotation;
        while (this != null && transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        player.State = AbstractPlayer.EPlayerState.Waiting;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("there's a collision");
        if(collision.collider.tag == "HumanPlayer" || collision.collider.tag == "AIPlayer")
        {
            collision.gameObject.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove);
            GameplayManager.Instance.Players[GameplayManager.Instance.currentPlayer].State = AbstractPlayer.EPlayerState.Waiting;
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if(!collision.collider.gameObject != gameObject)
            PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);
    }
}
