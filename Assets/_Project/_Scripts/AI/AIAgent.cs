using UnityEngine;

namespace Game.AI
{
    public class AIAgent : MonoBehaviour
    {
        [SerializeField] private BehaviorTree behaviorTree;

        public int IsItemBoxAvailable()
        {
            return (int)BTNode.EState.Success;
        }
    }
}
