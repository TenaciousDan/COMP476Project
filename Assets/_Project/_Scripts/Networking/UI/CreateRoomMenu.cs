using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class CreateRoomMenu : MonoBehaviourPunCallbacks
{
    // TODO: Ensure the room name is not empty before being created
    [SerializeField] private Text _roomName;

    public void OnClick_CreateRoom()
    {
        // Ensure User is connected
        if (!PhotonNetwork.IsConnected) return;
        
        // Set Room Options
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        
        // Create Rooms
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("Created room successfully");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print($"Room creation failed: {message}");
    }
}
