using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace FuzzyStateMachine
{
    [CreateAssetMenu(fileName = "New Graph", menuName = "FuzzyStateMachine/New Graph", order = 1)]
    public class StateMachineGraph : ScriptableObject
    {

        #region Struct Declaration

        /// <summary>
        /// Port Connection data used by Port Data to reinstate connections.
        /// </summary>
        [System.Serializable]
        public struct PortConnector
        {
            [Tooltip("Port ID input connection")] public int input;
            [Tooltip("Port ID output connection")] public int output;
        }

        /// <summary>
        /// Port Data used to load ports on nodes.
        /// </summary>
        [System.Serializable]
        public struct PortData
        {
            [Tooltip("Port name")] public string name;
            [Tooltip("Port ID")] public int id;
            [Tooltip("Port connections")] public PortConnector[] connections;
            [Tooltip("Port connection type")] public string type;
            [Tooltip("Port visual color")] public Color color;
            [Tooltip("Port orientation")] public int orientation;
            [Tooltip("Port direction")] public int direction;
            [Tooltip("Port capacity")] public int capacity;
        }

        /// <summary>
        /// Node Data used by the graph editor and loader to display data and calculate them.
        /// </summary>
        [System.Serializable]
        public struct NodeData
        {
            public string name; // Node name.
            public Object value; // Data input.
            public string value1; // Data input type.
            public float value2; // For floats inputs
            public string type; // Data input type.
            public PortData[] ports; // Ports data.
            public float x, y, w, h; // Size/Position.
        }

        #endregion

        #region Fields

        [Tooltip("All of the saved nodes on the graph we wish to load later.")]
        public List<NodeData> nodes = new List<NodeData>();

        [Tooltip("How many ports are we working with? *used for id*")]
        public int ports = 0;

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Dictionary of ports used by our saver, will clear it after.
        /// </summary>
        [System.NonSerialized] public Dictionary<Port, int> portDictionary = new Dictionary<Port, int>();

        /// <summary>
        /// Add a node and save it.
        /// </summary>
        /// <param name="a_node">Live node we wish to save.</param>
        public void AddNode(StateMachineGraphView.NodeInfo a_node)
        {
            // Declare style
            float w = a_node.node.style.width.value.value;
            float h = a_node.node.style.height.value.value;
            Rect xy = a_node.node.GetPosition();

            // Create node info
            NodeData nD = new NodeData { name = a_node.node.title, value = a_node.obj?.value, value1 = a_node.obj?.value != null ? (a_node.obj.value.GetType() == typeof(UnityEditor.MonoScript) ? ((UnityEditor.MonoScript)a_node.obj.value).GetClass().ToString() : a_node.obj.value.GetType().ToString()) : null, value2 = a_node.flo != null ? a_node.flo.value : -1f, x = xy.x, y = xy.y, w = w, h = h, type = a_node.typeString};

            nD.ports = new PortData[a_node.ports.Count];
            int index = 0;
            Debug.Log(a_node.ports.Count);

            // Port creation
            foreach (Port port in a_node.ports)
            {
                if (port == null)
                {
                    continue;
                }

                nD.ports[index] = new PortData { id = ports + index, name = port.portName, color = port.portColor, 
                    orientation = (int)port.orientation, direction = (int)port.direction, 
                    capacity = (int)port.capacity, type = port.portType?.FullName };

                portDictionary[port] = nD.ports[index].id;

                index++;
            }

            ports += index;

            nodes.Add(nD);

            Debug.Log(a_node.node.title + "; ports: " + a_node.ports.Count);

            // Save our new data
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        /// <summary>
        /// Connect node ports together in the save data.
        /// </summary>
        /// <param name="a_nIList"></param>
        public void ConnectPorts(List<StateMachineGraphView.NodeInfo> a_nIList)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeData nD = nodes[i];
                StateMachineGraphView.NodeInfo node = a_nIList[i];

                // Port connections
                for (int ii = 0; ii < node.ports.Count; ii++)
                {
                    Port port = node.ports[ii];

                    // If it has a connection, connect it.

                    if (port.connected)
                    {
                        List<PortConnector> pL = new List<PortConnector>();

                        foreach (Edge connection in port.connections)
                        {
                            PortConnector c = new PortConnector
                            {
                                input = port.direction == Direction.Input ? portDictionary[connection.output] : -1,
                                output = port.direction == Direction.Output ? portDictionary[connection.input] : -1
                            };

                            // Prevents overlaps.

                            bool failed = false;
                            foreach (PortConnector pC in pL)
                            {
                                if (pC.input == c.input && pC.output == c.output) failed = true;
                            }

                            if (!failed)
                                pL.Add(c);
                        }

                        nD.ports[ii].connections = pL.ToArray();
                    }
                }

                nodes[i] = nD;
            }

            // Save data
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        /// <summary>
        /// Opening an asset/ item very useful, in this situation used to open the node graph by double clicking the scriptable object!
        /// </summary>
        /// <returns></returns>
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenInGraphEditor(int a_instanceID, int a_line)
        {
            Object obj = UnityEditor.EditorUtility.InstanceIDToObject(a_instanceID);

            // If we don't check, it will work on ANY file !
            if (obj.GetType() == typeof(StateMachineGraph))
            {
                StateMachineGraphWindow.ShowWindow();
                StateMachineGraphWindow.GraphWindow.LoadGraph((StateMachineGraph)obj);
                
                return true;
            }

            
            return false;
        }
#endif
    }
}
