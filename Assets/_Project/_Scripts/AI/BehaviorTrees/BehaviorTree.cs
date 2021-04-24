using UnityEngine;

using System;
using System.Collections.Generic;

using Tenacious.Data;

namespace Game.AI
{
    public class BehaviorTree : MonoBehaviour
    {
        [Serializable]
        public class ActionCallback : SerializableCallback<int> { }

        [SerializeField] private BTGraphData behaviorTreeGraphData;
        [SerializeField] private List<ActionCallback> actions;

        private Dictionary<string, ActionCallback> actionDict;

        protected BTNode rootNode;
        public BTNode RootNode
        {
            get => rootNode;
            set => rootNode = value;
        }

        public void Awake()
        {
            actionDict = new Dictionary<string, ActionCallback>();
            foreach (ActionCallback ac in actions)
            {
                actionDict.Add(ac.methodName, ac);
            }

            if (behaviorTreeGraphData != null)
                ConstructFromBTGraphData(behaviorTreeGraphData);
        }

        public void ConstructFromBTGraphData(BTGraphData data)
        {
            string rootId = null;
            Dictionary<string, BTNode> nodeDict = new Dictionary<string, BTNode>();

            foreach (BTGraphNodeData nodeData in data.nodes)
            {
                if (nodeData.isRootNode)
                {
                    rootId = nodeData.id;
                    nodeDict.Add(rootId, new SelectorBTNode());
                }
                else
                {
                    if ("Selector".Equals(nodeData.nodeType))
                        nodeDict.Add(nodeData.id, new SelectorBTNode() { order = nodeData.position.y });
                    else if ("Sequence".Equals(nodeData.nodeType))
                        nodeDict.Add(nodeData.id, new SequenceBTNode() { order = nodeData.position.y });
                    else if ("Inverter".Equals(nodeData.nodeType))
                        nodeDict.Add(nodeData.id, new InverterBTNode() { order = nodeData.position.y });
                    else if ("Action".Equals(nodeData.nodeType) && actionDict.ContainsKey(nodeData.title))
                        nodeDict.Add(nodeData.id, new ActionBTNode(() => { return (BTNode.EState) actionDict[nodeData.title].Invoke(); }) { order = nodeData.position.y });
                }
            }

            foreach (BTGraphEdgeData edgeData in data.edges)
            {
                if (nodeDict.ContainsKey(edgeData.fromId))
                {
                    if (nodeDict[edgeData.fromId] is SelectorBTNode)
                    {
                        if (nodeDict.ContainsKey(edgeData.toId))
                        {

                            ((SelectorBTNode)nodeDict[edgeData.fromId]).Nodes.Add(nodeDict[edgeData.toId]);
                        }
                    }
                    else if (nodeDict[edgeData.fromId] is SequenceBTNode)
                    {
                        if (nodeDict.ContainsKey(edgeData.toId))
                            ((SequenceBTNode)nodeDict[edgeData.fromId]).Nodes.Add(nodeDict[edgeData.toId]);
                    }
                    else if (nodeDict[edgeData.fromId] is InverterBTNode)
                    {
                        if (nodeDict.ContainsKey(edgeData.toId))
                            ((InverterBTNode)nodeDict[edgeData.fromId]).Node = nodeDict[edgeData.toId];
                    }
                }
            }

            // sort children based on position (quick fix)
            foreach (string key in nodeDict.Keys)
            {
                if (nodeDict[key] is SelectorBTNode)
                {
                    ((SelectorBTNode)nodeDict[key]).Nodes.Sort((n1, n2) =>
                    {
                        return n1.order.CompareTo(n2.order);
                    });
                }
                else if (rootNode is SequenceBTNode)
                {
                    ((SequenceBTNode)nodeDict[key]).Nodes.Sort((n1, n2) =>
                    {
                        return n1.order.CompareTo(n2.order);
                    });
                }
            }

            rootNode = nodeDict[rootId];
        }

        /// <summary>
        /// Process and Evaluate the behavior tree. <br />
        /// See <see cref="BTNode.Evaluate"/>
        /// </summary>
        /// <returns>Success if one of the branches evalluated successfully, Failure otherwise.</returns>
        public BTNode.EState Evaluate()
        {
            return RootNode.Evaluate();
        }
    }
}
