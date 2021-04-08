using UnityEngine;

namespace Game.AI
{
    public class AIPlayerTurnStrategy : MonoBehaviour, IPlayerTurnStrategy
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private BehaviorTree behaviorTree;

        public void doMainPhase()
        {
            throw new System.NotImplementedException();
        }

        public void computePath()
        {
            throw new System.NotImplementedException();
        }

        public void useItem()
        {
            throw new System.NotImplementedException();
        }

        public int IsItemBoxAvailable()
        {
            return (int)BTNode.EState.Success;
        }
    }
}
