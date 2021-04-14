using UnityEngine;

using System.Collections.Generic;

using Photon.Pun;

public class NetworkPrefabPool : MonoBehaviour, IPunPrefabPool
{
    [SerializeField] private List<PhotonView> prefabs;
    private Dictionary<string, GameObject> poolDict;

    private void Awake()
    {
        poolDict = new Dictionary<string, GameObject>();
        foreach (PhotonView p in prefabs)
            poolDict.Add(p.name, p.gameObject);
    }

    public void Destroy(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {

        if (!poolDict.ContainsKey(prefabId))
        {
            Debug.LogError("Missing prefab '" + prefabId + "' in NetworkPrefabPool.");
            return null;
        }
        else
        {
            GameObject res = poolDict[prefabId];

            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);

            GameObject instance = GameObject.Instantiate(res, position, rotation) as GameObject;

            if (wasActive) res.SetActive(true);

            return instance;
        }
    }
}