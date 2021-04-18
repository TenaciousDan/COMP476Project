using System.Collections;
using System.Collections.Generic;
using Tenacious.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PU_Base : MonoBehaviour
{
    [SerializeField]
    private Scriptable_Base powerUpScript;

    [SerializeField]
    private MBGraphNode positionNode;

    private void Start()
    {
        powerUpScript.PositionNode = positionNode;
        transform.position = new Vector3(positionNode.transform.position.x, transform.position.y, positionNode.transform.position.z);
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
