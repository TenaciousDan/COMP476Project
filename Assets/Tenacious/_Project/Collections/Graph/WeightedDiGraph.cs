using System;
using System.Collections.Generic;

namespace Tenacious.Collections
{
    public class WeightedDiGraph<N, E> : IWeightedDiGraph<N, E>
    {
        private Dictionary<string, GraphEntry> graphEntries;

        public WeightedDiGraph()
        {
            graphEntries = new Dictionary<string, GraphEntry>();
        }

        public WeightedDiGraph(int capacity)
        {
            graphEntries = new Dictionary<string, GraphEntry>(capacity);
        }

        public int Count { get => graphEntries.Count; }
        public int EdgeCount { get; private set; }

        public void Clear() { graphEntries.Clear(); }

        public IEnumerable<GraphNode<N>> Nodes()
        {
            foreach (GraphEntry entry in graphEntries.Values)
                yield return entry.node;
        }

        public IEnumerable<GraphEdge<E>> Edges()
        {
            foreach (GraphEntry entry in graphEntries.Values)
                foreach (GraphEdge<E> edge in entry.edgeDict.Values)
                    yield return edge;
        }

        public GraphNode<N> this[string id]
        {
            get => graphEntries[id].node;
        }

        public bool Contains(string nodeId)
        {
            return graphEntries.ContainsKey(nodeId);
        }

        public GraphNode<N> AddNode(N data) { return AddNode(Guid.NewGuid().ToString(), data); }
        public GraphNode<N> AddNode(string uniqueId, N data)
        {
            GraphNode<N> addedNode = null;
            if (uniqueId == null)
                throw new ArgumentNullException("Node id cannot be null.");
            else if (graphEntries.ContainsKey(uniqueId))
                throw new ArgumentException("Duplicate node ID detected. Node id must be unique. Node was rejected.");
            else
            {
                addedNode = new GraphNode<N>(uniqueId, data);
                graphEntries.Add(uniqueId, new GraphEntry() { node = addedNode });
            }

            return addedNode;
        }

        public GraphNode<N> AddNode(N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default)
        {
            return AddNode(data, from.Id, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default)
        {
            return AddNode(Guid.NewGuid().ToString(), data, fromId, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(string uniqueId, N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default)
        {
            return AddNode(uniqueId, data, from.Id, edgeWeight, edgeDirectionality);
        }
        public GraphNode<N> AddNode(string uniqueId, N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default)
        {
            GraphNode<N> addedNode = AddNode(uniqueId, data);
            SetEdge(fromId, uniqueId, edgeWeight, edgeDirectionality);

            return addedNode;
        }

        public GraphEdge<E> GetEdge(GraphNode<N> from, GraphNode<N> to)
        {
            return GetEdge(from.Id, to.Id);
        }
        public GraphEdge<E> GetEdge(string fromId, string toId)
        {
            GraphEdge<E> edge = null;

            if (fromId == null || toId == null)
                throw new ArgumentNullException("Node ids cannot be null.");
            else if (!graphEntries.ContainsKey(fromId) || !graphEntries.ContainsKey(fromId))
                throw new ArgumentException("Node ids must be valid and must already exist in the graph.");
            else
            {
                foreach (string otherId in graphEntries[fromId].edgeDict.Keys)
                {
                    if (otherId.Equals(toId))
                    {
                        edge = graphEntries[fromId][toId];
                        break;
                    }
                }
            }

            return edge;
        }

        public List<GraphNode<N>> Neighbors(GraphNode<N> node) { return Neighbors(node.Id); }
        public List<GraphNode<N>> Neighbors(string nodeId)
        {
            List<GraphNode<N>> neighbors = new List<GraphNode<N>>();
            foreach (string otherId in graphEntries[nodeId].edgeDict.Keys)
                neighbors.Add(graphEntries[otherId].node);

            return neighbors;
        }

        public void RemoveEdge(GraphNode<N> from, GraphNode<N> to) { RemoveEdge(from.Id, to.Id); }
        public void RemoveEdge(string fromId, string toId)
        {
            EdgeDirectionality directionality = EdgeDirectionality.UniDirectional;
            if (graphEntries[fromId].edgeDict.ContainsKey(toId))
            {
                directionality = graphEntries[fromId][toId].Directionality;
                graphEntries[fromId].edgeDict.Remove(toId);
            }

            if (directionality == EdgeDirectionality.BiDirectional && graphEntries[toId].edgeDict.ContainsKey(fromId))
                graphEntries[toId].edgeDict.Remove(fromId);
        }

        public void RemoveNode(GraphNode<N> node) { RemoveNode(node.Id); }
        public void RemoveNode(string nodeId)
        {
            if (graphEntries.ContainsKey(nodeId))
            {
                foreach (string otherId in graphEntries[nodeId].edgeDict.Keys)
                {
                    if (graphEntries[nodeId][otherId].Directionality == EdgeDirectionality.BiDirectional)
                        graphEntries[otherId].edgeDict.Remove(nodeId);
                }

                graphEntries.Remove(nodeId);
            }
        }

        public void SetEdge(GraphNode<N> from, GraphNode<N> to, E weight = default, EdgeDirectionality directionality = default)
        {
            SetEdge(from.Id, to.Id, weight, directionality);
        }
        public void SetEdge(string fromId, string toId, E weight = default, EdgeDirectionality directionality = default)
        {
            if (fromId == null || toId == null)
                throw new ArgumentNullException("Node ids cannot be null.");
            else if (!graphEntries.ContainsKey(fromId) || !graphEntries.ContainsKey(fromId))
                throw new ArgumentException("Node ids must be valid and must already exist in the graph.");

            if (!graphEntries[fromId].edgeDict.ContainsKey(toId))
                ++EdgeCount;

            graphEntries[fromId][toId] = new GraphEdge<E>(fromId, toId, (self, newDirectionality) =>
            {
                if (newDirectionality == EdgeDirectionality.UniDirectional)
                {
                    if (graphEntries[toId].edgeDict.ContainsKey(fromId) && graphEntries[toId][fromId].Directionality == EdgeDirectionality.BiDirectional)
                    {
                        graphEntries[toId].edgeDict.Remove(fromId);
                        --EdgeCount;
                    }
                }
                else if (newDirectionality == EdgeDirectionality.BiDirectional)
                {
                    graphEntries[toId][fromId] = self;
                }
            })
            {
                Directionality = directionality,
                Weight = weight
            };
        }

        private class GraphEntry
        {
            public GraphNode<N> node;
            public Dictionary<string, GraphEdge<E>> edgeDict = new Dictionary<string, GraphEdge<E>>();

            public GraphEdge<E> this[string id]
            {
                get => edgeDict[id];
                set => edgeDict[id] = value;
            }
        }
    }

    public class GraphNode<N>
    {
        public GraphNode(N data) : this(Guid.NewGuid().ToString(), data) { }
        public GraphNode(string uniqueId, N data)
        {
            Id = uniqueId;
            Data = data;
        }

        public string Id { get; }
        public N Data { get; set; }
    }

    public class GraphEdge<E>
    {
        public delegate void EdgeDirectionalityChangeCallback(GraphEdge<E> self, EdgeDirectionality newEdgeDirectionality);
        private EdgeDirectionalityChangeCallback edgeDirectionalityChangeCallback;

        public GraphEdge(string fromId, string toId, EdgeDirectionalityChangeCallback edgeDirectionalityChangeCallback = null)
        {
            this.FromId = fromId;
            this.ToId = toId;
            this.edgeDirectionalityChangeCallback += edgeDirectionalityChangeCallback;
        }

        public string FromId { get; }
        public string ToId { get; }

        public E Weight { get; set; }

        private EdgeDirectionality directionality;
        public EdgeDirectionality Directionality 
        {
            get => directionality;
            set 
            {
                edgeDirectionalityChangeCallback?.Invoke(this, value);
                directionality = value;
            } 
        }
    }

    public enum EdgeDirectionality { BiDirectional, UniDirectional }
}
