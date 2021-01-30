using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

namespace Tenacious.Collections
{
    [ExecuteInEditMode]
    public class MBGraphNode : MonoBehaviour
    {
        [HideInInspector] public MBGraph mbGraph;
        [HideInInspector] public string nodeId;

        private void OnDestroy()
        {
            if (mbGraph != null)
                mbGraph.graph.RemoveNode(nodeId);
        }

#if UNITY_EDITOR
        public static Color __nodeGizmoColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f);
        public static float __nodeGizmoRadius = 0.5f;

        private void OnDrawGizmos()
        {
            int selectedMBGraphNodeCount = 0;
            int currentIndexInSelection = -1;
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.GetComponent<MBGraphNode>() != null)
                {
                    if (obj.GetComponent<MBGraphNode>() == this)
                        currentIndexInSelection = selectedMBGraphNodeCount;

                    ++selectedMBGraphNodeCount;
                }
            }

            Gizmos.color = __nodeGizmoColor;
            if (currentIndexInSelection != -1)
            {
                Gizmos.DrawCube(transform.position, new Vector3(__nodeGizmoRadius * 1.5f, __nodeGizmoRadius * 1.5f, __nodeGizmoRadius * 1.5f));
                if (currentIndexInSelection == 0 && selectedMBGraphNodeCount == 2)
                    Gizmos.DrawIcon(transform.position, "PreTexA@2x", false, new Color(1 - __nodeGizmoColor.r, 1 - __nodeGizmoColor.g, 1 - __nodeGizmoColor.b));
                else if (currentIndexInSelection == 1 && selectedMBGraphNodeCount == 2)
                    Gizmos.DrawIcon(transform.position, "PreTexB@2x", false, new Color(1 - __nodeGizmoColor.r, 1 - __nodeGizmoColor.g, 1 - __nodeGizmoColor.b));
            }
            else
                Gizmos.DrawSphere(transform.position, __nodeGizmoRadius);
        }
#endif
    }
}
