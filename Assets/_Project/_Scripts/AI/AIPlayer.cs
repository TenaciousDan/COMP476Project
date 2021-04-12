using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Game.AI
{
    [RequireComponent(typeof(Pathfinding))]
    public class AIPlayer : AbstractPlayer
    {
        [SerializeField] private BehaviorTree behaviorTree;

        private Queue<IEnumerator> actionQueue;
        private bool isProcessingActions;

        protected override void Awake()
        {
            base.Awake();

            actionQueue = new Queue<IEnumerator>();
        }

        public override void MainPhaseUpdate()
        {
            Phase = EPlayerPhase.Main;
            // Enqueue actions
            // ...

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

        /**********************************
         * BehaviorTree leaf node methods
         ***********************************/
        public int IsItemBoxAvailable()
        {
            return (int)BTNode.EState.Success;
        }
    }
}
