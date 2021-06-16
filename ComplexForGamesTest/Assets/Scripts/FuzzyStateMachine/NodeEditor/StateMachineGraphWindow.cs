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
        #region Fields

        /// <summary>
        /// The current created Node Editor window.
        /// </summary>
        public static StateMachineGraphWindow GraphWindow;

        /// <summary>
        /// The current running GraphView.
        /// </summary>
        public StateMachineGraphView GraphView { get; private set; }

        /// <summary>
        /// Loaded graph scriptable object.
        /// </summary>
        public StateMachineGraph Graph { get; private set; }

        // Created toolbar
        private Toolbar _toolBar;

        // Has the window been initialized already? (important for unneeded UIElement callbacks)
        private bool _isInitialized = false;

        #endregion

        /// <summary>
        /// Show/Create the GraphViewer through editor drop down.
        /// </summary>
        [MenuItem("Window/SME")]
        public static void ShowWindow()
        {
            // If the window doesnt exist, create a new one.
            if (StateMachineGraphWindow.GraphWindow == null)
            {
                GraphWindow = CreateWindow<StateMachineGraphWindow>();
                GraphWindow.titleContent = new GUIContent("Graph Window");
                GraphWindow.minSize = new Vector2(500, 400);
            }
            // Show it and Initialize the node editor.
            GraphWindow.Show();
            GraphWindow.Initialize();
        }

        /// <summary>
        /// Load the Node Graph from a graph scriptable object.
        /// </summary>
        /// <param name="a_graph"></param>
        public void LoadGraph(StateMachineGraph a_graph)
        {
            Graph = a_graph;
            GraphWindow.CreateToolbar();

            GraphView.listNodes.Clear();

            // If there is no graph output on this scriptable object, create one and insert it at index 0.
            if (a_graph.nodes.Count > 0 && a_graph.nodes[0].name != "Graph Output" || a_graph.nodes.Count == 0)
            {
                a_graph.nodes.Insert(0, new StateMachineGraph.NodeData { name = "Graph Output", w = 200, h = 100, type = "", value = null, x = 0, y = 0, ports = new StateMachineGraph.PortData[]
                    {
                        new StateMachineGraph.PortData { name = "In", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(StateMachineGraphView.FuzzyData).FullName }
                    }
                });
            }

            // Create all nodes by scriptable object node data
            foreach (StateMachineGraph.NodeData node in a_graph.nodes)
            {
                GraphView.CreateNodeByData(node);
            }

            // Connect all of the nodes ports
            for (int i = 0; i < a_graph.nodes.Count; i++)
            {
                StateMachineGraph.NodeData node = a_graph.nodes[i];
                StateMachineGraphView.NodeInfo nI = GraphView.listNodes[i];

                GraphView.ConnectPorts(nI, node);
            }

            // Clear the port dictionary, so when we load onto an already launched window, it wont double
            GraphView.portDictionary.Clear();

            // Focus the graph output node
            GraphView.listNodes[0].node.Focus();
        }

        /// <summary>
        /// Save node data to the desired scriptable object
        /// </summary>
        private void SaveData()
        {
            if (Graph != null && GraphView != null)
            {
                Graph.nodes.Clear();
                Graph.ports = 0;

                // Loop through each nodes and add them to the scriptable object
                foreach (StateMachineGraphView.NodeInfo node in GraphView.listNodes)
                {
                    if (node.node != null)
                        Graph.AddNode(node);
                }

                // Connect all the ports in the scriptable object
                Graph.ConnectPorts(GraphView.listNodes);

                // Clear unneeded data
                Graph.portDictionary.Clear();
            }
        }

        /// <summary>
        /// Create the top toolbar that has the Graph scriptable object and the Save button.
        /// </summary>
        public void CreateToolbar()
        {
            // If the toolbar exists, destroy it
            if (_toolBar != null)
            {
                _toolBar.Clear();
                this.rootVisualElement.Remove(_toolBar);
            }

            Toolbar toolbar = new Toolbar();

            // Graph Data Loader
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

            // Save data button
            toolbar.Add(new Button(() => SaveData()) { text = "Save Data" });

            this.rootVisualElement.Add(toolbar);

            // Finish
            _toolBar = toolbar;
        }

        /// <summary>
        /// Initializing the Graph and adding neccesary callbacks and items.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            _isInitialized = true;

            if (GraphView == null)
                GraphView = new StateMachineGraphView();

            GraphView.RegisterCallback<KeyDownEvent>(KeyDown);
            this.rootVisualElement.Add(GraphView);

            GraphView.Initialize();
            CreateToolbar();
        }

        /// <summary>
        /// Key Down unused
        /// </summary>
        /// <param name="evt">The event of pressing passed by Event Accessor</param>
        private void KeyDown(KeyDownEvent evt)
        {
        }
    }
}
#endif
