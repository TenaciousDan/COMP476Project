using Tenacious.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tenacious.Collections;
using System.Linq;

namespace Game.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        public TextMeshProUGUI checkpointsLeft;
        public List<Image> itemImages;
        public ProgressBar actionPoints;
        public HumanPlayer player;

        [SerializeField] private List<Button> btns;

        private bool moveBtnClicked = false;

        private void Start()
        {
            //player = GetComponent<HumanPlayer>();
        }

        private void Update()
        {
            actionPoints.current = player.CurrentActionPoints;

            ToggleBtns(player.Phase == AbstractPlayer.EPlayerPhase.Main);

            // Can only move if the button is clicked.
            if (moveBtnClicked)
            {
                CheckMove();
            }
        }

        private void ToggleBtns(bool toggle)
        {
            foreach (var btn in btns)
            {
                btn.interactable = toggle;
            }
        }

        /// <summary>
        /// Check whether the player can move in the selected tiles.
        /// </summary>
        private void CheckMove()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

                foreach (var hit in hits)
                {
                    if (hit.transform.name.Equals("GridSquare(Clone)"))
                    {
                        MoveBtnClick(); // Reset the button
                        var node = hit.transform.parent.GetComponent<MBGraphNode>();
                        var graphNodes = node.mbGraph.graph.Nodes().Where(x => x.Id == node.nodeId);
                        player.Move(graphNodes.ToList());
                    }
                }
            }
        }

        public void MoveBtnClick()
        {
            var currentNode = player.PositionNode;
            var neighbors = currentNode.mbGraph.graph.Neighbors(currentNode.nodeId);
            
            foreach (var node in neighbors)
            {
                node.Data.transform.GetChild(0).gameObject.SetActive(!moveBtnClicked);
            }

            moveBtnClicked = !moveBtnClicked;
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
            player.Phase = AbstractPlayer.EPlayerPhase.End; // GameplayManager ends turn instead?
            // TODO - End current turn.
        }
    }
}
