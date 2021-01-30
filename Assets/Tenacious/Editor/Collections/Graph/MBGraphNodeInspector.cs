using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

using System.Collections.Generic;

using TenaciousEditor.IMGUI;

using Tenacious.Collections;

namespace TenaciousEditor.Collections
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MBGraphNode))]
    public class MBGraphNodeInspector : InspectorBase
    {
        private int addNodeEdgeDirectionality = 0;
        private float addNodeEdgeWeight = default;
        private bool addNodeEdgeWeightIsDistance = true;
        private static string[] strEdgeDirectionalities = new string[] { "BiDirectional", "UniDirectional", "None" };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawControls();
        }

        protected void DrawControls()
        {
            MBGraphNode target = (MBGraphNode)this.target;

            EditorGUILayout.Space();

            List<MBGraphNode> selectedNodes = new List<MBGraphNode>();
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.GetComponent<MBGraphNode>() != null)
                    selectedNodes.Add(obj.GetComponent<MBGraphNode>());
            }

            if (selectedNodes.Count == 1)
            {
                // A single node is selected
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    if (GUILayout.Button("Add Node"))
                    {
                        GameObject addedNode = null;
                        if (addNodeEdgeDirectionality == 2)
                            addedNode = target.mbGraph.AddNode();
                        else
                            addedNode = target.mbGraph.AddNode(target, addNodeEdgeWeight, (EdgeDirectionality)addNodeEdgeDirectionality);

                        Selection.activeGameObject = addedNode;
                    }

                    addNodeEdgeDirectionality = EditorGUILayout.Popup("Edge Directionality", addNodeEdgeDirectionality, strEdgeDirectionalities);
                    addNodeEdgeWeightIsDistance = EditorGUILayout.Toggle("Weight == Distance", addNodeEdgeWeightIsDistance);
                    addNodeEdgeWeight = EditorGUILayout.FloatField("Edge Weight", addNodeEdgeWeight);
                }
                EditorGUILayout.EndVertical();
            }
            else if (selectedNodes.Count == 2)
            {
                // A pair of nodes (edge) has been selected
                GraphEdge<float>[] modifiableEdges = new GraphEdge<float>[2];
                modifiableEdges[0] = target.mbGraph.graph.GetEdge(selectedNodes[0].nodeId, selectedNodes[1].nodeId);
                modifiableEdges[1] = target.mbGraph.graph.GetEdge(selectedNodes[1].nodeId, selectedNodes[0].nodeId);

                for (int i = 0; i < modifiableEdges.Length; ++i)
                {
                    GraphEdge<float> edge = modifiableEdges[i];

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    var edgeLabelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
                    EditorGUILayout.LabelField(i == 0 ? "A -> B" : "B -> A", edgeLabelStyle);
                    DrawHorizontalLine();

                    int edgeDirectionalityInt = EditorGUILayout.Popup("Directionality", edge == null ? 2 : (int)edge.Directionality, strEdgeDirectionalities);
                    float edgeWeight = EditorGUILayout.FloatField("Edge Weight", edge == null ? 0 : edge.Weight);
                    bool edgeWeightIsDistance = GUILayout.Button("Weight == Distance");

                    EditorGUILayout.EndVertical();

                    if (
                        (edge != null || edgeDirectionalityInt != 2) &&
                        ((edge == null && edgeDirectionalityInt != 2) || edgeWeight != edge.Weight || edgeDirectionalityInt != (int)edge.Directionality)
                    )
                    {
                        if (edge != null && edgeDirectionalityInt == 2)
                            target.mbGraph.RemoveEdge(edge.FromId, edge.ToId);
                        else if (edgeDirectionalityInt != 2)
                        {
                            if (i == 0)
                                target.mbGraph.SetEdge(selectedNodes[0], selectedNodes[1], edgeWeight, (EdgeDirectionality)edgeDirectionalityInt);
                            else if (i == 1)
                                target.mbGraph.SetEdge(selectedNodes[1], selectedNodes[0], edgeWeight, (EdgeDirectionality)edgeDirectionalityInt);
                        }

                        EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
                        EditorUtility.SetDirty(target);
                    }

                    if (edgeWeightIsDistance)
                    {
                        edge.Weight = (selectedNodes[0].transform.position - selectedNodes[1].transform.position).magnitude;
                        EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
                        EditorUtility.SetDirty(target);
                    }

                    if (edgeDirectionalityInt == 0) break;
                }
            }
        }
    }
}
