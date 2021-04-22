using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class StateMachineGraphWindow : EditorWindow
{
    static StateMachineGraphWindow graphWindow;

    [MenuItem("Window/SME")]
    public static void ShowWindow()
    {
        Debug.Log("please work");
        if (StateMachineGraphWindow.graphWindow == null)
        {
            graphWindow = CreateWindow<StateMachineGraphWindow>();
            graphWindow.titleContent = new GUIContent("Graph Window");
            graphWindow.minSize = new Vector2(500, 400);
        }
        graphWindow.Show();
        graphWindow.Initialize();
    }

    bool isInitialize = false;
    private StateMachineGraphView graphView;
    private string _fileName = "New State Machine";

    public void Initialize()
    {
        if (isInitialize) return;

        isInitialize = true;

        if (graphView == null)
        {
            graphView = new StateMachineGraphView();
        }





        this.rootVisualElement.Add(graphView);

        graphView.Initialize();

        var toolbar = new Toolbar();

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });

        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
        // toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) {text = "New Node",});
        this.rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool v)
    {
        throw new NotImplementedException();
    }
}