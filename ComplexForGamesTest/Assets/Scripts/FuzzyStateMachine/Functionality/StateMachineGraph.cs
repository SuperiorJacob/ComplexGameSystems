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
        public struct PortConnector
        {
            public int input;
            public int output;
        }

        [System.Serializable]
        public struct PortData
        {
            public string name;
            public int id;
            public PortConnector[] connections;
            public string type;
            public Color color;
            public UnityEditor.Experimental.GraphView.Orientation orientation;
            public UnityEditor.Experimental.GraphView.Direction direction;
            public UnityEditor.Experimental.GraphView.Port.Capacity capacity;
        }

        [System.Serializable]
        public struct NodeData
        {
            public string name; // Node name.
            public Object value; // Data input.
            public string type; // Data input type.
            public PortData[] ports; // Ports data.
            public float x, y, w, h; // Size/Position.
        }

        public List<NodeData> nodes = new List<NodeData>();
        public int ports = 0;

        // Hopefully wont save :)
        public Dictionary<UnityEditor.Experimental.GraphView.Port, int> portDictionary = new Dictionary<UnityEditor.Experimental.GraphView.Port, int>();

        public void AddNode(StateMachineGraphView.NodeInfo node)
        {
            float w = node.node.style.width.value.value; // What the actual hell.
            float h = node.node.style.height.value.value; // Like actually unity please...
            Rect xy = node.node.GetPosition(); // Peace finally.

            NodeData nD = new NodeData { name = node.node.title, value = node.obj != null ? node.obj.value : null, x = xy.x, y = xy.y, w = w, h = h, type = node.obj != null ? node.type.FullName : ""};

            nD.ports = new PortData[node.ports.Count];
            int index = 0;

            // Declaration
            foreach (UnityEditor.Experimental.GraphView.Port port in node.ports)
            {
                if (port == null) continue;

                nD.ports[index] = new PortData { id = ports + index, name = port.portName, color = port.portColor, 
                    orientation = port.orientation, direction = port.direction, 
                    capacity = port.capacity, type = port.portType.FullName };

                portDictionary[port] = ports + index;

                index++;
            }

            ports += index;

            nodes.Add(nD);

            Debug.Log(node.node.title + "; ports: " + node.ports.Count);

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        public void ConnectPorts(List<StateMachineGraphView.NodeInfo> nIList)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeData nD = nodes[i];
                StateMachineGraphView.NodeInfo node = nIList[i];

                // Port connection
                for (int ii = 0; ii < node.ports.Count; ii++)
                {
                    UnityEditor.Experimental.GraphView.Port port = node.ports[ii];

                    //nD.ports[ii].connections = new PortConnector[nD.ports[ii].connections.Length];

                    if (port.connected)
                    {
                        List<PortConnector> pL = new List<PortConnector>();

                        foreach (var connection in port.connections)
                        {
                            PortConnector c = new PortConnector
                            {
                                input = port.direction == UnityEditor.Experimental.GraphView.Direction.Input ? portDictionary[connection.output] : -1,
                                output = port.direction == UnityEditor.Experimental.GraphView.Direction.Output ? portDictionary[connection.input] : -1
                            };

                            // Probably uneccessary but, this is so I cant break my own code.

                            bool failed = false;
                            foreach (PortConnector pC in pL)
                            {
                                if (pC.input == c.input && pC.input == c.input) failed = true;
                            }

                            if (!failed)
                                pL.Add(c);
                        }

                        nD.ports[ii].connections = pL.ToArray();
                    }
                }

                nodes[i] = nD;
            }

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
