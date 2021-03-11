using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
        private void OnDrawGizmos()
        {
            if (mbGraph == null || !mbGraph.__showNodeGizmos) return;

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

            Gizmos.color = mbGraph.__nodeGizmoColor;
            if (currentIndexInSelection != -1)
            {
                Gizmos.DrawCube(transform.position, new Vector3(mbGraph.__nodeGizmoRadius * 1.5f, mbGraph.__nodeGizmoRadius * 1.5f, mbGraph.__nodeGizmoRadius * 1.5f));
                if (currentIndexInSelection == 0 && selectedMBGraphNodeCount == 2)
                    Gizmos.DrawIcon(transform.position, "PreTexA@2x", false, new Color(1 - mbGraph.__nodeGizmoColor.r, 1 - mbGraph.__nodeGizmoColor.g, 1 - mbGraph.__nodeGizmoColor.b));
                else if (currentIndexInSelection == 1 && selectedMBGraphNodeCount == 2)
                    Gizmos.DrawIcon(transform.position, "PreTexB@2x", false, new Color(1 - mbGraph.__nodeGizmoColor.r, 1 - mbGraph.__nodeGizmoColor.g, 1 - mbGraph.__nodeGizmoColor.b));
            }
            else
                Gizmos.DrawSphere(transform.position, mbGraph.__nodeGizmoRadius);
        }
#endif
    }
}
