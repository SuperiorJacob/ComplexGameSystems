#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FuzzyStateMachine
{
    public class StateMachineGraphView : GraphView
    {
        /// <summary>
        /// Node info used by the GraphWindow, realtime references not saveable.
        /// </summary>
        public struct NodeInfo
        {
            public Node node; // The node.
            public System.Type type; // ObjectFields type.
            public ObjectField obj; // ObjectField itself.
            public FloatField flo; // FloatField if used.
            public string typeString; // Type but as a string, taken from save.
            public List<Port> ports; // List of ports generated by port connector.
        }

        /// <summary>
        /// Temporary struct used for data connection type.
        /// </summary>
        public struct FuzzyData { };

        #region Fields

        /// <summary>
        /// A list of all the current nodes in the graph.
        /// </summary>
        public List<NodeInfo> listNodes = new List<NodeInfo>();

        /// <summary>
        /// Dictionary used to store all of the current loaded ports in the graph for lookup capabilities.
        /// </summary>
        public Dictionary<int, Port> portDictionary = new Dictionary<int, Port>();

        // Default background colour in our graph nodes.
        private Color defaultBackgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);

        // Grid background of the graph.
        private GridBackground gridBackground;

        #endregion

        // Apparently, the default grid class is weird so we use an inherited version.
        private class TempGridBackground : GridBackground { }

        /// <summary>
        /// Creates the GraphViewer, and initializes all required window UI information.
        /// </summary>
        public void Initialize()
        {
            listNodes.Clear();
            AddGridBackGround();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }

        /// <summary>
        /// Creates the grid background to be used by the Nodes.
        /// </summary>
        void AddGridBackGround()
        {
            //Add grid background
            gridBackground = new TempGridBackground();

            gridBackground.name = "GridBackground";
            // Insert gridbackground to the 0 slot of the graph view UI element
            Insert(0, gridBackground);

            //The expansion size is the same as the parent object
            this.StretchToParentSize();
        }

        #region Node & Port Creation

        /// <summary>
        /// Creates a Node ready for the GraphViewer.
        /// </summary>
        /// <param name="a_title">Name of the node</param>
        /// <param name="a_width">Node style width</param>
        /// <param name="a_height">Node style height</param>
        /// <param name="a_col">Node background colour</param>
        /// <returns>Created Node ready to be added to the Graph</returns>
        private Node CreateNode(string a_title, float a_width, float a_height, StyleColor a_col)
        {
            Node n = new Node
            {
                title = a_title,
                style =
                {
                    width = a_width,
                    height = a_height
                }
            };

            n.extensionContainer.style.backgroundColor = a_col;
            n.capabilities |= Capabilities.Movable;
            n.capabilities |= Capabilities.Collapsible;
            n.capabilities |= Capabilities.Renamable;

            // Deletion event, used for when the node is deleted.
            n.RegisterCallback<DetachFromPanelEvent>(
                obj => {
                    int removal = -1;

                    for (int i = 0; i < listNodes.Count; i++)
                    {
                        NodeInfo nI = listNodes[i];

                        if (nI.node == n)
                        {
                            removal = i;
                            break;
                        }
                    }

                    if (removal > -1)
                        listNodes.RemoveAt(removal);
                });

            return n;
        }

        /// <summary>
        /// Creates a Port ready to be used by the GraphViewer.
        /// </summary>
        /// <param name="a_node">The node to add the port</param>
        /// <param name="a_name">Port name</param>
        /// <param name="a_color">The ports color</param>
        /// <param name="a_orientation">The ports orientation</param>
        /// <param name="a_direction">The ports direction IN or OUT</param>
        /// <param name="a_capacity">Ports capacity of multi or single</param>
        /// <param name="a_type">The port type used to tell if can connect or not</param>
        /// <returns>Returns the created port</returns>
        private Port CreatePort(Node a_node, string a_name, Color a_color, Orientation a_orientation, Direction a_direction, Port.Capacity a_capacity, System.Type a_type)
        {
            Port p = a_node.InstantiatePort(a_orientation, a_direction, a_capacity, a_type);
            p.portName = a_name;
            p.portColor = a_color;

            return p;
        }

        /// <summary>
        /// Returns a list of ports capable of connecting (used internally by GraphView)
        /// </summary>
        /// <param name="a_portToCompare">What port are we checking for connectability?</param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port a_portToCompare, NodeAdapter a_nodeAdapter = default)
        {
            List<Port> canAdapts = new List<Port>();

            foreach (Port port in ports.ToList())
            {
                if (a_portToCompare.portType == port.portType) // If the types are the same, they can connect.
                {
                    canAdapts.Add(port);
                }
            }

            return canAdapts;
        }

        public NodeInfo CreateNodeByData(StateMachineGraph.NodeData a_data)
        {
            System.Type typ = System.Type.GetType(a_data.type);

            Node n = CreateNode(a_data.name, a_data.w, a_data.h, defaultBackgroundColor);

            NodeInfo nI = new NodeInfo { node = n, obj = null, type = typ, typeString = a_data.type };

            if (a_data.type == "Greater" || a_data.type == "Lesser" || a_data.type == "Equal")
            {
                FloatField floatField = new FloatField(">", 100);
                floatField.value = a_data.value2;
                n.extensionContainer.Add(floatField);

                nI.flo = floatField;
            }
            else
            {
                if (typ != null)
                {
                    ObjectField fuzzyLogic = new ObjectField();

                    fuzzyLogic.objectType = typ.BaseType == typeof(ScriptableObject) ? typ : typeof(MonoScript);

                    fuzzyLogic.RegisterCallback<ChangeEvent<Object>>(
                        obj =>
                        {
                            ObjectField vObj = (ObjectField)obj.target;

                            System.Type c = vObj.value.GetType();

                            if (c == typeof(MonoScript))
                            {
                                c = ((MonoScript)vObj.value).GetClass();
                            }

                            if (vObj.value != null && (c == typ || c != null && c.BaseType == typ))
                            {
                                n.title = vObj.value.name;
                            }
                            else
                            {
                                // Throw exception for failure.
                                if (vObj.value != null && c != null)
                                {
                                    throw new System.Exception($"{typ.Name} is not the same as {(c != null ? c.Name : "null")} or {(c != null ? c.BaseType.Name : "")}!");
                                }

                                n.title = "New " + typ.Name;
                                vObj.value = null;
                            }

                        }
                    );

                    nI.obj = fuzzyLogic;
                    fuzzyLogic.value = a_data.value;
                    fuzzyLogic.name = typ.Name + " Script";

                    n.extensionContainer.Add(fuzzyLogic);
                }

            }

            nI.ports = new List<Port>() { };
            
            foreach (StateMachineGraph.PortData port in a_data.ports)
            {
                System.Type portTyp = System.Type.GetType(port.type);

                Port p = CreatePort(n, port.name, port.color, (Orientation)port.orientation, (Direction)port.direction, (Port.Capacity)port.capacity, portTyp);
                portDictionary[port.id] = p;

                if (port.direction == (int)Direction.Output)
                {
                    n.outputContainer.Add(p);
                }
                else
                {
                    n.inputContainer.Add(p);
                }

                nI.ports.Add(p);
            }

            n.RefreshExpandedState();
            n.RefreshPorts();

            n.SetPosition(new Rect(new Vector2(a_data.x, a_data.y), Vector2.zero));

            AddElement(n);
            listNodes.Add(nI);

            return nI;
        }

        /// <summary>
        /// Connects all the ports to the respected graph node port.
        /// </summary>
        /// <param name="a_nodeInfo">Target graph node data</param>
        /// <param name="a_data">Saved target node data</param>
        public void ConnectPorts(NodeInfo a_nodeInfo, StateMachineGraph.NodeData a_data)
        {
            for (int i = 0; i < a_nodeInfo.ports.Count; i++)
            {
                StateMachineGraph.PortData port = a_data.ports[i];
                Port p = a_nodeInfo.ports[i];

                if (port.connections == null || p.connected) return; // Test if already connected or null, because connections can double up :)

                // Connecting respected ports on the graph and visualise it.
                foreach (StateMachineGraph.PortConnector pC in port.connections)
                {
                    if (port.direction == (int)Direction.Output)
                    {
                        AddElement(p.ConnectTo(portDictionary[pC.output]));
                    }
                    else
                    {
                        AddElement(p.ConnectTo(portDictionary[pC.input]));
                    }
                }
            }
        }

        #endregion

        #region Drop Down Menu

        /// <summary>
        /// Used to create the dropdown menu!
        /// </summary>
        /// <param name="a_evt">Dropdown menu population event.</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent a_evt)
        {
            base.BuildContextualMenu(a_evt);
            a_evt.menu.AppendAction("Create/Inputs/Variables", OnCreateVariables);
            a_evt.menu.AppendAction("Create/Inputs/State", OnCreateState);
            a_evt.menu.AppendAction("Create/Inputs/RuleSet", OnCreateRuleSet);
            a_evt.menu.AppendAction("Create/Inputs/ShapeSet", OnCreateShapeSet);

            a_evt.menu.AppendAction("Create/Data/Logic", OnCreateLogic);
            a_evt.menu.AppendAction("Create/Data/HasState", OnCreateHasState);

            a_evt.menu.AppendAction("Create/Maths/Greater", OnCreateGreater);
            a_evt.menu.AppendAction("Create/Maths/Lesser", OnCreateLesser);
            a_evt.menu.AppendAction("Create/Maths/Equal", OnCreateEqual);
        }

        /// <summary>
        /// HasState Node : Checks if the data of two data inputs has a state, whichever one has a state or whichever one has the higher deffuz will pass through.
        /// </summary>
        private void OnCreateHasState(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 140,
                x = a_action.eventInfo.mousePosition.x,
                y = a_action.eventInfo.mousePosition.y,
                type = "HasState",
                name = "Has State",
                value2 = -1
            };

            data.ports = new StateMachineGraph.PortData[]
            {
                new StateMachineGraph.PortData { name = "In Data", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyData).FullName },
                new StateMachineGraph.PortData { name = "In Data 2", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyData).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output, capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyData).FullName },
            };

            CreateNodeByData(data);
        }

        /// <summary>
        /// Variable Node : Takes in a variable scriptable object, and outputs FuzzyVariables.
        /// </summary>
        private void OnCreateVariables(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = a_action.eventInfo.mousePosition.x, y = a_action.eventInfo.mousePosition.y, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.red, 
                orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output, 
                capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Variables";
        }

        /// <summary>
        /// State Node : Takes in a state script, and outputs FuzzyState.
        /// </summary>
        private void OnCreateState(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = a_action.eventInfo.mousePosition.x, y = a_action.eventInfo.mousePosition.y, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.cyan,
                orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output,
                capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New State";
        }

        /// <summary>
        /// ShapeSet Node : Takes in a shapeset scriptable object, and outputs FuzzyShapeSet.
        /// </summary>
        private void OnCreateShapeSet(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 100,
                x = a_action.eventInfo.mousePosition.x,
                y = a_action.eventInfo.mousePosition.y,
                type = typeof(FuzzyStateMachine.Variable.FuzzyShapeSet).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.green,
                orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output,
                capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyStateMachine.Variable.FuzzyShapeSet).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Shape Set";
        }

        /// <summary>
        /// RuleSet Node : Takes in a ruleset script, and outputs FuzzyRuleSet.
        /// </summary>
        private void OnCreateRuleSet(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 100,
                x = a_action.eventInfo.mousePosition.x,
                y = a_action.eventInfo.mousePosition.y,
                type = typeof(FuzzyRuleSet).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.blue,
                orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output,
                capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyRuleSet).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Fuzzy RuleSet";
        }

        /// <summary>
        /// Logic Node : Loads a logic script, using variable, shape and ruleset inputs to output FuzzyData.
        /// </summary>
        private void OnCreateLogic(DropdownMenuAction a_action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 300, h = 160, x = a_action.eventInfo.mousePosition.x, y = a_action.eventInfo.mousePosition.y, type = typeof(FuzzyLogic).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { 
                new StateMachineGraph.PortData { name = "In Variables", color = Color.red, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName },
                new StateMachineGraph.PortData { name = "In Shape Set", color = Color.green, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(Variable.FuzzyShapeSet).FullName },
                new StateMachineGraph.PortData { name = "In Rule Set", color = Color.blue, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyRuleSet).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output, capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyData).FullName }
            };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Logic";
        }

        /// <summary>
        /// Maths Node : Takes in data, a state, a maths statement and outputs new data if the operation was successful.
        /// </summary>
        private void OnCreateMaths(DropdownMenuAction a_action, string a_type)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 140,
                x = a_action.eventInfo.mousePosition.x,
                y = a_action.eventInfo.mousePosition.y,
                type = a_type,
                name = a_type + " " + (a_type == "Equal" ? "To" : "Than"),
                value2 = -1
            };

            data.ports = new StateMachineGraph.PortData[] 
            {
                new StateMachineGraph.PortData { name = "In Data", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyData).FullName },
                new StateMachineGraph.PortData { name = "In State", color = Color.cyan, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Input, capacity = (int)Port.Capacity.Single, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = (int)Orientation.Horizontal, direction = (int)Direction.Output, capacity = (int)Port.Capacity.Multi, type = typeof(FuzzyData).FullName },
            };

            NodeInfo nI = CreateNodeByData(data);
            nI.flo.label = (a_type == "Equal" ? "==" : (a_type == "Greater" ? ">" : "<"));
        }

        /// <summary>
        /// Greater Node : The maths node, checks if number is greater than the deffuzified value.
        /// </summary>
        private void OnCreateGreater(DropdownMenuAction a_action)
        {
            OnCreateMaths(a_action, "Greater");
        }

        /// <summary>
        /// Lesser Node : The maths node, checks if number is lesser than the deffuzified value.
        /// </summary>
        private void OnCreateLesser(DropdownMenuAction a_action)
        {
            OnCreateMaths(a_action, "Lesser");
        }

        /// <summary>
        /// Greater Node : The maths node, checks if number is equal to the deffuzified value (which is very unlikely unless 0% or 100%)
        /// </summary>
        private void OnCreateEqual(DropdownMenuAction a_action)
        {
            OnCreateMaths(a_action, "Equal");
        }

        #endregion
    }
}
#endif
