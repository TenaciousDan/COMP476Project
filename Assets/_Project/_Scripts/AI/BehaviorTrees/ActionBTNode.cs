using UnityEngine;

namespace Game.AI
{
    public class ActionBTNode : BTNode
    {
        public delegate EState ActionBTNodeDelegate();
        private ActionBTNodeDelegate action;

        public ActionBTNode(ActionBTNodeDelegate action)
        {
            this.action = action;
        }

        public override EState Evaluate()
        {
            switch (action())
            {
                case EState.Success:
                    State = EState.Success;
                    return State;
                case EState.Failure:
                    State = EState.Failure;
                    return State;
                case EState.Running:
                    State = EState.Running;
                    return State;
                default:
                    State = EState.Failure;
                    return State;
            }
        }
    }
}
