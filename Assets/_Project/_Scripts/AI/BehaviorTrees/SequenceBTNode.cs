using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.AI
{
    public class SequenceBTNode : BTNode
    {
        protected List<BTNode> nodes = new List<BTNode>();

        public SequenceBTNode() { }

        public SequenceBTNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }

        public List<BTNode> Nodes { get => nodes; }

        public override EState Evaluate()
        {
            bool isAnyNodeRunning = false;

            for (int i = 0; i < nodes.Count; ++i)
            {
                BTNode node = nodes[i];
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
