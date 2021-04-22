using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class StateMachineGraphView : GraphView
{
    public void Initialize()
    {
        AddGridBackGround();

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
    }

    private class TempGridBackground : GridBackground { }
    void AddGridBackGround()
    {
        //Add grid background
        GridBackground gridBackground = new TempGridBackground();

        gridBackground.name = "GridBackground";
        Insert(0, gridBackground);

        //Set the background zoom range
        this.SetupZoom(0.25f, 2.0f);

        //The expansion size is the same as the parent object
        this.StretchToParentSize();
    }
}
