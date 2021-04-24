using Tenacious.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tenacious.Collections;
using System.Linq;
using System;
using Photon.Pun;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        public TextMeshProUGUI checkpointsLeft;
        public List<Image> itemImages;
        public ProgressBar actionPoints;
        public HumanPlayer player;

        [SerializeField] private Sprite cpPointerSprite;
        [SerializeField] private Sprite cpInRangeSprite;
        [SerializeField] private SDictionary<string, RectTransform> cpPointerTransforms;

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
            ShowCheckpointMarkers();
            RepositionCheckpointPointers();

            ToggleBtns(player.State != AbstractPlayer.EPlayerState.Busy);

            ToggleMoveBtn(player.CurrentActionPoints != 0);
            UpdateItems(player.Phase == AbstractPlayer.EPlayerPhase.Main);

            // Can only move if the button is clicked.
            if (moveBtnClicked)
            {
                var pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                var eventRaycastResult = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, eventRaycastResult);
                //RaycastResult result = null;

                foreach (var result in eventRaycastResult)
                {
                    if (result.gameObject.name.Equals("Button"))
                    {
                        //MoveBtnClick();
                        return;
                    }
                }

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

            if (GameplayManager.Instance.gameIsOver)
            {
                ToggleBtns(false);
            }
        }

        private void ShowCheckpointMarkers()
        {
            foreach (string cpName in cpPointerTransforms.Keys)
            {
                bool cpActive = false;
                for (int i = 0; i < player.checkpoints.Count; ++i)
                {
                    Checkpoint cp = player.checkpoints[i];
                    if (cp.checkpointName.Equals(cpName))
                    {
                        cpActive = true;
                        break;
                    }
                }
                cpPointerTransforms[cpName].parent.GetComponentInChildren<TextMeshProUGUI>().text = cpName;
                cpPointerTransforms[cpName].parent.gameObject.SetActive(cpActive);
            }
        }

        private void RepositionCheckpointPointers()
        {
            Camera camera = GameplayManager.Instance.cameraRig.rigCamera;

            for (int i = 0; i < player.checkpoints.Count; ++i)
            {
                Checkpoint cp = player.checkpoints[i];
                RectTransform pointer = cpPointerTransforms[cp.checkpointName];

                Vector3 toPosition = cp.transform.position;
                toPosition.y = 0;
                Vector3 fromPosition = GameplayManager.Instance.cameraRig.transform.position;
                fromPosition.y = 0;
                float dist = (toPosition - fromPosition).magnitude;

                if (dist <= 25)
                {
                    pointer.GetComponent<Image>().sprite = cpInRangeSprite;
                    pointer.localEulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    pointer.GetComponent<Image>().sprite = cpPointerSprite;

                    Vector3 dir = (toPosition - fromPosition).normalized;

                    float angle = -Vector3.SignedAngle(GameplayManager.Instance.cameraRig.transform.forward, dir, Vector3.up);
                    pointer.localEulerAngles = new Vector3(0, 0, angle);
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
                            player.State = AbstractPlayer.EPlayerState.Busy;
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

            for (int i = 0; i < startNode.transform.childCount; ++i)
            {
                if (startNode.transform.GetChild(i).tag.Equals("GridSquare"))
                    startNode.transform.GetChild(i).gameObject.SetActive(show);
            }    

            var nodeNeighbors = startNode.mbGraph.graph.Neighbors(startNode.nodeId);
            foreach (var node in nodeNeighbors)
            {
                int extraValue = (node.Data.transform.GetComponentInChildren<OilSpillManager>()) != null ? 2 : 0;

                var mbNode = node.Data.GetComponent<MBGraphNode>();

                bool doIt = true;
                if (GameplayManager.Instance.blockedOffNodes.Contains(mbNode) && player.checkpoints.Count > 1)
                    doIt = false;

                if (node.Data.transform.position.x > startNode.transform.position.x && direction == Direction.Right)
                {
                    if (doIt)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Right);
                }
                if (node.Data.transform.position.x < startNode.transform.position.x && direction == Direction.Left)
                {
                    if (doIt)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Left);
                }
                if (node.Data.transform.position.z > startNode.transform.position.z && direction == Direction.Down)
                {
                    if (doIt)
                        ShowMovementTiles(show, mbNode, times - 1 - extraValue, Direction.Down);
                }
                if (node.Data.transform.position.z < startNode.transform.position.z && direction == Direction.Up)
                {
                    if (doIt)
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
                    for (int i = 0; i < player.PositionNode.transform.childCount; ++i)
                    {
                        if (player.PositionNode.transform.GetChild(i).tag.Equals("GridSquare"))
                            player.PositionNode.transform.GetChild(i).gameObject.SetActive(show);
                    }
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
                    if ((hit.transform.tag.Equals("AIPlayer") || hit.transform.tag.Equals("HumanPlayer")) && hit.transform.gameObject != player.gameObject)
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
                for (int i = 0; i < neighbor.Data.transform.childCount; ++i)
                {
                    if (neighbor.Data.transform.GetChild(i).tag.Equals("GridSquare"))
                        neighbor.Data.transform.GetChild(i).gameObject.SetActive(show);
                }
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
            if (moveBtnClicked)
            {
                MoveBtnClick();
            }

            GameplayManager.Instance.photonView.RPC("UpdateCurrentPlayer", RpcTarget.All);
            player.Phase = AbstractPlayer.EPlayerPhase.End; // GameplayManager ends turn instead?
        }
    }
}