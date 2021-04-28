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

        private string _fileName = "New State Machine";

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
            foreach (StateMachineGraph.NodeData node in a_graph.nodes)
            {
                GraphView.CreateNodeByData(node);
            }

            // load graph
        }

        bool isInitialize = false;

        public class FuzzyVariable { };
        public class FuzzyState { };
        public class FuzzyData { };

        public class FuzzyLogic
        {
            public string name;

            FuzzyLogic(string a_name)
            {
                name = a_name;
            }

            public virtual FuzzyState Calculate(FuzzyVariable[] fuzzyVariables, FuzzyState[] fuzzyStates)
            {
                return fuzzyStates[0];
            }
        }

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

                foreach (StateMachineGraphView.NodeInfo node in GraphView.listNodes)
                {
                    Graph.AddNode(node);
                }
            }
        }
    }
}
#endif