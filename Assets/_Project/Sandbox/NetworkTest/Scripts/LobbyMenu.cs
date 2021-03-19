using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyMenu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    [SerializeField] private GameObject joinOrCreateScreen;
    [SerializeField] private GameObject lobbyScreen;

    [Header("Join Or Create Screen")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    [Header("Lobby Screen")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private Button startGameButton;


    // Start is called before the first frame update
    void Start()
    {
        joinOrCreateScreen.SetActive(true);
        lobbyScreen.SetActive(false);
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        // Deactivate all screens
        joinOrCreateScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        // Enable requested screen
        screen.SetActive(true);
    }

    // Called when create room button is pressed
    public void CreateRoomBtnClick(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.CreateRoom(roomNameInput.text);
    }

    // Called when join room button is pressed
    public void JoinRoomBtnClick(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        // Change screen
        SetScreen(lobbyScreen);

        // Tell everyone to update the lobby because a new player has joined.
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // We don't need RPC because OnJoinedRoom is called for the client who just join the room
        // OnPlayerLeftRoom gets called for all clients in the room
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = string.Empty;

        // Display all the players currrently in the lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"{player.NickName}\n";
        }

        // Only the host can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    public void LeaveLobbyBtnClick()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(joinOrCreateScreen);
    }

    public void StartGameBtnClick()
    {
        // TODO
        // Change the name of the scene to load.
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "TestGame");
    }
}
