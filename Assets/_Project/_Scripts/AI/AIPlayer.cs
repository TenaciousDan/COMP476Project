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

        public override void StandbyPhase()
        {
            Phase = EPlayerPhase.Standby;
        }

        public override void MainPhase()
        {
            throw new System.NotImplementedException();
        }

        public override void EndPhase()
        {
            Phase = EPlayerPhase.End;
        }

        // behavior tree methods
        public int IsItemBoxAvailable()
        {
            return (int)BTNode.EState.Success;
        }
    }
}
