using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Tenacious.Scenes;

public class LobbyMenu : MonoBehaviourPunCallbacks
{
    public SceneReference gameplayScene;

    [Header("Screens")] 
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject joinOrCreateScreen;
    [SerializeField] private GameObject lobbyScreen;

    [Header("Join Or Create Screen")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;

    [Header("Lobby Screen")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private TextMeshProUGUI roomListText;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button startGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList;

    // Start is called before the first frame update
    private void Start()
    {
        loginScreen.SetActive(true);
        joinOrCreateScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        cachedRoomList = new Dictionary<string, RoomInfo>();
        UpdateRoomListView();
    }

    #region PUN CALLBACKS
    
    public override void OnConnectedToMaster()
    {
        SetScreen(joinOrCreateScreen);
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;

        RefreshBtnClick();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }
    
    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }
    
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnJoinedRoom()
    {
        cachedRoomList.Clear();
        
        NetworkManager.Instance.aiPlayerCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["aiPlayerCount"];

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
    
    #endregion
    
    #region UI CALLBACKS
    
    // Called when logging into the lobby
    public void LoginBtnClick()
    {
        var playerName = playerNameInputField.text;

        if (!playerName.Equals(string.Empty))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
    
    // Called when create room button is pressed
    public void CreateRoomBtnClick(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.CreateRoom(roomNameInput.text, NetworkManager.Instance.aiPlayerCount);
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

    public void LeaveLobbyBtnClick()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(joinOrCreateScreen);
    }

    public void StartGameBtnClick()
    {
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, gameplayScene.ScenePath);
    }

    public void RefreshBtnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.GetCustomRoomList(PhotonNetwork.CurrentLobby, null);
        }
    }
    
    #endregion

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }
    
    private void UpdateRoomListView()
    {
        roomListText.text = cachedRoomList.Count == 0 ? $"No open rooms." : string.Empty;
        
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            roomListText.text += $"{info.Name}\n";
        }
    }
    
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = string.Empty;

        // Display all the players currently in the lobby
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.IsMasterClient ? $"[Host] {player.NickName}\n" : $"{player.NickName}\n";
        }
        
        // Display all AI players if any
        foreach (int ai in Enumerable.Range(1, NetworkManager.Instance.aiPlayerCount))
        {
            playerListText.text += $"AI Player {ai}\n";
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

    void SetScreen(GameObject screen)
    {
        // Deactivate all screens
        loginScreen.SetActive(false);
        joinOrCreateScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        // Enable requested screen
        screen.SetActive(true);
    }

    public void HandleInputData(int value)
    {
        NetworkManager.Instance.aiPlayerCount = value;
    }
}