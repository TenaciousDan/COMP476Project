using UnityEngine;

using System;
using System.Collections.Generic;

namespace Tenacious.Collections
{
    [Serializable]
    public class SWeightedDiGraph<N, E> : IWeightedDiGraph<N, E>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] string nodeTypeName;
        [SerializeField, HideInInspector] string edgeTypeName;

        [SerializeField] List<SNode> nodes = new List<SNode>();
        [SerializeField] List<SEdge> edges = new List<SEdge>();
        [SerializeField, HideInInspector] bool[] nodeIdCollisions;
        [SerializeField, HideInInspector] WeightedDiGraph<N, E> digraph = new WeightedDiGraph<N, E>();

        [Serializable]
        struct SNode
        {
            public string id;
            public N data;
            public SNode(string id, N data)
            {
                this.id = id;
                this.data = data;
            }
        }

        [Serializable]
        struct SEdge
        {
            public string fromId;
            public string toId;
            public E weight;
            public int directionality;
            public SEdge(string fromId, string toId, E weight, int directionality)
            {
                this.fromId = fromId;
                this.toId = toId;
                this.weight = weight;
                this.directionality = directionality;
            }
        }

        public void OnBeforeSerialize()
        {
            nodes.Clear();
            edges.Clear();

            // first pass, store node data
            foreach (GraphNode<N> graphNode in digraph.Nodes())
            {
                SNode snode = new SNode(graphNode.Id, graphNode.Data);
                if (!nodes.Contains(snode))
                {
                    nodes.Add(snode);
                }
            }

            // second pass, store edge data
            foreach (GraphEdge<E> graphEdge in digraph.Edges())
            {
                SEdge sedge = new SEdge(graphEdge.FromId, graphEdge.ToId, graphEdge.Weight, (int)graphEdge.Directionality);
                if (!edges.Contains(sedge))
                {
                    edges.Add(sedge);
                }
            }

            Type nType = typeof(N);
            Type eType = typeof(E);
            if (nodeTypeName == null || (!nodeTypeName.Equals(nType) || !edgeTypeName.Equals(eType)))
            {
                nodeTypeName = nType.IsPrimitive || nType.Equals(typeof(string)) ? nType.Name.ToLower() : nType.Name;
                edgeTypeName = eType.IsPrimitive || eType.Equals(typeof(string)) ? eType.Name.ToLower() : eType.Name;
            }
        }

        public void OnAfterDeserialize()
        {
            nodeIdCollisions = new bool[nodes.Count];
            digraph = new WeightedDiGraph<N, E>(nodes.Count);

            // first pass, restore nodes
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id != null)
                {
                    if (!Contains(nodes[i].id))
                    {
                        AddNode(nodes[i].id, nodes[i].data);
                        nodeIdCollisions[i] = false;
                    }
                    else
                        nodeIdCollisions[i] = true;
                }
            }

            // second pass, restore edges
            for (int i = 0; i < edges.Count; i++)
            {
                try
                {
                    SetEdge(edges[i].fromId, edges[i].toId, edges[i].weight, (EdgeDirectionality)edges[i].directionality);
                }
                catch (Exception e) when (e is ArgumentNullException || e is ArgumentException)
                {
                    // ignore the edge
                }
            }
        }

        public SWeightedDiGraph()
        {
            digraph = new WeightedDiGraph<N, E>();
        }
        public SWeightedDiGraph(int capacity)
        {
            digraph = new WeightedDiGraph<N, E>(capacity);
        }

        public WeightedDiGraph<N, E> InternalGraph { get => digraph; }

        public int Count => digraph.Count;

        public int EdgeCount => digraph.EdgeCount;

        public void Clear() { digraph.Clear(); }

        public IEnumerable<GraphNode<N>> Nodes()
        {
            return digraph.Nodes();
        }

        public IEnumerable<GraphEdge<E>> Edges()
        {
            return digraph.Edges();
        }

        public GraphNode<N> this[string id]
        {
            get => digraph[id];
        }

        public bool Contains(string nodeId)
        {
            return digraph.Contains(nodeId);
        }

        public GraphNode<N> AddNode(N data)
        {
            return digraph.AddNode(data);
        }
        public GraphNode<N> AddNode(string uniqueId, N data)
        {
            return digraph.AddNode(uniqueId, data);
        }
        public GraphNode<N> AddNode(N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = EdgeDirectionality.BiDirectional)
        {
            return digraph.AddNode(data, from, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = EdgeDirectionality.BiDirectional)
        {
            return digraph.AddNode(data, fromId, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(string uniqueId, N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = EdgeDirectionality.BiDirectional)
        {
            return digraph.AddNode(uniqueId, data, from, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(string uniqueId, N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = EdgeDirectionality.BiDirectional)
        {
            return digraph.AddNode(uniqueId, data, fromId, edgeWeight, edgeDirectionality);
        }

        public GraphEdge<E> GetEdge(GraphNode<N> from, GraphNode<N> to)
        {
            return digraph.GetEdge(from, to);
        }
        public GraphEdge<E> GetEdge(string fromId, string toId)
        {
            return digraph.GetEdge(fromId, toId);
        }

        public List<GraphNode<N>> Neighbors(GraphNode<N> node)
        {
            return digraph.Neighbors(node);
        }
        public List<GraphNode<N>> Neighbors(string nodeId)
        {
            return digraph.Neighbors(nodeId);
        }

        public void RemoveEdge(GraphNode<N> from, GraphNode<N> to)
        {
            digraph.RemoveEdge(from, to);
        }
        public void RemoveEdge(string fromId, string toId)
        {
            digraph.RemoveEdge(fromId, toId);
        }

        public void RemoveNode(GraphNode<N> node)
        {
            digraph.RemoveNode(node);
        }
        public void RemoveNode(string nodeId)
        {
            digraph.RemoveNode(nodeId);
        }

        public void SetEdge(GraphNode<N> from, GraphNode<N> to, E weight = default, EdgeDirectionality directionality = EdgeDirectionality.BiDirectional)
        {
            digraph.SetEdge(from, to, weight, directionality);
        }
        public void SetEdge(string fromId, string toId, E weight = default, EdgeDirectionality directionality = EdgeDirectionality.BiDirectional)
        {
            digraph.SetEdge(fromId, toId, weight, directionality);
        }
    }
}
