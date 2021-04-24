using UnityEngine;

using Tenacious.Audio;

namespace Game.Scenes
{
    public class LobbyScene : MonoBehaviour
    {
        private void Awake()
        {
            AudioManager.Instance.PlayMusic("Intro");
        }
    }
}
