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
        public struct NodeInfo
        {
            public Node node;
            public System.Type type;
            public ObjectField obj;
            public FloatField flo;
            public List<Port> ports;
        }

        public GridBackground gridBackground;
        public List<NodeInfo> listNodes = new List<NodeInfo>();

        private Node outputNode;

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

        private class TempGridBackground : GridBackground { }
        void AddGridBackGround()
        {
            //Add grid background
            gridBackground = new TempGridBackground();

            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);

            //The expansion size is the same as the parent object
            this.StretchToParentSize();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> canAdapts = new List<Port>();

            foreach (Port port in ports.ToList())
            {
                if (startPort.portType == port.portType)
                {
                    canAdapts.Add(port);
                }
            }

            return canAdapts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            //evt.menu.AppendAction("Create", OnCreate, a => DropdownMenu.MenuAction.StatusFlags.Normal);
            evt.menu.AppendAction("Create/Variables", OnCreateVariables);
            evt.menu.AppendAction("Create/State", OnCreateState);
            evt.menu.AppendAction("Create/RuleSet", OnCreateRuleSet);
            evt.menu.AppendAction("Create/Logic", OnCreateLogic);
            evt.menu.AppendAction("Create/ShapeSet", OnCreateShapeSet);
            evt.menu.AppendAction("Create/Maths/Greater", OnCreateGreater);
        }

        private Node CreateNode(string title, float width, float height, StyleColor col)
        {
            Node n = new Node
            {
                title = title,
                style =
                {
                    width = width,
                    height = height
                }
            };

            n.extensionContainer.style.backgroundColor = col;
            //n.capabilities &= ~Capabilities.Deletable;
            n.capabilities |= Capabilities.Movable;
            n.capabilities |= Capabilities.Collapsible;
            n.capabilities |= Capabilities.Renamable;

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

                    //Debug.Log($"Trying to remove {n.title} at {removal}");

                    if (removal > -1)
                        listNodes.RemoveAt(removal);
                });

            return n;
        }

        private Port CreatePort(Node n, string name, Color color, Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            Port p = n.InstantiatePort(orientation, direction, capacity, type);
            p.portName = name;
            p.portColor = color;

            return p;
        }

        private Color defaultBackgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
        public Dictionary<int, Port> portDictionary = new Dictionary<int, Port>();

        public NodeInfo CreateNodeByData(StateMachineGraph.NodeData data)
        {
            System.Type typ = System.Type.GetType(data.type);

            Node n = CreateNode(data.name, data.w, data.h, defaultBackgroundColor);

            NodeInfo nI = new NodeInfo { node = n, obj = null, type = typ };

            if (typ != null)
            {
                if (data.type == "Greater")
                {
                    FloatField floatField = new FloatField(">", 100);
                    floatField.value = data.value2;
                    n.extensionContainer.Add(floatField);
                }
                else
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
                    fuzzyLogic.value = data.value;
                    fuzzyLogic.name = typ.Name + " Script";

                    n.extensionContainer.Add(fuzzyLogic);
                }
                
            }

            nI.ports = new List<Port>() { };
            
            foreach (StateMachineGraph.PortData port in data.ports)
            {
                System.Type portTyp = System.Type.GetType(port.type);

                Port p = CreatePort(n, port.name, port.color, port.orientation, port.direction, port.capacity, portTyp);
                portDictionary[port.id] = p;

                if (port.direction == Direction.Output)
                {
                    //if (typ != null) p.source = fuzzyLogic;
                    //else p.source = "Graph Output";
 
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

            n.SetPosition(new Rect(new Vector2(data.x, data.y), Vector2.zero));

            AddElement(n);
            listNodes.Add(nI);

            return nI;
        }

        public void ConnectPorts(NodeInfo nI, StateMachineGraph.NodeData data)
        {
            for (int i = 0; i < nI.ports.Count; i++)
            {
                StateMachineGraph.PortData port = data.ports[i];
                Port p = nI.ports[i];

                if (port.connections == null || p.connected) return; // Test if already connected or null, because connections can double up :)

                foreach (StateMachineGraph.PortConnector pC in port.connections)
                {
                    if (port.direction == Direction.Output)
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

        private void OnCreateVariables(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.red, 
                orientation = Orientation.Horizontal, direction = Direction.Output, 
                capacity = Port.Capacity.Multi, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Variables";
        }

        private void OnCreateState(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.cyan,
                orientation = Orientation.Horizontal, direction = Direction.Output,
                capacity = Port.Capacity.Multi, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New State";
        }

        private void OnCreateShapeSet(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 100,
                x = action.eventInfo.mousePosition.x,
                y = action.eventInfo.mousePosition.y,
                type = typeof(FuzzyStateMachine.Variable.FuzzyShapeSet).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.green,
                orientation = Orientation.Horizontal, direction = Direction.Output,
                capacity = Port.Capacity.Multi, type = typeof(FuzzyStateMachine.Variable.FuzzyShapeSet).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Shape Set";
        }

        private void OnCreateRuleSet(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 100,
                x = action.eventInfo.mousePosition.x,
                y = action.eventInfo.mousePosition.y,
                type = typeof(FuzzyRuleSet).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.blue,
                orientation = Orientation.Horizontal, direction = Direction.Output,
                capacity = Port.Capacity.Multi, type = typeof(FuzzyRuleSet).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Fuzzy RuleSet";
        }

        private void OnCreateLogic(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 300, h = 160, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(FuzzyLogic).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { 
                new StateMachineGraph.PortData { name = "In Variables", color = Color.red, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(FuzzyStateMachine.Variable.FuzzyVariable).FullName },
                new StateMachineGraph.PortData { name = "In Shape Set", color = Color.green, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(Variable.FuzzyShapeSet).FullName },
                new StateMachineGraph.PortData { name = "In Rule Set", color = Color.blue, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(FuzzyRuleSet).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = Orientation.Horizontal, direction = Direction.Output, capacity = Port.Capacity.Multi, type = typeof(StateMachineGraphWindow.FuzzyData).FullName }
            };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Logic";
        }

        private void OnCreateGreater(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200,
                h = 140,
                x = action.eventInfo.mousePosition.x,
                y = action.eventInfo.mousePosition.y,
                type = "Greater"
            };

            data.ports = new StateMachineGraph.PortData[] 
            {
                new StateMachineGraph.PortData { name = "In Data", color = Color.yellow, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(StateMachineGraphWindow.FuzzyData).FullName },
                new StateMachineGraph.PortData { name = "In State", color = Color.cyan, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Single, type = typeof(FuzzyStateMachine.States.StateMachineState).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = Orientation.Horizontal, direction = Direction.Output, capacity = Port.Capacity.Multi, type = typeof(StateMachineGraphWindow.FuzzyData).FullName },
            };

            NodeInfo info = CreateNodeByData(data);

            FloatField floatField = new FloatField(">", 100);

            info.flo = floatField;
            info.node.title = "Greater";
            info.node.extensionContainer.Add(floatField);

            info.node.RefreshExpandedState();
        }
    }
}
#endif