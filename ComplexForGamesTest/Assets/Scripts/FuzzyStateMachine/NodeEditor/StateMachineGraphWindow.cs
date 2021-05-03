#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FuzzyStateMachine
{
    public class StateMachineGraphWindow : EditorWindow
    {
        public static StateMachineGraphWindow GraphWindow;

        public StateMachineGraphView GraphView { get; private set; }
        public StateMachineGraph Graph { get; private set; }

        private Toolbar toolBar;

        [MenuItem("Window/SME")]
        public static void ShowWindow()
        {
            Debug.Log("please work");
            if (StateMachineGraphWindow.GraphWindow == null)
            {
                GraphWindow = CreateWindow<StateMachineGraphWindow>();
                GraphWindow.titleContent = new GUIContent("Graph Window");
                GraphWindow.minSize = new Vector2(500, 400);
            }
            GraphWindow.Show();
            GraphWindow.Initialize();
        }

        public void LoadGraph(StateMachineGraph a_graph)
        {
            Graph = a_graph;
            GraphWindow.CreateToolbar();

            GraphView.listNodes.Clear();

            if (a_graph.nodes.Count > 0 && a_graph.nodes[0].name != "Graph Output" || a_graph.nodes.Count == 0)
            {
                a_graph.nodes.Insert(0, new StateMachineGraph.NodeData { name = "Graph Output", w = 200, h = 100, type = "", value = null, x = 0, y = 0, ports = new StateMachineGraph.PortData[]
                    {
                        new StateMachineGraph.PortData { name = "In", color = Color.yellow, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(StateMachineGraphWindow.FuzzyData).FullName }
                    }
                });
            }

            foreach (StateMachineGraph.NodeData node in a_graph.nodes)
            {
                GraphView.CreateNodeByData(node);
            }

            for (int i = 0; i < a_graph.nodes.Count; i++)
            {
                StateMachineGraph.NodeData node = a_graph.nodes[i];
                StateMachineGraphView.NodeInfo nI = GraphView.listNodes[i];

                GraphView.ConnectPorts(nI, node);
            }

            GraphView.portDictionary.Clear();

            GraphView.listNodes[0].node.Focus();

            // load graph
        }

        bool isInitialize = false;

        public class FuzzyData { };

        public void CreateToolbar()
        {
            if (toolBar != null)
            {
                toolBar.Clear();
                this.rootVisualElement.Remove(toolBar);
            }

            Toolbar toolbar = new Toolbar();

            ObjectField graphObject = new ObjectField { name = "Graph Object", objectType = typeof(StateMachineGraph) };
            graphObject.value = Graph;

            graphObject.RegisterCallback<ChangeEvent<UnityEngine.Object>>(
                obj =>
                {
                    ObjectField vObj = (ObjectField)obj.target;
                    StateMachineGraph graph = (StateMachineGraph)vObj.value;

                    if (graph != null)
                    {
                        LoadGraph(graph);
                    }
                }
            );
            toolbar.Add(graphObject);

            toolbar.Add(new Button(() => SaveData()) { text = "Save Data" });

            this.rootVisualElement.Add(toolbar);

            toolBar = toolbar;
        }

        public void Initialize()
        {
            if (isInitialize) return;

            isInitialize = true;

            if (GraphView == null)
            {
                GraphView = new StateMachineGraphView();
            }

            GraphView.RegisterCallback<KeyDownEvent>(KeyDown);
            this.rootVisualElement.Add(GraphView);

            GraphView.Initialize();
            CreateToolbar();
        }

        private void KeyDown(KeyDownEvent evt)
        {

        }

        // Make Choice Visualiser

        private void SaveData()
        {
            if (Graph != null && GraphView != null)
            {
                Graph.nodes.Clear();
                Graph.ports = 0;

                foreach (StateMachineGraphView.NodeInfo node in GraphView.listNodes)
                {
                    if (node.node != null)
                        Graph.AddNode(node);
                }

                Graph.ConnectPorts(GraphView.listNodes);

                Graph.portDictionary.Clear();
            }
        }
    }
}
#endif