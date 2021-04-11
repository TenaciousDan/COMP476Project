using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SharedHUD : MonoBehaviour
    {
        [SerializeField] private List<PlayerUIContainer> playerContainers;

        public List<AbstractPlayer> players;

        /// <summary>
        /// Call to initialize the shared player UI components.
        /// </summary>
        [PunRPC]
        private void Initialize()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerContainers[i].gameObj.SetActive(true);
                // TODO - Configure the elements of the player's UI.
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
        public GameObject gameObj;
        public Image turnPointerImage;
        public TextMeshProUGUI playerName;
        public List<Image> itemImages;
        public TextMeshProUGUI checkpoints;
    }
}

