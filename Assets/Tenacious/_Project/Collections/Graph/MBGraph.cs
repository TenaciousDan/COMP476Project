using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

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
        [Header("Gizmos")]
        public Color __nodeGizmoColor = Color.cyan;
        public float __nodeGizmoRadius = 0.5f;
        public Color __edgeGizmoColor = Color.white;
        public bool __showEdgeGizmos = true;
        public bool __showNodeGizmos = false;

        // generation properties
        public enum EGenerationType { Grid };
        [HideInInspector] public EGenerationType __generationType = EGenerationType.Grid;
        [HideInInspector] public bool __detectCollisions = true;
        [HideInInspector] public float __collisionRadius = 0.25f;
        [HideInInspector] public bool __useTagsToIgnoreCollisions = false;
        [HideInInspector] public List<string> __collisionTags;
        [HideInInspector] public Color __edgeGizmoArrowColor = Color.white;
        [HideInInspector] public float __arrowTipSize = 0.5f;

        // grid generation properties
        [HideInInspector] public int __gridColumns = 1;
        [HideInInspector] public int __gridRows = 1;
        [HideInInspector] public float __gridCellSize = 1;

        private void OnDrawGizmos()
        {
            if (__showEdgeGizmos)
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
                }
            }
        }
#endif
    }
}
