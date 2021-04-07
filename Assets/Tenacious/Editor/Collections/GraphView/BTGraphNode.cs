using UnityEditor;
using UnityEditor.Experimental.GraphView;

using System;

namespace TenaciousEditor.Collections
{
    public class BTGraphNode : Node
    {
        public string Id { get; set; }
        public bool IsRootNode { get; }
        public string NodeType { get; set; }

        public BTGraphNode(bool isRootNode = false) : base()
        {
            Id = Guid.NewGuid().ToString();
            title = "";
            IsRootNode = isRootNode;
        }
    }
}
