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
            UpdateAP();
            ToggleBtns(player.Phase == AbstractPlayer.EPlayerPhase.Main);
            UpdateItems();

            // Can only move if the button is clicked.
            if (moveBtnClicked)
            {
                ClickMove();
            }
        }

        private void UpdateAP()
        {
            actionPoints.current = player.CurrentActionPoints;
            actionPoints.maximum = player.MaxActionPoints;
        }

        /// <summary>
        /// Update item sprites and button interactibility for items.
        /// </summary>
        private void UpdateItems()
        {
            for(int i = 0; i < 3; i++)
            {
                var btnImage = btns[i].transform.GetChild(0).GetComponent<Image>();
                bool hasItem = player.Inventory.items[i]?.inventoryImage != null;
                btns[i].interactable = hasItem;
                btnImage.enabled = hasItem;
                btnImage.sprite = player.Inventory.items[i]?.inventoryImage;
            }
        }

        private void ToggleBtns(bool toggle)
        {
            ToggleItemBtns(toggle);
            ToggleMoveBtn(toggle);
            ToggleEndTurnBtn(toggle);
        }

        private void ToggleItemBtns(bool toggle)
        {
            for(int i = 0; i < 3; i++)
            {
                btns[i].interactable = toggle;
            }
        }

        private void ToggleMoveBtn(bool toggle)
        {
            btns[3].interactable = toggle;
        }

        private void ToggleEndTurnBtn(bool toggle)
        {
            btns[4].interactable = toggle;
        }

        /// <summary>
        /// Check whether the player can move in the selected tiles.
        /// </summary>
        private void ClickMove()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

                foreach (var hit in hits)
                {
                    if (hit.transform.name.Equals("GridSquare(Clone)"))
                    {
                        var node = hit.transform.parent.GetComponent<MBGraphNode>();

                        if(node != player.PositionNode)
                        {
                            MoveBtnClick(); // Reset the button
                            var graphNodes = node.mbGraph.graph.Nodes().Where(x => x.Id == node.nodeId);
                            player.Move(graphNodes.ToList());
                        }
                    }
                }
            }
        }

        public void MoveBtnClick()
        {
            ShowMovementTiles(!moveBtnClicked);
            moveBtnClicked = !moveBtnClicked;
        }

        private void ShowMovementTiles(bool show)
        {
            var currentNode = player.PositionNode;
            var neighbors = currentNode.mbGraph.graph.Neighbors(currentNode.nodeId);

            foreach (var node in neighbors)
            {
                node.Data.transform.GetChild(0).gameObject.SetActive(show);
            }
        }

        public void ItemBtnClick(int itemIndex)
        {
            player.Inventory.UseItem(itemIndex);
        }

        public void EndTurnBtnClick()
        {
            print("Ending turn!");
            player.Phase = AbstractPlayer.EPlayerPhase.End; // GameplayManager ends turn instead?
        }
    }
}
