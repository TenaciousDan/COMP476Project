using UnityEngine;

using System.Collections;
using System.Collections.Generic;

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

            behaviorTree.Evaluate();

            // Process actions
            if (!isProcessingActions)
                StartCoroutine(ProcessActionsQueue());
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
        private List<AbstractPlayer> playerAttackThreats = new List<AbstractPlayer>();
        private MBGraphNode moveTargetNode;

        /// <summary>
        /// Resets the behavior tree properties. Should be called before each time the behavior tree is evaluated. <br />
        /// See <see cref="BehaviorTree.Evaluate"/>
        /// </summary>
        private void ResetBehaviorTreeProperties()
        {
            itemToUseIndex = -1;
            shouldTakeCover = false;
            playerAttackThreats.Clear();
            moveTargetNode = null;
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

            return (int)BTNode.EState.Success;
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
            // TODO: foreach player in the game, check if they have ranged/attack items
            //       if they do, then get the item with the longest range and check if they can reach this player
            //       if they can reach us 
            //           set shouldTakeCover to true
            //           add players who are a threat to playerAttackThreats list
            //           return Success
            //       else if no one can reach us with an attack
            //           return failure


            return (int)BTNode.EState.Success;
        }

        public int MoveToCover()
        {
            // TODO: foreach player in the playerAttackThreats list, check if they have ranged/attack items
            //       if they do, then foreach spot that the player can reach choose the one that is safest
            //       and add the CRMove() coroutine to the queue (see below) and return success
            //       else if there is no cover available then return failure

            // actionQueue.Enqueue(CRMove(path));

            return (int)BTNode.EState.Success;
        }

        public int IsItemInRange()
        {
            // TODO: check if item box is in range (there's enough action points to reach it)
            //       if true 
            //           set moveTargetNode and return success
            //       else
            //           return failure

            return (int)BTNode.EState.Success;
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
            // TODO: set moveTargetNode to closest checkpoint
            //       implement GetCostToMove() function that returns how much it costs to move (ex: player could have a debuff that makes it cost more/less to move)

            List<GraphNode<GameObject>> path = new List<GraphNode<GameObject>>();
            path = pathFinding.FindPath(PositionNode.nodeId, moveTargetNode.nodeId, MovementHeuristic);

            while (path.Count > CurrentActionPoints) // use GetCostToMove() instead of CurrentActionPoints
            {
                actionQueue.Enqueue(CRMove(path));
            }

            return (int)BTNode.EState.Success;
        }

        public int EndTurn()
        {
            Phase = EPlayerPhase.End;

            return (int)BTNode.EState.Success;
        }
    }
}
