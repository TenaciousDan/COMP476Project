using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.AI
{
    public class SequenceBTNode : BTNode
    {
        protected List<BTNode> nodes = new List<BTNode>();

        public SequenceBTNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }

        public override EState Evaluate()
        {
            bool isAnyNodeRunning = false;

            foreach (BTNode node in nodes)
            {
                switch (node.Evaluate())
                {
                    case EState.Running:
                        isAnyNodeRunning = true;
                        break;
                    case EState.Success:
                        break;
                    case EState.Failure:
                        State = EState.Failure;
                        return State;
                    default:
                        break;
                }
            }

            State = isAnyNodeRunning ? EState.Running : EState.Success;

            return State;
        }
    }
}
