using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

using System.Collections.Generic;

using Tenacious.Data;

namespace TenaciousEditor.Collections
{
    public class BTGraphView : GraphView
    {
        private static readonly Vector2 MIN_NODE_SIZE = new Vector2(200, 200);

        public BTGraphNode RootNode { get; private set; }

        public BTGraphView()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();

            CreateBTNode(Vector2.one * 150, true);
        }

        public BTGraphNode CreateBTNode(Vector2 position, bool isRootNode = false)
        {
            return CreateBTNode(new BTGraphNodeData() { position = position, isRootNode = isRootNode });
        }
        public BTGraphNode CreateBTNode(BTGraphNodeData nodeData)
        {
            BTGraphNode node = new BTGraphNode(nodeData.isRootNode) { title = "Node" };

            if (!nodeData.isRootNode)
            {
                Port inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                inputPort.portName = "Input";
                node.inputContainer.Add(inputPort);
            }
            else
            {
                if (RootNode != null) RemoveElement(RootNode);
                node.title = "Root";
                RootNode = node;
                AddElement(RootNode);
            }

            Port outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);

            if (!nodeData.isRootNode)
            {
                TextField txtNodeType = new TextField("Type");
                txtNodeType.value = nodeData.nodeType;
                txtNodeType.AddToClassList("nodetype-input");
                txtNodeType.RegisterValueChangedCallback(evt => 
                {
                    node.NodeType = evt.newValue;
                });
                node.extensionContainer.Add(txtNodeType);

                TextField txtNodeTitle = new TextField();
                txtNodeTitle.AddToClassList("title-input");
                txtNodeTitle.RegisterValueChangedCallback(evt =>
                {
                    node.title = evt.newValue;
                });
                txtNodeTitle.SetValueWithoutNotify(node.title);
                node.titleContainer.RemoveAt(0);
                node.titleContainer.Insert(0, txtNodeTitle);
            }

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(nodeData.position, MIN_NODE_SIZE));

            AddElement(node);

            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach((endPort) =>
            {
                if (startPort != endPort && startPort.node != endPort.node && startPort.direction != endPort.direction)
                    compatiblePorts.Add(endPort);
            });

            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("New Node", (action) => { CreateBTNode(action.eventInfo.mousePosition); });
        }
    }
}
