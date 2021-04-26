using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class StateMachineGraphView : GraphView
{
    public GridBackground gridBackground;

    public void Initialize()
    {
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

    private Node CreateNode(string title, int width, int height, StyleColor col)
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

    private Port CreatePort(Node n, string name, Color color,  Orientation orientation, Direction direction, Port.Capacity capacity, System.Type type)
    {
        Port p = n.InstantiatePort(orientation, direction, capacity, type);
        p.portName = name;
        p.portColor = color;

        return p;
    }

    private Color defaultBackgroundColor = new Color(0.6f, 0.24f, 0.24f, 0.8f);

    private void OnCreateVariables(DropdownMenuAction action)
    {
        Node n = CreateNode("New Variables", 200, 100, defaultBackgroundColor);
        Port p = CreatePort(n, "Out", Color.red, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyVariable));
        n.outputContainer.Add(p);

        ObjectField fuzzyLogic = new ObjectField { name = "Variable Script", objectType = typeof(StateMachineGraphWindow.FuzzyVariable) };

        fuzzyLogic.RegisterCallback<ChangeEvent<Object>>(
            obj =>
            {
                ObjectField vObj = (ObjectField)obj.target;
                n.title = vObj.value.ToString();
            }
        );

        p.source = fuzzyLogic.value;

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }

    private void OnCreateState(DropdownMenuAction action)
    {
        Node n = CreateNode("New State", 200, 100, defaultBackgroundColor);
        Port p = CreatePort(n, "Out", Color.cyan, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Superior.FuzzyStateMachine.StateMachineState));
        n.outputContainer.Add(p);

        ObjectField fuzzyLogic = new ObjectField { name = "State Script", objectType = typeof(Superior.FuzzyStateMachine.StateMachineState) };

        fuzzyLogic.RegisterCallback<ChangeEvent<Object>>(
            obj =>
            {
                ObjectField vObj = (ObjectField)obj.target;
                n.title = vObj.value.ToString();
            }
        );

        p.source = fuzzyLogic.value;

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }

    private void OnCreateLogic(DropdownMenuAction action)
    {
        Node n = CreateNode("New Logic", 300, 140, defaultBackgroundColor);
        n.inputContainer.Add(CreatePort(n, "In Variables", Color.red, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyVariable)));
        n.inputContainer.Add(CreatePort(n, "In States", Color.cyan, Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Superior.FuzzyStateMachine.StateMachineState)));
        n.outputContainer.Add(CreatePort(n, "Out State", Color.cyan, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Superior.FuzzyStateMachine.StateMachineState)));

        ObjectField fuzzyLogic = new ObjectField { name = "State Script", objectType = typeof(Superior.FuzzyStateMachine.StateMachineState) };

        fuzzyLogic.RegisterCallback<ChangeEvent<Object>>(
            obj =>
            {
                ObjectField vObj = (ObjectField)obj.target;
                n.title = vObj.value.ToString();
            }
        );

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }
}
