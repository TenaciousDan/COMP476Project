using UnityEngine;

using System;
using System.Collections.Generic;

namespace Tenacious.Data
{
    [Serializable]
    public class BTGraphData : ScriptableObject
    {
        public BTGraphNodeData rootNode;
        public List<BTGraphNodeData> nodes = new List<BTGraphNodeData>();
        public List<BTGraphEdgeData> edges = new List<BTGraphEdgeData>();
    }

    [Serializable]
    public class BTGraphNodeData
    {
        public string id;
        public bool isRootNode;
        public string title;
        public string nodeType;
        public Vector2 position;
    }

    [Serializable]
    public class BTGraphEdgeData
    {
        public string fromId;
        public string toId;
    }
}
