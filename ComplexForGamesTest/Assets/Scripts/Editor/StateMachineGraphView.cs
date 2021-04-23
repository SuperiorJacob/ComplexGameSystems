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

    private void OnCreateVariables(DropdownMenuAction action)
    {
        Node n = new Node
        {
            title = "New Variables",
            style =
            {
                width = 200,
                height = 100
            }
        };

        n.extensionContainer.style.backgroundColor = new Color(0.6f, 0.24f, 0.24f, 0.8f);

        Port p = n.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyVariable));
        p.portName = "Out";
        p.portColor = Color.red;
        n.outputContainer.Add(p);

        ObjectField fuzzyLogic = new ObjectField
        {
            name = "State Script",
            objectType = typeof(StateMachineGraphWindow.FuzzyVariable)
        };

        p.source = fuzzyLogic.value;

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();
        n.capabilities &= ~Capabilities.Deletable;
        n.capabilities |= Capabilities.Movable;
        n.capabilities |= Capabilities.Collapsible;
        n.capabilities |= Capabilities.Renamable;

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }

    private void OnCreateState(DropdownMenuAction action)
    {
        Node n = new Node
        {
            title = "New State",
            style =
            {
                width = 200,
                height = 100
            }
        };

        n.extensionContainer.style.backgroundColor = new Color(0.6f, 0.24f, 0.24f, 0.8f);

        Port p = n.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyState));
        p.portName = "Out";
        p.portColor = Color.cyan;
        n.outputContainer.Add(p);

        ObjectField fuzzyLogic = new ObjectField
        {
            name = "State Script",
            objectType = typeof(StateMachineGraphWindow.FuzzyState)
        };

        p.source = fuzzyLogic.value;

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();
        n.capabilities &= ~Capabilities.Deletable;
        n.capabilities |= Capabilities.Movable;
        n.capabilities |= Capabilities.Collapsible;
        n.capabilities |= Capabilities.Renamable;

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }

    private void OnCreateLogic(DropdownMenuAction action)
    {
        Node n = new Node
        {
            title = "New Logic",
            style =
            {
                width = 300,
                height = 140
            }
        };

        n.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);

        Port inVars = n.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyVariable));
        inVars.portName = "In Variables";
        inVars.portColor = Color.red;
        n.inputContainer.Add(inVars);

        Port inStates = n.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyState));
        inStates.portName = "In States";
        inStates.portColor = Color.cyan;
        n.inputContainer.Add(inStates);

        Port outPort = n.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(StateMachineGraphWindow.FuzzyState));
        outPort.portName = "Out State";
        outPort.portColor = Color.cyan;
        n.outputContainer.Add(outPort);

        ObjectField fuzzyLogic = new ObjectField
        {
            name = "Fuzzy Logic Script",
            objectType = typeof(StateMachineGraphWindow.FuzzyLogic)
        };

        n.extensionContainer.Add(fuzzyLogic);

        n.RefreshExpandedState();
        n.RefreshPorts();
        n.capabilities &= ~Capabilities.Deletable;
        n.capabilities |= Capabilities.Resizable;
        n.capabilities |= Capabilities.Movable;
        n.capabilities |= Capabilities.Collapsible;
        n.capabilities |= Capabilities.Renamable;

        n.SetPosition(new Rect(action.eventInfo.mousePosition, Vector2.zero));

        AddElement(n);
    }
}
