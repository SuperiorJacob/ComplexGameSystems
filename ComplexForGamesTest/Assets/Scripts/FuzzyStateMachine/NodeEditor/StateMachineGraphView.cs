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
            public ObjectField obj;
            public List<Port> ports;
        }

        public GridBackground gridBackground;
        public List<NodeInfo> listNodes = new List<NodeInfo>();

        public void Initialize()
        {
            listNodes.Clear();
            AddGridBackGround();

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            Node outPutNode = CreateNode("Output", 200, 100, defaultBackgroundColor);
            Port inputPort = CreatePort(outPutNode, "In", Color.yellow, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Object));
            
            outPutNode.inputContainer.Add(inputPort);

            outPutNode.SetPosition(new Rect(Vector2.zero, Vector2.zero));

            AddElement(outPutNode);
        }

        private class TempGridBackground : GridBackground { }
        void AddGridBackGround()
        {
            //Add grid background
            gridBackground = new TempGridBackground();

            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);

            //Set the background zoom range
            this.SetupZoom(0.25f, 2.0f);

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
            evt.menu.AppendAction("Create/Logic", OnCreateLogic);
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
            n.capabilities &= ~Capabilities.Deletable;
            n.capabilities |= Capabilities.Movable;
            n.capabilities |= Capabilities.Collapsible;
            n.capabilities |= Capabilities.Renamable;

            return n;
        }

        private Port CreatePort(Node n, string name, Color color, Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
        {
            Port p = n.InstantiatePort(orientation, direction, capacity, type);
            p.portName = name;
            p.portColor = color;

            return p;
        }

        private Color defaultBackgroundColor = new Color(0.6f, 0.24f, 0.24f, 0.8f);

        public NodeInfo CreateNodeByData(StateMachineGraph.NodeData data)
        {
            System.Type typ = System.Type.GetType(data.type);

            Node n = CreateNode(typ.Name, data.w, data.h, defaultBackgroundColor);

            ObjectField fuzzyLogic = new ObjectField();

            fuzzyLogic.objectType = typ;

            fuzzyLogic.RegisterCallback<ChangeEvent<Object>>(
                obj =>
                {
                    ObjectField vObj = (ObjectField)obj.target;
                    n.title = vObj.value.ToString();
                }
            );

            NodeInfo nI = new NodeInfo { node = n, obj = fuzzyLogic };

            fuzzyLogic.name = typ + " Script";
            
            foreach (StateMachineGraph.PortData port in data.ports)
            {
                System.Type portTyp = System.Type.GetType(port.type);

                Port p = CreatePort(n, port.name, port.color, port.orientation, port.direction, port.capacity, portTyp);

                if (port.direction == Direction.Output)
                {
                    p.source = fuzzyLogic;
                    n.outputContainer.Add(p);
                }
                else
                {
                    n.inputContainer.Add(p);
                }

                nI.ports = new List<Port>() { p };
            }

            n.extensionContainer.Add(fuzzyLogic);

            n.RefreshExpandedState();
            n.RefreshPorts();

            n.SetPosition(new Rect(new Vector2(data.x, data.y), Vector2.zero));

            AddElement(n);
            listNodes.Add(nI);

            return nI;
        }

        private void OnCreateVariables(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(StateMachineGraphWindow.FuzzyVariable).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.red, 
                orientation = Orientation.Horizontal, direction = Direction.Output, 
                capacity = Port.Capacity.Multi, type = typeof(StateMachineGraphWindow.FuzzyVariable).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Variables";
            info.obj.objectType = typeof(StateMachineGraphWindow.FuzzyVariable);
        }

        private void OnCreateState(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 200, h = 100, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(FuzzyStateMachine.StateMachineState).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { new StateMachineGraph.PortData { name = "Out", color = Color.cyan,
                orientation = Orientation.Horizontal, direction = Direction.Output,
                capacity = Port.Capacity.Multi, type = typeof(FuzzyStateMachine.StateMachineState).FullName } };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New State";
            info.obj.objectType = typeof(FuzzyStateMachine.StateMachineState);
        }

        private void OnCreateLogic(DropdownMenuAction action)
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData
            {
                w = 300, h = 140, x = action.eventInfo.mousePosition.x, y = action.eventInfo.mousePosition.y, type = typeof(StateMachineGraphWindow.FuzzyLogic).FullName
            };

            data.ports = new StateMachineGraph.PortData[] { 
                new StateMachineGraph.PortData { name = "In Variables", color = Color.red, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Multi, type = typeof(StateMachineGraphWindow.FuzzyVariable).FullName },
                new StateMachineGraph.PortData { name = "In States", color = Color.cyan, orientation = Orientation.Horizontal, direction = Direction.Input, capacity = Port.Capacity.Multi, type = typeof(FuzzyStateMachine.StateMachineState).FullName },
                new StateMachineGraph.PortData { name = "Out Data", color = Color.yellow, orientation = Orientation.Horizontal, direction = Direction.Output, capacity = Port.Capacity.Multi, type = typeof(StateMachineGraphWindow.FuzzyData).FullName }
            };

            NodeInfo info = CreateNodeByData(data);
            info.node.title = "New Logic";
            info.obj.objectType = typeof(StateMachineGraphWindow.FuzzyLogic);
        }
    }
}
#endif