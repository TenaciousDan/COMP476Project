using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tenacious.Collections
{
    public class MBGraph : MonoBehaviour
    {
        [HideInInspector]
        public SWeightedDiGraph<GameObject, float> graph = new SWeightedDiGraph<GameObject, float>();

        public GameObject AddNode(MBGraphNode from = null, float edgeWeight = default, EdgeDirectionality directionality = default)
        {
            GameObject obj = new GameObject("Node", typeof(MBGraphNode));
            obj.transform.parent = transform;
            obj.transform.position = from != null ? from.transform.position : transform.position;

            MBGraphNode graphNodeComponent = obj.GetComponent<MBGraphNode>();
            graphNodeComponent.mbGraph = this;

            if (from == null)
                graphNodeComponent.nodeId = graph.AddNode(obj).Id;
            else
                graphNodeComponent.nodeId = graph.AddNode(obj, from.nodeId, edgeWeight, directionality).Id;

            return obj;
        }

        public void SetEdge(MBGraphNode from, MBGraphNode to, float edgeWeight = default, EdgeDirectionality directionality = default)
        {
            SetEdge(from.nodeId, to.nodeId, edgeWeight, directionality);
        }
        public void SetEdge(string fromId, string toId, float edgeWeight = default, EdgeDirectionality directionality = default)
        {
            graph.SetEdge(fromId, toId, edgeWeight, directionality);
        }

        public void RemoveEdge(MBGraphNode from, MBGraphNode to, float edgeWeight = default, EdgeDirectionality directionality = default)
        {
            RemoveEdge(from.nodeId, to.nodeId);
        }
        public void RemoveEdge(string fromId, string toId, float edgeWeight = default, EdgeDirectionality directionality = default)
        {
            graph.RemoveEdge(fromId, toId);
        }

        public void DestroyNode(MBGraphNode node)
        {
            Destroy(node);
        }

#if UNITY_EDITOR
        //[HideInInspector]
        //public int __addNodeEdgeDirection = 2;

        public static Color __nodeGizmoColor = Color.cyan;
        public static float __nodeGizmoRadius = 0.5f;
        public static Color __edgeGizmoColor = Color.white;
        public static Color __edgeGizmoArrowColor = Color.white;
        public static float __arrowTipSize = 0.2f;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(__edgeGizmoColor.r, __edgeGizmoColor.g, __edgeGizmoColor.b, __edgeGizmoColor.a * 0.5f);
            Handles.color = __edgeGizmoArrowColor;
            foreach (GraphEdge<float> edge in graph.Edges())
            {
                Gizmos.DrawLine(graph[edge.FromId].Data.transform.position, graph[edge.ToId].Data.transform.position);

                Vector3 edgeVector = graph[edge.ToId].Data.transform.position - graph[edge.FromId].Data.transform.position;
                Vector3 midpoint = graph[edge.FromId].Data.transform.position + (edgeVector.normalized * (edgeVector.magnitude / 2));
                Vector3 arrowTipPosition = midpoint - (edgeVector.normalized * __arrowTipSize / 2);

                if (edge.Directionality == EdgeDirectionality.UniDirectional)
                {
                    Handles.ConeHandleCap(
                        0,
                        arrowTipPosition,
                        edgeVector == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(edgeVector),
                        __arrowTipSize,
                        EventType.Repaint
                    );
                }

                // Vector3 quarterpoint = graph[edge.FromId].Data.transform.position + (edgeVector / 4);
                // Handles.Label(quarterpoint + Vector3.up * (__arrowTipSize * 2), edge.Weight.ToString("0.###"));
            }
        }
#endif
    }
}
