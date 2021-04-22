//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class GraphEditorWindow : EditorWindow
//{
//    bool isLeftClicking = false;

//    Rect button1 = new Rect(0, 0, 20, 20);

//    Rect windowRight = new Rect(500, 500, 500, 1000);
//    Texture image;

//    Rect[] windowArray = { new Rect(100 + 100, 100, 100, 100), new Rect(100, 100, 100, 100) };

//    Vector2 mousePos;

//    struct MouseDraggingConnector
//    {
//        public bool dragging;
//        public int windowID;
//        public bool connected;
//        public int endID;
//    }

//    MouseDraggingConnector currentDrag;

//    [MenuItem("Window/Fuzzy State Machine Editor")]
//    static void Init()
//    {
//        GetWindow(typeof(GraphEditorWindow), false, "Fuzzy State Machine Editor");

//    }

//    private Rect PushInside(Rect a, Rect b)
//    {
//        return new Rect(a.x + b.x, a.y + b.y, a.width, a.height);
//    }

//    private void OnGUI()
//    {
//        Event e = Event.current;

//        isLeftClicking = (e != null && e.button == 1);

//        Debug.Log(isLeftClicking);

//        mousePos = e.mousePosition;

//        if (currentDrag.dragging && !currentDrag.connected)
//        {
//            Rect inside = PushInside(button1, windowArray[currentDrag.windowID]);
//            Vector2 pre = button1.size / 2;

//            Handles.BeginGUI();
//            Handles.DrawBezier(inside.position + pre, mousePos, new Vector2(windowArray[currentDrag.windowID].position.x + pre.x, windowArray[currentDrag.windowID].position.y + pre.y), new Vector2(windowArray[currentDrag.windowID].position.x + pre.x, windowArray[currentDrag.windowID].position.y + pre.y), Color.yellow, null, 5f);
//            //Handles.DrawLine(inside.position, mousePos);
//            //Handles.DrawBezier(inside.position, mousePos, new Vector2(windowArray[currentDrag.windowID].xMax, windowArray[currentDrag.windowID].center.y), new Vector2(0, 0), Color.magenta, null, 5f);
//            Handles.EndGUI();
//        }
//        else if (currentDrag.connected)
//        {
//            Rect inside = PushInside(button1, windowArray[currentDrag.windowID]);
//            Rect inside2 = PushInside(button1, windowArray[currentDrag.endID]);
//            Vector2 pre = button1.size / 2;

//            Handles.BeginGUI();
//            Handles.DrawBezier(inside.position + pre, inside2.position + pre, new Vector2(windowArray[currentDrag.windowID].position.x + pre.x, windowArray[currentDrag.windowID].position.y + pre.y), new Vector2(windowArray[currentDrag.endID].position.x + pre.x, windowArray[currentDrag.endID].position.y + pre.y), Color.yellow, null, 5f);
//            //Handles.DrawBezier(inside.position, inside2.position, new Vector2(windowArray[currentDrag.windowID].position.x, windowArray[currentDrag.windowID].position.y), new Vector2(windowArray[currentDrag.endID].position.x, windowArray[currentDrag.endID].position.y), Color.yellow, null, 5f);
//            //Handles.DrawLine(inside.position, inside2.position);
//            //Handles.DrawBezier(inside.position, inside2.position, new Vector2(windowArray[currentDrag.windowID].xMax, windowArray[currentDrag.windowID].center.y), new Vector2(windowArray[currentDrag.endID].xMin, windowArray[currentDrag.endID].center.y), Color.yellow, null, 5f);
//            Handles.EndGUI();
//        }

//        //Handles.BeginGUI();
//        //Handles.DrawBezier(windowArray[0].center, windowArray[0].center, new Vector2(windowArray[0].xMax, windowArray[0].center.y), new Vector2(windowArray[0].xMin, windowArray[0].center.y), Color.red, null, 5f);
//        //Handles.EndGUI();

//        BeginWindows();
//        windowArray[0] = GUI.Window(0, windowArray[0], NodeFunction, "Box1");
//        windowArray[1] = GUI.Window(1, windowArray[1], NodeFunction, "Box2");

//        windowRight = GUI.Window(2, windowRight, WindowFunction, "Right Side");

//        EndWindows();

//    }

//    void Update()
//    {
//        if (focusedWindow == this &&
//            mouseOverWindow == this)
//        {
//            this.Repaint();

//        }
//    }

//    void NodeFunction(int windowID)
//    {
//        GUI.Box(button1, "" + windowID);

//        Rect inside = PushInside(button1, windowArray[windowID]);

//        if (inside.Contains(mousePos) && isLeftClicking)
//        {
//            if (currentDrag.connected && windowID == currentDrag.windowID)
//            {
//                currentDrag.connected = false;
//                return;
//            }

//            if (currentDrag.dragging && windowID != currentDrag.windowID)
//            {
//                currentDrag.endID = windowID;
//                currentDrag.connected = true;

//                return;
//            }

//            currentDrag.windowID = windowID;
//            currentDrag.dragging = true;
//        }
//        else if (!isLeftClicking && windowID == currentDrag.windowID && currentDrag.dragging)
//        {
//            currentDrag.dragging = false;
//        }
//        else GUI.DragWindow();
//    }

//    void WindowFunction(int windowID)
//    {
//        GUI.color = currentDrag.dragging ? Color.green : Color.red;
//        GUI.Box(new Rect(0, 0, 50, 50), image);
//        GUI.color = Color.gray;

//        GUI.DragWindow();
//    }
//}