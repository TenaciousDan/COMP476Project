using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketManager : MonoBehaviour
{
    private float moveSpeed = 20;
    private float rotationSpeed = 20;
    private float numActionPointsToRemove = 2;

    public void FireRocket(GameObject target)
    {
        StartCoroutine(MoveRocket(target));
    }

    private IEnumerator MoveRocket(GameObject target)
    {
        Vector3 targetPosition = target.transform.position;
        targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);

        Vector3 targetDirection = (target.transform.position) - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = toRotation;
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name);
        if(collision.collider.tag == "HumanPlayer" || collision.collider.tag == "AIPlayer")
        {
            print("Hit player!");
            collision.gameObject.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
