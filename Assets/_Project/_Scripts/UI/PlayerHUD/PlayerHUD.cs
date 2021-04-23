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
        [SerializeField] private float distanceBetweenNodes = 10.0f;

        private bool moveBtnClicked = false;
        private bool rocketBtnClicked = false;
        private bool oilSpillBtnClicked = false;

        private enum Direction
        {
            Left,
            Right,
            Up,
            Down,
        }

        private void Update()
        {
            UpdateAP();
            UpdateItems(player.Phase == AbstractPlayer.EPlayerPhase.Main);
            ToggleMoveBtn(player.CurrentActionPoints != 0);

            // Can only move if the button is clicked.
            if (moveBtnClicked)
            {
                ClickMove();
            }
            else if (rocketBtnClicked)
            {
                var target = ClickSelectRocketTarget();
                if (target != null)
                {
                    SpecialItemClick(target, "Rocket");
                    rocketBtnClicked = false;
                }
            }
            else if (oilSpillBtnClicked)
            {
                var target = ClickSelectOilTarget();
                if (target != null)
                {
                    SpecialItemClick(target, "Oil Spill");
                    oilSpillBtnClicked = false;
                }
            }
        }

        private void UpdateAP()
        {
            actionPoints.current = player.CurrentActionPoints;
            actionPoints.maximum = player.MaxActionPoints;
        }

        private void SpecialItemClick(GameObject target, string itemName)
        {
            int itemIndex = player.Inventory.GetItemIndex(itemName);
            ItemBtnClick(itemIndex); // Reset button
            player.Inventory.UseItem(itemIndex, target);
        }

        /// <summary>
        /// Update item sprites and button interactibility for items.
        /// </summary>
        private void UpdateItems(bool isTurn)
        {
            for (int i = 0; i < 3; i++)
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
                            player.CostPerMovement = Vector3.Distance(player.PositionNode.transform.position, graphNodes.First().Data.transform.position) / distanceBetweenNodes; // Really dumb hack but it works...
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
                int extraValue = (node.Data.transform.GetComponentInChildren<OilSpillManager>()) != null ? 2 : 0;

                var mbNode = node.Data.GetComponent<MBGraphNode>();

                if (node.Data.transform.position.x > startNode.transform.position.x && direction == Direction.Right)
                {
                    if (GameplayManager.Instance.blockedOffNodes.Contains(mbNode) /*&& player.checkpoints <= 1*/)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Right);
                }
                if (node.Data.transform.position.x < startNode.transform.position.x && direction == Direction.Left)
                {
                    if (GameplayManager.Instance.blockedOffNodes.Contains(mbNode) /*&& player.checkpoints <= 1*/)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Left);
                }
                if (node.Data.transform.position.z > startNode.transform.position.z && direction == Direction.Down)
                {
                    if (GameplayManager.Instance.blockedOffNodes.Contains(mbNode) /*&& player.checkpoints <= 1*/)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Down);
                }
                if (node.Data.transform.position.z < startNode.transform.position.z && direction == Direction.Up)
                {
                    if (GameplayManager.Instance.blockedOffNodes.Contains(mbNode) /*&& player.checkpoints <= 1*/)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Up);
                }
            }
        }

        public void ItemBtnClick(int itemIndex)
        {
            var item = player.Inventory.GetItemFromIndex(itemIndex);

            switch (item.powerUpName)
            {
                case "Rocket":
                    rocketBtnClicked = !rocketBtnClicked;
                    ShowPlayerNodes(rocketBtnClicked);
                    break;
                case "Oil Spill":
                    oilSpillBtnClicked = !oilSpillBtnClicked;
                    ShowOilSpillTiles(oilSpillBtnClicked);
                    break;
                default:
                    player.Inventory.UseItem(itemIndex);
                    break;
            }
        }

        private void ShowPlayerNodes(bool show)
        {
            foreach (var player in GameplayManager.Instance.Players)
            {
                if(player != this.player)
                {
                    player.PositionNode.transform.GetChild(0).gameObject.SetActive(show);
                }
            }
        }

        private GameObject ClickSelectRocketTarget()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

                foreach (var hit in hits)
                {
                    if (hit.transform.tag.Equals("Player") && hit.transform.gameObject != player.gameObject)
                    {
                        return hit.transform.gameObject;
                    }
                }
            }

            return null;
        }

        private void ShowOilSpillTiles(bool show)
        {
            var neighbors = player.PositionNode.mbGraph.graph.Neighbors(player.PositionNode.nodeId);

            foreach (var neighbor in neighbors)
            {
                neighbor.Data.transform.GetChild(0).gameObject.SetActive(show);
            }
        }

        private GameObject ClickSelectOilTarget()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));

                foreach (var hit in hits)
                {
                    if (hit.transform.name.Equals("GridSquare(Clone)") && hit.transform != player.PositionNode.transform)
                    {
                        return hit.transform.parent.gameObject;
                    }
                }
            }

            return null;
        }

        public void EndTurnBtnClick()
        {
            player.Phase = AbstractPlayer.EPlayerPhase.End; // GameplayManager ends turn instead?
            //player.photonView.RPC("EndTurn", Photon.Pun.RpcTarget.All);
        }
    }
}
