using Tenacious.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        public TextMeshProUGUI checkpointsLeft;
        public List<Image> itemImages;
        public ProgressBar actionPoints;

        public void MoveBtnClick()
        {
            print("Time to move!");
            // TODO - Call move for player
        }

        public void FirstItemBtnClick()
        {
            print("Using item1");
            // TODO - Use item1
        }

        public void SecondItemBtnClick()
        {
            print("Using item2");
            // TODO - Use item2
        }

        public void ThirdItemBtnClick()
        {
            print("Using item3");
            // TODO - Use item3
        }

        public void EndTurnBtnClick()
        {
            print("Ending turn!");
            // TODO - End current turn.
        }
    }
}
