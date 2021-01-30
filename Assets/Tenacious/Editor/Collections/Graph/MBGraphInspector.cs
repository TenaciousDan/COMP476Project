using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

using System.Collections.Generic;

using TenaciousEditor.IMGUI;
using TenaciousEditor.Utilities;

using Tenacious.Collections;

namespace TenaciousEditor.Collections
{
    [CustomEditor(typeof(MBGraph))]
    public class MBGraphInspector : InspectorBase
    {
        [MenuItem("GameObject/Tenacious/General/DiGraph", false)]
        static void MenuItem()
        {
            List<GameObject> graphObjects = EditorUtil.CreateGameObject("Graph", typeof(MBGraph));
            List<GameObject> createdGraphNodeObjects = new List<GameObject>();
            foreach (GameObject obj in graphObjects)
            {
                createdGraphNodeObjects.Add(obj.GetComponent<MBGraph>().AddNode());
            }

            Selection.objects = createdGraphNodeObjects.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawControls();
        }

        protected void DrawControls()
        {
            MBGraph target = (MBGraph)this.target;

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            {
                if (GUILayout.Button("Add Node"))
                    AddNodeClick(target);

                if (GUILayout.Button("Set All Weights == Distances"))
                    SetAllWeightsToDistancesClick(target);
            }
            EditorGUILayout.EndVertical();
        }

        private void AddNodeClick(MBGraph target)
        {
            GameObject addedNode = target.AddNode();
            Selection.activeGameObject = addedNode;

            EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
            EditorUtility.SetDirty(target);
        }

        private void SetAllWeightsToDistancesClick(MBGraph target)
        {
            foreach (GraphEdge<float> edge in target.graph.Edges())
            {
                edge.Weight = (target.graph[edge.FromId].Data.transform.position - target.graph[edge.ToId].Data.transform.position).magnitude;
            }

            EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
            EditorUtility.SetDirty(target);
        }
    }
}
