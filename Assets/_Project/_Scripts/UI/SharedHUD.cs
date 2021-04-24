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
            UpdateSharedHud();
        }

        /// <summary>
        /// Update whose turn it is and and their relevant information.
        /// </summary>
        private void UpdateSharedHud()
        {
            if (GameplayManager.Instance.hasInitializedPlayers)
            {
                for (int i = 0; i < GameplayManager.Instance.Players.Count; i++)
                {
                    var currentPlayer = GameplayManager.Instance.Players[i];
                    playerContainers[i].turnPointerObj.SetActive(i == GameplayManager.Instance.currentPlayer);
                    playerContainers[i].playerName.text = currentPlayer.Name;
                    UpdatePlayerItems(currentPlayer, i);
                    UpdatePlayerCheckpoints(currentPlayer, i);
                }
            }
        }

        private void UpdatePlayerItems(AbstractPlayer player, int playerIndex)
        {
            for (int i = 0; i < 3; i++)
            {
                bool hasItem = player.Inventory.items[i]?.inventoryImage != null;
                var hudItemImage = playerContainers[playerIndex].itemImages[i];
                hudItemImage.enabled = hasItem;
                hudItemImage.sprite = player.Inventory.items[i]?.inventoryImage;
            }
        }

        private void UpdatePlayerCheckpoints(AbstractPlayer player, int playerIndex)
        {
            playerContainers[playerIndex].checkpoints.text = $"x{player.checkpoints.Count}";
        }

        [PunRPC]
        public void Initialize()
        {
            for (int i = 0; i < GameplayManager.Instance.Players.Count; i++)
            {
                playerContainers[i].playerContainer.SetActive(true);
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

