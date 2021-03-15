using System.Collections.Generic;

namespace Tenacious.Collections
{
    public interface IWeightedDiGraph<N, E>
    {
        int Count { get; }
        int EdgeCount { get; }

        void Clear();

        bool Contains(string nodeId);

        GraphNode<N> AddNode(N data);
        GraphNode<N> AddNode(string uniqueId, N data);

        GraphNode<N> AddNode(N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default);
        GraphNode<N> AddNode(N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default);
        GraphNode<N> AddNode(string uniqueId, N data, GraphNode<N> from, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default);
        GraphNode<N> AddNode(string uniqueId, N data, string fromId, E edgeWeight = default, EdgeDirectionality edgeDirectionality = default);

        void RemoveNode(GraphNode<N> node);
        void RemoveNode(string nodeId);

        GraphEdge<E> GetEdge(GraphNode<N> from, GraphNode<N> to);
        GraphEdge<E> GetEdge(string fromId, string toId);

        void SetEdge(GraphNode<N> from, GraphNode<N> to, E weight = default, EdgeDirectionality directionality = default);
        void SetEdge(string fromId, string toId, E weight = default, EdgeDirectionality directionality = default);

        void RemoveEdge(GraphNode<N> from, GraphNode<N> to);
        void RemoveEdge(string fromId, string toId);

        List<GraphNode<N>> Neighbors(GraphNode<N> node);
        List<GraphNode<N>> Neighbors(string nodeId);

        public GraphNode<N> this[string id] { get; }
    }
}
