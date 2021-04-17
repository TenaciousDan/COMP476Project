using Tenacious.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tenacious.Collections;
using System.Linq;
using System;

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

        private enum Direction
        {
            Left,
            Right,
            Up,
            Down,
        }

        private void Start()
        {
            //player = GetComponent<HumanPlayer>();
        }

        private void Update()
        {
            //print(player.Phase);
            UpdateAP();
            //ToggleBtns(player.Phase == AbstractPlayer.EPlayerPhase.Main || player.Phase == AbstractPlayer.EPlayerPhase.Standby);
            UpdateItems(player.Phase == AbstractPlayer.EPlayerPhase.Main);
            ToggleMoveBtn(player.CurrentActionPoints != 0);

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
        private void UpdateItems(bool isTurn)
        {
            for(int i = 0; i < 3; i++)
            {
                var btnImage = btns[i].transform.GetChild(0).GetComponent<Image>();
                bool hasItem = player.Inventory.items[i]?.inventoryImage != null;
                btns[i].interactable = hasItem && isTurn;
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
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                ShowMovementTiles(!moveBtnClicked, player.PositionNode, (int)player.CurrentActionPoints + 1, direction);
            }

            moveBtnClicked = !moveBtnClicked;
        }
        
        /// <summary>
        /// Show the available neighbors to travel to.
        /// </summary>
        /// <param name="show"></param> Whether to show the tiles or not.
        /// <param name="startNode"></param> Which node to start from.
        /// <param name="times"></param> Number of levels to find the neighbors.
        /// <param name="direction"></param> Which direction the neighbors are in.
        private void ShowMovementTiles(bool show, MBGraphNode startNode, int times, Direction direction)
        {
            if (times <= 0)
                return;

            startNode.transform.GetChild(0).gameObject.SetActive(show);
            var nodeNeighbors = startNode.mbGraph.graph.Neighbors(startNode.nodeId);
            foreach (var node in nodeNeighbors)
            {
                if (node.Data.transform.position.x > startNode.transform.position.x && direction == Direction.Right)
                {
                    ShowMovementTiles(show, node.Data.GetComponent<MBGraphNode>(), times - 1, Direction.Right);
                }
                if (node.Data.transform.position.x < startNode.transform.position.x && direction == Direction.Left)
                {
                    ShowMovementTiles(show, node.Data.GetComponent<MBGraphNode>(), times - 1, Direction.Left);
                }
                if (node.Data.transform.position.z > startNode.transform.position.z && direction == Direction.Down)
                {
                    ShowMovementTiles(show, node.Data.GetComponent<MBGraphNode>(), times - 1, Direction.Down);
                }
                if (node.Data.transform.position.z < startNode.transform.position.z && direction == Direction.Up)
                {
                    ShowMovementTiles(show, node.Data.GetComponent<MBGraphNode>(), times - 1, Direction.Up);
                }
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
