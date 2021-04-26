using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Game.AI;
using Photon.Pun;
using Photon.Realtime;
using Tenacious.Collections;
using Tenacious.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Room Properties
    public const int MaxRoomSize = 4;
    public int aiPlayerCount = 0;

    public Hashtable spawnIndices;

    private static NetworkManager instance;
    private static bool reinitializationPermitted = false;
    private static bool destroyed = false;
    private static object mutex = new object();

    public static bool IsInitialized { get; protected set; }

    [Tooltip("Allow this object to be re-initialized after being destroyed.")]
    [SerializeField] private bool allowReinitialization = false;
    [Tooltip("Destroy this object when a new scene is loaded.")]
    [SerializeField] private bool destroyOnLoad = false;

    [SerializeField]
    private NetworkPrefabPool networkPrefabPool;
    
    // Instance
    public static NetworkManager Instance
    {
        get
        {
            if (destroyed)
            {
                string log_message = "NetworkManager has already been destroyed";

                if (reinitializationPermitted)
                {
                    Debug.LogWarning(log_message + " - Creating new instance.");
                    destroyed = false;
                }
                else
                {
                    Debug.LogWarning(log_message + " - Returning null.");
                    return null;
                }
            }

            return CreateInstance(null);
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            if (destroyed && !allowReinitialization)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                CreateInstance(this.gameObject); 
                PhotonNetwork.PrefabPool = networkPrefabPool;
            }
        }
        else
        {
            // an instance already exists and this is a duplicate
            Destroy(this.gameObject);
            destroyed = false;
            return;
        }

        reinitializationPermitted = allowReinitialization;

        if (!DestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master server!");
    }

    #region ROOMS
    public void CreateRoom(string roomName, int aiPlayerCount)
    {
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.MaxPlayers = Convert.ToByte(MaxRoomSize - aiPlayerCount);
        newRoomOptions.CustomRoomProperties = new Hashtable {{"aiPlayerCount", aiPlayerCount}};

        PhotonNetwork.CreateRoom(roomName, newRoomOptions);
    }

    public override void OnCreatedRoom()
    {
        print($"Created room: {PhotonNetwork.CurrentRoom.Name}");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print(message);
    }
    #endregion

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        //SceneLoader.Instance.LoadScene(sceneName, SceneLoader.FADE_TRANSITION);
        // uncomment if the above line causes problems
        PhotonNetwork.LoadLevel(sceneName);
    }

    #region SINGLETON
    private static NetworkManager CreateInstance(GameObject game_object)
    {
        lock (mutex)
        {
            // Create new instance if one doesn't already exist
            if (instance == null)
            {
                if (game_object == null)
                {
                    // create the GameObject that will house the singleton instance
                    game_object = new GameObject("NetworkManager");
                    game_object.AddComponent<NetworkManager>();
                }

                instance = game_object.GetComponent<NetworkManager>();
            }

            IsInitialized = true;

            return instance;
        }
    }
    public bool DestroyOnLoad
    {
        get { return destroyOnLoad; }
        set
        {
            destroyOnLoad = value;
            if (destroyOnLoad)
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
            else
                DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        // When Unity quits it destroys objects in a random order and this can create issues for singletons. 
        // So we prevent reinitialization and access to the instance when the application quits to prevent problems.
        reinitializationPermitted = false;
        destroyed = true; // pretend its already destroyed
    }

    private void OnDestroy()
    {
        instance = null;
        destroyed = true;

        IsInitialized = false;
    }
    #endregion
}