using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.AI
{
    public class InverterBTNode : BTNode
    {
        protected BTNode node;

        public InverterBTNode() { }

        public InverterBTNode(BTNode node)
        {
            this.node = node;
        }

        public BTNode Node { get => node; set => node = value; }

        public override EState Evaluate()
        {
            switch (node.Evaluate())
            {
                case EState.Running:
                    State = EState.Running;
                    break;
                case EState.Success:
                    State = EState.Failure;
                    break;
                case EState.Failure:
                    State = EState.Success;
                    break;
                default:
                    break;
            }

            return State;
        }
    }
}
