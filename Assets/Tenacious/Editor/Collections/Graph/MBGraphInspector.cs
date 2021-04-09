using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

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
            Selection.objects = graphObjects.ToArray();
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

            if (target.graph.Count == 0)
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    GUILayout.Label("Generation Options", EditorStyles.boldLabel);

                    target.__generationType = (MBGraph.EGenerationType)EditorGUILayout.EnumPopup("Generation Type", target.__generationType);

                    if (target.__generationType == MBGraph.EGenerationType.Grid)
                    {
                        target.__gridType = (MBGraph.EGridType)EditorGUILayout.EnumPopup("Grid Type", target.__gridType);
                        target.__gridColumns = EditorGUILayout.IntField("Number of Columns", target.__gridColumns);
                        target.__gridRows = EditorGUILayout.IntField("Number of Rows", target.__gridRows);
                        target.__gridCellSize = EditorGUILayout.FloatField("Grid Cell Size", target.__gridCellSize);

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            target.__detectCollisions = EditorGUILayout.Toggle("Detect Collisions", target.__detectCollisions);
                            if (target.__detectCollisions)
                            {
                                target.__collisionRadius = EditorGUILayout.FloatField("Collision Radius", target.__collisionRadius);

                                target.__useTagsToIgnoreCollisions = EditorGUILayout.Toggle("Use Tags To Ignore Collisions", target.__useTagsToIgnoreCollisions);

                                EditorGUI.BeginChangeCheck();
                                EditorGUI.indentLevel++; EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(serializedObject.FindProperty("__collisionTags"), new GUIContent("Ignored Collision Tags"));
                                EditorGUI.indentLevel--; EditorGUI.indentLevel--;
                                if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
                            }
                        }
                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("Generate Graph"))
                            GenerateGrid(target);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                if (GUILayout.Button("Clear Graph"))
                    ClearNodes(target);
            }
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
            target.graph.isDirty = true;
            foreach (GraphEdge<float> edge in target.graph.Edges())
            {
                edge.Weight = (target.graph[edge.FromId].Data.transform.position - target.graph[edge.ToId].Data.transform.position).magnitude;
            }

            EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
            EditorUtility.SetDirty(target);
        }

        private void ClearNodes(MBGraph target)
        {
            for (int i = target.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(target.transform.GetChild(i).gameObject);
        }

        private void GenerateGrid(MBGraph target)
        {
            ClearNodes(target);

            float width = target.__gridColumns * target.__gridCellSize;
            float height = target.__gridRows * target.__gridCellSize;
            Vector3 position = new Vector3(target.transform.position.x - (width / 2), target.transform.position.y, target.transform.position.z - (height / 2));

            // 1st pass: generate nodes
            MBGraphNode[,] nodeGrid = new MBGraphNode[target.__gridRows, target.__gridColumns];
            for (int r = 0; r < target.__gridRows; ++r)
            {
                float startingX = position.x;
                for (int c = 0; c < target.__gridColumns; ++c)
                {
                    bool shouldAddNode = true;
                    if (target.__detectCollisions)
                    {
                        List<Collider> colliders = FilterColliders(target, Physics.OverlapSphere(position, target.__collisionRadius));
                        shouldAddNode = colliders.Count == 0;
                    }

                    if (shouldAddNode)
                    {
                        GameObject addedNodeObj = target.AddNode();
                        addedNodeObj.transform.position = position;
                        nodeGrid[r, c] = addedNodeObj.GetComponent<MBGraphNode>();
                    }

                    position = new Vector3(position.x + target.__gridCellSize, position.y, position.z);
                }
                position = new Vector3(startingX, position.y, position.z + target.__gridCellSize);
            }

            // 2nd pass: generate edges
            for (int r = 0; r < target.__gridRows; ++r)
            {
                for (int c = 0; c < target.__gridColumns; ++c)
                {
                    if (nodeGrid[r, c] == null) continue;

                    // NWES edges
                    if (c > 0 && nodeGrid[r, c - 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r, c - 1]);
                    if (c < target.__gridColumns - 1 && nodeGrid[r, c + 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r, c + 1]);
                    if (r > 0 && nodeGrid[r - 1, c] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r - 1, c]);
                    if (r < target.__gridRows - 1 && nodeGrid[r + 1, c] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r + 1, c]);

                    // diagonal edges
                    if (target.__gridType == MBGraph.EGridType.EightWay)
                    {
                        if (r > 0 && c > 0 && nodeGrid[r - 1, c - 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r - 1, c - 1]);
                        if (r < target.__gridRows - 1 && c < target.__gridColumns - 1 && nodeGrid[r + 1, c + 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r + 1, c + 1]);
                        if (r > 0 && c < target.__gridColumns - 1 && nodeGrid[r - 1, c + 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r - 1, c + 1]);
                        if (r < target.__gridRows - 1 && c > 0 && nodeGrid[r + 1, c - 1] != null) ConnectGridNodes(target, nodeGrid[r, c], nodeGrid[r + 1, c - 1]);
                    }
                }
            }
        }

        private void ConnectGridNodes(MBGraph target, MBGraphNode nodeA, MBGraphNode nodeB)
        {
            Vector3 direction = nodeB.transform.position - nodeA.transform.position;
            bool shouldConnect = true;
            if (target.__detectCollisions)
            {
                List<RaycastHit> hits = FilterRaycastHits(target, Physics.SphereCastAll(nodeA.transform.position, 0.25f, direction, direction.magnitude));
                shouldConnect = hits.Count == 0;
            }

            if (shouldConnect)
                target.SetEdge(nodeA, nodeB);
        }

        private List<Collider> FilterColliders(MBGraph target, Collider[] colliders)
        {
            List<Collider> colliderList = new List<Collider>();
            foreach (Collider c in colliders)
            {
                bool collidedWithTag = target.__collisionTags != null && target.__collisionTags.Contains(c.tag);
                bool ignoreCollision = target.__useTagsToIgnoreCollisions ? false : true;
                if (collidedWithTag)
                {
                    if (target.__useTagsToIgnoreCollisions)
                        ignoreCollision = true;
                    else
                        ignoreCollision = false;
                }

                if (!c.isTrigger && !ignoreCollision)
                    colliderList.Add(c);
            }

            return colliderList;
        }

        private List<RaycastHit> FilterRaycastHits(MBGraph target, RaycastHit[] hits)
        {
            List<RaycastHit> hitList = new List<RaycastHit>();
            foreach (RaycastHit hit in hits)
            {
                bool collidedWithTag = target.__collisionTags != null && target.__collisionTags.Contains(hit.collider.tag);
                bool ignoreCollision = target.__useTagsToIgnoreCollisions ? false : true;
                if (collidedWithTag)
                {
                    if (target.__useTagsToIgnoreCollisions)
                        ignoreCollision = true;
                    else
                        ignoreCollision = false;
                }

                if (!hit.collider.isTrigger && !ignoreCollision)
                    hitList.Add(hit);
            }

            return hitList;
        }
    }
}
