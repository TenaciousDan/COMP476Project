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
        targetDirection = new Vector3(targetDirection.x + 90, targetDirection.y, targetDirection.z);
        while (transform.position != targetPosition)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            collision.gameObject.GetComponent<AbstractPlayer>().GetHit(numActionPointsToRemove);
        }
        Destroy(gameObject);
    }
}
