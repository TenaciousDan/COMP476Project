using System;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{

    // Open Connection to PUN On Start
    void Start()
    {
        print("Connecting to Server...");
        PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
        PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Called When Connected to Server
    public override void OnConnectedToMaster()
    {
        print("Connected to Server.");
        print(PhotonNetwork.LocalPlayer.NickName);

        PhotonNetwork.JoinLobby();
    }

    // Called When Disconnected from Server
    public override void OnDisconnected(DisconnectCause cause)
    {
        Console.WriteLine($"Disconnected from Server. Cause: {cause}");
    }
}
