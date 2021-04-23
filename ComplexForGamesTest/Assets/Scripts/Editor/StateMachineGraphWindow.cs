using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void Initialize()
    {
        if (isInitialize) return;

        isInitialize = true;

        if (graphView == null)
        {
            graphView = new StateMachineGraphView();
        }

        graphView.RegisterCallback<KeyDownEvent>(KeyDown);
        this.rootVisualElement.Add(graphView);

        graphView.Initialize();

        Toolbar toolbar = new Toolbar();

        TextField fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });

        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        this.rootVisualElement.Add(toolbar);
    }

    private void KeyDown(KeyDownEvent evt)
    {

    }

    // Make Choice Visualiser

    private void RequestDataOperation(bool v)
    {
        throw new NotImplementedException();
    }
}