using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    [CreateAssetMenu(fileName = "New Graph", menuName = "FuzzyStateMachine/New Graph", order = 1)]
    public class StateMachineGraph : ScriptableObject
    {
        // Yep. Saving data time.

        [System.Serializable]
        public struct PortData
        {
            public string name;
            public string type;
            public Color color;
            public UnityEditor.Experimental.GraphView.Orientation orientation;
            public UnityEditor.Experimental.GraphView.Direction direction;
            public UnityEditor.Experimental.GraphView.Port.Capacity capacity;
        }

        [System.Serializable]
        public struct NodeData
        {
            public Object value; // Data input.
            public string type; // Data input type.
            public PortData[] ports; // Ports data.
            public float x, y, w, h; // Size/Position.
        }

        public List<NodeData> nodes = new List<NodeData>();

        public void AddNode(StateMachineGraphView.NodeInfo node)
        {
            float w = node.node.style.width.value.value; // What the actual hell.
            float h = node.node.style.height.value.value; // Like actually unity please...
            Rect xy = node.node.GetPosition(); // Peace finally.

            NodeData nD = new NodeData { value = node.obj.value, x = xy.x, y = xy.y, w = w, h = h, type = node.obj.objectType.FullName};

            nD.ports = new PortData[node.ports.Count];
            int index = 0;

            foreach (UnityEditor.Experimental.GraphView.Port port in node.ports)
            {
                nD.ports[index] = new PortData { name = port.portName, color = port.portColor, 
                    orientation = port.orientation, direction = port.direction, 
                    capacity = port.capacity, type = port.portType.FullName };

                index++;
            }

            nodes.Add(nD);

            Debug.Log(node.node.title + "; ports: " + node.ports.Count);

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        /* TODO
         Make node data structs for an array etc, to save all of the graphs information and connections!!
         */

        // Incredibly useful operation for double clicking the scriptable object!
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenInGraphEditor(int instanceID, int line)
        {
            Object obj = UnityEditor.EditorUtility.InstanceIDToObject(instanceID);

            if (obj.GetType() == typeof(StateMachineGraph))
            {
                StateMachineGraphWindow.ShowWindow();
                StateMachineGraphWindow.GraphWindow.LoadGraph((StateMachineGraph)obj);
                
                return true;
            }

            
            return false;
        }
    }
}
