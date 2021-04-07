using UnityEngine;

namespace Game.AI
{
    public class ActionBTNode : BTNode
    {
        public delegate EState ActionBTNodeDelegate();
        private ActionBTNodeDelegate action;

        public ActionBTNode() { }

        public ActionBTNode(ActionBTNodeDelegate action)
        {
            this.action = action;
        }

        public ActionBTNodeDelegate Action { get => action; set => action = value; }

        public override EState Evaluate()
        {
            if (action != null)
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
            else
            {
                State = EState.Success;
                return State;
            }
        }
    }
}
