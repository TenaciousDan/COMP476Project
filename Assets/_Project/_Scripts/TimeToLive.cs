using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    public float timeToLive = 5;
    private float ttlTimer;

    private void Awake()
    {
        ttlTimer = timeToLive;
    }

    private void Update()
    {
        if (ttlTimer <= 0)
            Destroy(gameObject);
        else
            ttlTimer -= Time.deltaTime;
    }
}
