using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

using TenaciousEditor;
using TenaciousEditor.IMGUI;

using Tenacious.Data;

namespace TenaciousEditor.Collections
{
    public class BTGraphEditorWindow : EditorWindowBase
    {
        private string EditorPreference_LastLoadedBehaviorTreeGraph;

        private BTGraphView graphView;
        private Label lblFilePath;

        [MenuItem("Tenacious/BehaviorTree/BehaviorTree Graph Editor")]
        static void MenuItem()
        {
            GetWindow<BTGraphEditorWindow>("BehaviorTree Graph Editor").Show();
        }

        private void Awake()
        {
            EditorPreference_LastLoadedBehaviorTreeGraph = PlayerSettings.productGUID.ToString() + "TenaciousEditor.Collections.BTGraphEditorWindow.LastLoadedBehaviorTreeGraph";
        }

        private void OnEnable()
        {
            InitGraphView();
            InitToolbar();

            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetPaths.EDITOR_GRAPHVIEW_DIR + "BTGraphEditorWindow.uss"));

            string lastLoaded = EditorPrefs.GetString(EditorPreference_LastLoadedBehaviorTreeGraph, null);
            if (!string.IsNullOrEmpty(lastLoaded))
            {
                LoadData(lastLoaded);
            }
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void OnInspectorUpdate()
        {
            if (graphView.RootNode != null && !graphView.Contains(graphView.RootNode))
                graphView.Add(graphView.RootNode);
        }

        private void InitGraphView()
        {
            graphView = new BTGraphView();
            graphView.StretchToParentSize();

            MiniMap miniMap = new MiniMap();
            miniMap.SetPosition(new Rect(10, 28, 200, 140));
            graphView.Add(miniMap);

            rootVisualElement.Insert(0, graphView);
        }

        private void InitToolbar()
        {
            Toolbar toolbar = new Toolbar();

            Button btnSaveAs = new Button(() => { SaveData(); });
            btnSaveAs.text = "Save As...";
            toolbar.Add(btnSaveAs);

            Button btnSave = new Button(() => 
            {
                SaveData(lblFilePath.text); 
            });
            btnSave.text = "Save";
            toolbar.Add(btnSave);

            lblFilePath = new Label();
            toolbar.Add(lblFilePath);

            toolbar.Add(new ToolbarSpacer() { flex = true });

            Button btnNew = new Button(() => { ResetGraphView(); });
            btnNew.text = "New";
            toolbar.Add(btnNew);

            Button btnLoad = new Button(() => { LoadData(); });
            btnLoad.text = "Load";
            toolbar.Add(btnLoad);

            rootVisualElement.Add(toolbar);
        }

        private void ResetGraphView()
        {
            rootVisualElement.Remove(graphView);
            InitGraphView();
            EditorPrefs.SetString(EditorPreference_LastLoadedBehaviorTreeGraph, "");
            lblFilePath.text = "";
        }

        private void LoadData(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = EditorUtility.OpenFilePanel(
                    "Select BehaviorTree Graph data file",
                    Application.dataPath,
                    "asset"
                );
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                filePath = filePath.Replace(Application.dataPath, "Assets");
                BTGraphData data = AssetDatabase.LoadAssetAtPath<BTGraphData>(filePath);

                if (data != null)
                {
                    ResetGraphView();

                    EditorPrefs.SetString(EditorPreference_LastLoadedBehaviorTreeGraph, filePath);   
                    lblFilePath.text = filePath;

                    foreach (BTGraphNodeData nodeData in data.nodes)
                    {
                        BTGraphNode restoredNode = graphView.CreateBTNode(nodeData);
                        restoredNode.Id = nodeData.id;
                        restoredNode.title = nodeData.title;
                        restoredNode.NodeType = nodeData.nodeType;
                        graphView.AddElement(restoredNode);
                    }

                    foreach (BTGraphEdgeData edgeData in data.edges)
                    {
                        BTGraphNode fromNode = edgeData.fromId == null ? null : (BTGraphNode)graphView.nodes.ToList().Find((n) => ((BTGraphNode)n).Id == edgeData.fromId);
                        BTGraphNode toNode = edgeData.toId == null ? null : (BTGraphNode)graphView.nodes.ToList().Find((n) => ((BTGraphNode)n).Id == edgeData.toId);

                        if (fromNode != null && toNode != null)
                        {
                            Edge restoredEdge = new Edge()
                            {
                                output = (Port)fromNode.outputContainer.ElementAt(0),
                                input = (Port)toNode.inputContainer.ElementAt(0)
                            };
                            restoredEdge.input.Connect(restoredEdge);
                            restoredEdge.output.Connect(restoredEdge);

                            graphView.AddElement(restoredEdge);
                        }
                    }
                }
                else
                {
                    EditorPrefs.SetString(EditorPreference_LastLoadedBehaviorTreeGraph, "");
                }
            }
        }

        private void SaveData(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = EditorUtility.SaveFilePanel(
                    "Save BehaviorTree Graph data file",
                    Application.dataPath,
                    "New BehaviorTree Graph",
                    "asset"
                );
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                BTGraphData data = ScriptableObject.CreateInstance<BTGraphData>();
                foreach (Edge edge in graphView.edges.ToList())
                {
                    BTGraphNode outputNode = edge.output.node as BTGraphNode;
                    BTGraphNode inputNode = edge.input.node as BTGraphNode;

                    data.edges.Add(new BTGraphEdgeData()
                    {
                        fromId = outputNode.Id,
                        toId = inputNode.Id
                    });
                }

                foreach (BTGraphNode node in graphView.nodes.ToList())
                {
                    data.nodes.Add(new BTGraphNodeData()
                    {
                        id = node.Id,
                        isRootNode = node.IsRootNode,
                        title = node.title,
                        nodeType = node.NodeType,
                        position = node.GetPosition().position
                    });
                }

                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetDatabase.CreateAsset(data, filePath);
                AssetDatabase.SaveAssets();

                EditorPrefs.SetString(EditorPreference_LastLoadedBehaviorTreeGraph, filePath);
                lblFilePath.text = filePath;
            }
        }
    }
}
