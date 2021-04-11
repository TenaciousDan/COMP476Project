using UnityEngine;

namespace Game.AI
{
    public class AIPlayer : AbstractPlayer
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private BehaviorTree behaviorTree;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void MainPhase()
        {
            Phase = EPlayerPhase.Main;
        }

        // behavior tree methods
        public int IsItemBoxAvailable()
        {
            return (int)BTNode.EState.Success;
        }
    }
}
