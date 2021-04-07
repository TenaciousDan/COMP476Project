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

        [Serializable]
        public class ActionDescriptor
        {
            public string key;
            public ActionCallback action;
        }

        [SerializeField] private BTGraphData behaviorTreeGraphData;
        [SerializeField] private List<ActionDescriptor> actions;

        private Dictionary<string, ActionDescriptor> actionDict;

        protected BTNode rootNode;
        public BTNode RootNode
        {
            get => rootNode;
            set => rootNode = value;
        }

        public void Awake()
        {
            actionDict = new Dictionary<string, ActionDescriptor>();
            foreach (ActionDescriptor ad in actions)
                actionDict.Add(ad.key, ad);

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
                        nodeDict.Add(nodeData.id, new SelectorBTNode());
                    else if ("Sequence".Equals(nodeData.nodeType))
                        nodeDict.Add(nodeData.id, new SequenceBTNode());
                    else if ("Inverter".Equals(nodeData.nodeType))
                        nodeDict.Add(nodeData.id, new InverterBTNode());
                    else if ("Action".Equals(nodeData.nodeType) && actionDict.ContainsKey(nodeData.title))
                        nodeDict.Add(nodeData.id, new ActionBTNode(() => { return (BTNode.EState) actionDict[nodeData.title].action.Invoke(); }));
                }
            }

            foreach (BTGraphEdgeData edgeData in data.edges)
            {
                if (nodeDict.ContainsKey(edgeData.fromId))
                {
                    if (nodeDict[edgeData.fromId] is SelectorBTNode)
                    {
                        if (nodeDict.ContainsKey(edgeData.toId))
                            ((SelectorBTNode)nodeDict[edgeData.fromId]).Nodes.Add(nodeDict[edgeData.toId]);
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
        }
    }
}
