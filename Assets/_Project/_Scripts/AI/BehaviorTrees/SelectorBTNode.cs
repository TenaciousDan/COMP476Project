using UnityEngine;

using System;
using System.Collections.Generic;

namespace Game.AI
{
    public class SelectorBTNode : BTNode
    {
        protected List<BTNode> nodes = new List<BTNode>();

        public SelectorBTNode() { }

        public SelectorBTNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }

        public List<BTNode> Nodes { get => nodes; }

        public override EState Evaluate()
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                BTNode node = nodes[i];
                switch (node.Evaluate())
                {
                    case EState.Running:
                        State = EState.Running;
                        return State;
                    case EState.Success:
                        State = EState.Success;
                        return State;
                    case EState.Failure:
                        break;
                    default:
                        break;
                }
            }

            State = EState.Failure;

            return State;
        }
    }
}
