using UnityEngine;

using Photon.Pun;

public class WelcomeScene : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        if (NetworkManager.IsInitialized)
        {
            PhotonNetwork.Disconnect();

            Destroy(NetworkManager.Instance.gameObject);
        }
    }
}
