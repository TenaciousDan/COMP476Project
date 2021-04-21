using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tenacious.Collections;

namespace Game.AI
{
    [RequireComponent(typeof(Pathfinding))]
    public class AIPlayer : AbstractPlayer
    {
        [SerializeField] private BehaviorTree behaviorTree;

        private Pathfinding pathFinding;

        private Queue<IEnumerator> actionQueue;
        private bool isProcessingActions;

        protected override void Awake()
        {
            base.Awake();

            actionQueue = new Queue<IEnumerator>();
            pathFinding = GetComponent<Pathfinding>();
        }

        public override void MainPhaseUpdate()
        {
            Phase = EPlayerPhase.Main;

            if (!isProcessingActions)
            {
                ResetBehaviorTreeProperties();
                GetAllReachableNodes(PositionNode.nodeId, (int)CurrentActionPoints);
                behaviorTree.Evaluate();

                // Process actions
                StartCoroutine(ProcessActionsQueue());
            }
        }

        private void EnqueueAction(IEnumerator action)
        {
            actionQueue.Enqueue(action);
        }

        private IEnumerator ProcessActionsQueue()
        {
            isProcessingActions = true;

            while (true)
            {
                if (actionQueue.Count > 0)
                {
                    State = EPlayerState.Busy;
                    yield return StartCoroutine(actionQueue.Dequeue());
                    State = EPlayerState.Waiting;
                }
                else
                    break;
            }

            isProcessingActions = false;
        }

        public override void InitializePlayer(float _maxActionPoints, Vector3 _positionOffset, string _startingNodeId, string name, int playerIndex)
        {
            base.InitializePlayer(_maxActionPoints, _positionOffset, _startingNodeId, name, playerIndex);

            pathFinding.mbGraph = GameplayManager.Instance.gridGraph;
        }

        private float MovementHeuristic(Transform start, Transform end)
        {
            // Manhattan distance
            float dx = Mathf.Abs(start.position.x - end.position.x);
            float dz = Mathf.Abs(start.position.z - end.position.z);

            // TODO: implement GetCostToMove() function that returns how much it costs to move (ex: player could have a debuff that makes it cost more/less to move)
            float cost = 1;

            return cost * (dx + dz);
        }

        /**********************************
         * BehaviorTree leaf node methods and properties
         ***********************************/

        private int itemToUseIndex = -1;
        private bool shouldTakeCover;
        private AbstractPlayer playerAttackTarget;
        private List<AbstractPlayer> playerAttackThreats = new List<AbstractPlayer>();
        private List<string> reachableNodeIds = new List<string>();
        private MBGraphNode moveTargetNode;
        private GameObject nodeToSpillOn;

        /// <summary>
        /// Resets the behavior tree properties. This function is called each time before the behavior tree is evaluated. <br />
        /// See <see cref="BehaviorTree.Evaluate"/>
        /// </summary>
        private void ResetBehaviorTreeProperties()
        {
            itemToUseIndex = -1;
            shouldTakeCover = false;
            playerAttackTarget = null;
            playerAttackThreats.Clear();
            reachableNodeIds.Clear();
            moveTargetNode = null;
            nodeToSpillOn = null;
        }

        public int HasItem()
        {
            if (Inventory.items.Count > 0)
                return (int)BTNode.EState.Success;
            else
                return (int)BTNode.EState.Failure;
        }

        public int ShouldUseItem()
        {
            // TODO: foreach item in inventory,
            //       if any item would give the player any value
            //           set itemToUseIndex to be the item that offers the greatest value and return success
            //       else if none of the items give any value at this moment (ex: no targets, etc...)
            //           return failure

            int missileIndex = Inventory.GetItemIndex("Missile");
            int boostIndex = Inventory.GetItemIndex("Boost");
            int shieldIndex = Inventory.GetItemIndex("Shield");
            int oilSpillIndex = Inventory.GetItemIndex("Oil Spill");

            if (boostIndex != -1)
            {
                itemToUseIndex = boostIndex;
            }
            else if (missileIndex != -1)
            {
                // check if other players are in sight for missile target
                foreach (AbstractPlayer p in GameplayManager.Instance.Players)
                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, p.transform.position - transform.position);
                    foreach (RaycastHit hit in hits)
                    {
                        // we are colliding with ourself
                        if (hit.transform == transform) continue;

                        if (hit.collider.tag.Equals("Player"))
                        {
                            AbstractPlayer potentialTarget = hit.collider.GetComponent<AbstractPlayer>();

                            // TODO: check if potentialTarget is in equal or higher place standing. If not, then do not waste the missile on them.

                            playerAttackTarget = potentialTarget;
                            itemToUseIndex = missileIndex;
                            break;
                        }
                    }
                }
            }
            else if (shieldIndex != -1)
            {
                // check if player is in sight for foreign missiles
                foreach (AbstractPlayer p in GameplayManager.Instance.Players)
                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.position - p.transform.position);
                    foreach (RaycastHit hit in hits)
                    {
                        // the ray has hit us
                        if (hit.transform == transform)
                        {
                            itemToUseIndex = shieldIndex;
                        }
                    }
                }

                itemToUseIndex = shieldIndex;
            }
            else if (oilSpillIndex != -1)
            {
                // we can only place oil spills on neighbor nodes around us
                IWeightedDiGraph<GameObject, float> graph = PositionNode.mbGraph.graph;
                List<GameObject> potentialOilTargets = new List<GameObject>();
                foreach (GraphNode<GameObject> graphNode in graph.Neighbors(PositionNode.nodeId))
                {
                    // CalculatePathToGoal() and if this node is on that path then do not add it to the list (otherwise we'd be hindering ourselves)
                    potentialOilTargets.Add(graphNode.Data);
                }

                // If there 1 or less nodes that don't have an oil spill on them then don't place the oil spill or else we would be trapping ourselves
                if (potentialOilTargets.Count > 1)
                {
                    // select the first potential oil target
                    nodeToSpillOn = potentialOilTargets[0];
                    itemToUseIndex = oilSpillIndex;
                }
            }

            return itemToUseIndex >= 0 ? (int)BTNode.EState.Success : (int)BTNode.EState.Failure;
        }

        public int UseItem()
        {
            // TODO: powerup/item class needs to expose a method to activate the effect of the powerup
            //       it should take an AbstractPlayer as a parameter and modify that player's state accordingly

            // Inventory.items[itemToUseIndex].ActivatePowerup(this);

            return (int)BTNode.EState.Success;
        }

        public int ShouldTakeCover()
        {
            // foreach player in the game, check if they have ranged/attack items
            //       if they do, then get the item with the longest range and check if they can reach this player
            //       if they can reach us 
            //           set shouldTakeCover to true
            //           add players who are a threat to playerAttackThreats list
            //           return Success
            //       else if no one can reach us with an attack
            //           return failure

            foreach (AbstractPlayer p in GameplayManager.Instance.Players)
            {
                int pOilSpillIndex = p.Inventory.GetItemIndex("Oil Spill");
                int pMissileIndex = p.Inventory.GetItemIndex("Missile");

                if (pMissileIndex != -1)
                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.position - p.transform.position);
                    foreach (RaycastHit hit in hits)
                    {
                        // the ray has hit us
                        if (hit.transform == transform)
                        {
                            playerAttackThreats.Add(p);
                            shouldTakeCover = true;
                        }
                    }
                }
                else if (pOilSpillIndex != -1)
                {
                    IWeightedDiGraph<GameObject, float> graph = PositionNode.mbGraph.graph;
                    foreach (GraphNode<GameObject> graphNode in graph.Neighbors(PositionNode.nodeId))
                    {
                        if (graphNode.Data.transform == PositionNode.transform)
                        {
                            // we can be hit by this player's oil spill directly
                            playerAttackThreats.Add(p);
                            shouldTakeCover = true;
                        }
                    }
                }
            }

            return (int)BTNode.EState.Success;
        }

        public int MoveToCover()
        {
            // TODO: foreach player in the playerAttackThreats list, check if they have ranged/attack items
            //       if they do, then foreach spot that the player can reach choose the one that is safest
            //       and add the CRMove() coroutine to the queue (see below) and return success
            //       else if there is no cover available then return failure

            // TODO: in AbstractPlayer, make a function that gets a list of all reachable nodes (given current action points)
            List<MBGraphNode> reachableNodes = new List<MBGraphNode>();
            // reachableNodes = GetAllReachableNodes();

            List<MBGraphNode> coverNodes = reachableNodes.Select(n => n).ToList();
            foreach (AbstractPlayer player in playerAttackThreats)
            {
                int pMissileIndex = player.Inventory.GetItemIndex("Missile");

                if (pMissileIndex != -1)
                {
                    foreach (MBGraphNode node in coverNodes)
                    {
                        // if we don't hit something, then there is no cover at this node and so, we remove it from the cover list
                        RaycastHit[] hits = Physics.RaycastAll(player.transform.position, node.transform.position - player.transform.position);
                        bool coverAvailable = false;
                        foreach (RaycastHit hit in hits)
                        {
                            // ignore ourselves
                            if (hit.collider.transform != transform)
                                continue;

                            coverAvailable = true;
                            break;
                        }

                        if (!coverAvailable)
                            coverNodes.Remove(node);
                    }
                }


            }

            if (coverNodes.Count > 0)
            {
                // TODO: Get cover node that is closest to the closest checkpoint/goal
                // need a way to get the checkpoint/goal nodes
                MBGraphNode optimalCoverNode = coverNodes[0];

                List<GraphNode<GameObject>> path = pathFinding.FindPath(PositionNode.nodeId, optimalCoverNode.nodeId, MovementHeuristic);
                actionQueue.Enqueue(CRMove(path));

                return (int)BTNode.EState.Success;
            }
            else
                return (int)BTNode.EState.Failure;
        }
        
        public int IsItemInRange()
        {
            var nodesWithItems = new List<MBGraphNode>();
            
            // Check all reachable nodes to see if it contains a PowerUp
            foreach (var nodeID in reachableNodeIds)
            {
                var node = GameplayManager.Instance.gridGraph.graph[nodeID];
                var hitColliders = Physics.OverlapSphere(node.Data.transform.position, 2.5f);

                // Check all colliders and see if one of them is a PowerUp
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.CompareTag("PowerUp"))
                    {
                        nodesWithItems.Add(GameplayManager.Instance.gridGraph.graph[nodeID].Data.GetComponent<MBGraphNode>());
                    }
                }
            }

            // At least one PowerUp is in Range
            if (nodesWithItems.Count > 0)
            {
                var shortestDistance = int.MaxValue;
                var closestNode = nodesWithItems[0];
                
                // Find the closest PowerUp
                foreach (var node in nodesWithItems)
                {
                    List<GraphNode<GameObject>> path = pathFinding.FindPath(PositionNode.nodeId, node.nodeId, MovementHeuristic);

                    if (path.Count < shortestDistance)
                    {
                        shortestDistance = path.Count;
                        closestNode = node;
                    }
                }

                moveTargetNode = closestNode;
                return (int)BTNode.EState.Success;
            }

            // No Items in Range
            return (int)BTNode.EState.Failure;
        }

        public int ShouldGetItem()
        {
            // for now I simply only check if they have room for more items
            // but we should also check if they are in the lead, if so then keep pushing towards goal or randomly decide to get an item if they have none

            // TODO: Inventory needs to expose size
            //if (Inventory.items.Count < Inventory.Size)
            //    return (int)BTNode.EState.Success;
            //else
            //    return (int)BTNode.EState.Failure;

            return (int)BTNode.EState.Success;
        }

        public int MoveToItem()
        {
            List<GraphNode<GameObject>> path = new List<GraphNode<GameObject>>();

            path = pathFinding.FindPath(PositionNode.nodeId, moveTargetNode.nodeId, MovementHeuristic);

            actionQueue.Enqueue(CRMove(path));

            return (int)BTNode.EState.Success;
        }

        public int MoveToBestSpot()
        {
            if (CurrentActionPoints <= 0) return (int)BTNode.EState.Failure;
            
            // get the closest checkpoint/goal node
            float cpDist = Mathf.Infinity;
            Checkpoint checkpointTarget = null;
            foreach (Checkpoint cp in checkpoints)
            {
                if (cp != null)
                {
                    if (checkpointTarget != null && cp.isGoal) continue;

                    float tempDist = (cp.node.transform.position - transform.position).magnitude;
                    if (tempDist < cpDist || checkpointTarget.isGoal)
                    {
                        checkpointTarget = cp;
                        cpDist = tempDist;
                    }
                }
            }

            MBGraphNode checkpointNode = checkpointTarget == null ? null : checkpointTarget.node;
            if (checkpointNode == null)
                return (int)BTNode.EState.Failure;

            // get the closest reachable node to the checkpoint/goal
            GraphNode<GameObject> optimalGraphNode = reachableNodeIds.Count > 0 ? GameplayManager.Instance.gridGraph.graph[reachableNodeIds[0]] : null;
            float dist = Mathf.Infinity;
            foreach (string nodeId in reachableNodeIds)
            {
                GraphNode<GameObject> graphNode = GameplayManager.Instance.gridGraph.graph[nodeId];
                float tempDist = (graphNode.Data.transform.position - checkpointNode.transform.position).magnitude;
                if (tempDist < dist)
                {
                    optimalGraphNode = graphNode;
                    dist = tempDist;
                }
            }

            if (optimalGraphNode != null)
            {
                List<GraphNode<GameObject>> path = pathFinding.FindPath(PositionNode.nodeId, optimalGraphNode.Id, MovementHeuristic);
                actionQueue.Enqueue(CRMove(path));

                return (int)BTNode.EState.Success;
            }
            else
                return (int)BTNode.EState.Failure; // <- we cannot move anymore
        }

        public int EndTurn()
        {
            Phase = EPlayerPhase.End;

            return (int)BTNode.EState.Success;
        }

        // Returns all nodes reachable by the players current action points
        private void GetAllReachableNodes(string positionNodeID, int iterationsLeft)
        {
            if (iterationsLeft <= 0)
            {
                return;
            }
            
            var neighbors = GameplayManager.Instance.gridGraph.graph.Neighbors(positionNodeID);
            reachableNodeIds.Add(positionNodeID);
            
            foreach (var node in neighbors)
            {
                if (!reachableNodeIds.Contains(node.Id))
                {
                    GetAllReachableNodes(node.Id, iterationsLeft - 1);
                }
            }
        }
    }
}
