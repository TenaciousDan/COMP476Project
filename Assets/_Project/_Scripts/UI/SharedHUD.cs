using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SharedHUD : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<PlayerUIContainer> playerContainers;

        public List<AbstractPlayer> players;

        private void Awake()
        {
            foreach (var playerContainer in playerContainers)
            {
                playerContainer.playerContainer.SetActive(false);
            }
        }

        /// <summary>
        /// Placeholder until networking is in.
        /// </summary>
        private void Update()
        {
            
        }

        /// <summary>
        /// Call to initialize the shared player UI components.
        /// </summary>
        [PunRPC]
        public void Initialize()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerContainers[i].playerContainer.SetActive(true);
                // TODO - Configure the elements of the player's UI.
            }
        }

        public void InitializeTest()
        {
            for (int i = 0; i < GameplayManager.Instance.Players.Count; i++)
            {
                playerContainers[i].playerContainer.SetActive(true);
            }
        }

        /// <summary>
        /// Call whenever a player updates something during their turn.
        /// </summary>
        [PunRPC]
        public void UpdateUI()
        {
            foreach (var player in players)
            {
                // Update each component of the UI.
            }
        }
    }

    /// <summary>
    /// Holds the information of the shared player's UI.
    /// </summary>
    [System.Serializable]
    public class PlayerUIContainer
    {
        public GameObject playerContainer;
        public GameObject turnPointerObj;
        public TextMeshProUGUI playerName;
        public List<Image> itemImages;
        public TextMeshProUGUI checkpoints;
    }
}

