using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyStateMachine
{
    public class StateMachineLoader : MonoBehaviour
    {
        public StateMachineGraph _graph;
        public FunctionData _outPut;

        public struct NodeBranch
        {
            public StateMachineGraph.NodeData data;
            public List<NodeBranch> outs;
            public List<NodeBranch> ins;
        }

        public struct FunctionData
        {
            public NodeBranch branch;
            public List<FuzzyLogic> logic;

            public Variable.FuzzyVariable variables;
            public Variable.FuzzyShapeSet shapeSet;
            public FuzzyRuleSet ruleSet;

            public States.StateMachineState state;
            public float deffuzied;
        }

        public List<string> logs = new List<string>();

        /// <summary>
        /// Branches backwards from start node and creates a list of nodes heading towards the final node (the start node).
        /// </summary>
        /// <param name="a_startNode">The final node, where to start from going backwards</param>
        /// <returns>A node branch tree</returns>
        public NodeBranch BranchFromStartNode(StateMachineGraph.NodeData a_startNode)
        {
            Dictionary<int, StateMachineGraph.NodeData> portIDToNode = new Dictionary<int, StateMachineGraph.NodeData>();
            Dictionary<StateMachineGraph.NodeData, NodeBranch> branchByNode = new Dictionary<StateMachineGraph.NodeData, NodeBranch>();

            foreach (var n in _graph.nodes)
            {
                branchByNode[n] = new NodeBranch { data = n, ins = new List<NodeBranch>(), outs = new List<NodeBranch>() };

                for (int i = 0; i < n.ports.Length; i++)
                {
                    portIDToNode[n.ports[i].id] = n;
                }
            }

            foreach (var n in _graph.nodes)
            {
                NodeBranch b = branchByNode[n];

                for (int i = 0; i < n.ports.Length; i++)
                {
                    portIDToNode[n.ports[i].id] = n;
                }

                for (int i = 0; i < n.ports.Length; i++)
                {
                    var port = n.ports[i];

                    for (int p = 0; p < port.connections.Length; p++)
                    {
                        var c = port.connections[p];

                        if (c.input > -1 && portIDToNode.ContainsKey(c.input) && branchByNode.ContainsKey(portIDToNode[c.input]))
                        {
                            b.ins.Add(branchByNode[portIDToNode[c.input]]);
                        }
                        else if (c.output > -1 && portIDToNode.ContainsKey(c.output) && branchByNode.ContainsKey(portIDToNode[c.output]))
                        {
                            b.outs.Add(branchByNode[portIDToNode[c.output]]);
                        }
                    }
                }

                branchByNode[n] = b;
            }

            return branchByNode[a_startNode];
        }

        public NodeBranch PreviousByIn(NodeBranch a_branch)
        {
            return a_branch.ins[0];
        }

        public NodeBranch BackBranchOfTree(NodeBranch a_tree, int a_depth, out int a_outDepth)
        {
            a_depth = a_depth + 1;
            a_outDepth = a_depth;

            if (a_tree.ins == null || a_tree.ins.Count < 1)
            {
                a_outDepth = a_depth - 1;
                return a_tree.outs[0];
            }

            NodeBranch n = PreviousByIn(a_tree);

            return BackBranchOfTree(n, a_depth, out a_outDepth);
        }

        public FunctionData RunFunction(NodeBranch a_data, FunctionData a_pass = default, int a_depth = -1)
        {
            FunctionData fD = a_pass;

            if (fD.logic == null) fD.logic = new List<FuzzyLogic>();

            if (a_depth > 0) a_depth--;
            if (a_depth == 0) return fD;

            fD.branch = a_data;

            logs.Add($"Calculating function: {a_data.data.name}");

            if (System.Type.GetType(a_data.data.type) == typeof(FuzzyLogic))
            {
                MonoScript l = (MonoScript)a_data.data.value;
                FuzzyLogic logic = (FuzzyLogic)System.Activator.CreateInstance(l.GetClass());

                logs.Add($"Initializing Logic: {a_data.data.type}");

                if (a_data.ins[0].data.value != null)
                {
                    fD.variables = (Variable.FuzzyVariable)a_data.ins[0].data.value;
                    logs.Add($" Loading variables: {fD.variables.name}");
                }
                if (a_data.ins[1].data.value != null)
                {
                    fD.shapeSet = (Variable.FuzzyShapeSet)a_data.ins[1].data.value;
                    logs.Add($" Loading shape set: {fD.shapeSet.name}");
                }
                if (a_data.ins[2].data.value != null)
                {
                    l = (MonoScript)a_data.ins[2].data.value;
                    fD.ruleSet = (FuzzyRuleSet)System.Activator.CreateInstance(l.GetClass());
                    logs.Add($" Loading rules: {l.name}");
                }

                fD.variables.Init();
                logs.Add($" Initializing variables...");
                fD.ruleSet.NewSet(fD.shapeSet.LoadShapeSet());
                logs.Add($" Loading set...");

                logic.rule = fD.ruleSet;
                logic.variables = fD.variables;

                logs.Add($" Calculating logic...");
                logic.Calculate();

                fD.deffuzied = logic.Deffuzify();
                logs.Add($" Deffuzified: {fD.deffuzied}");

                fD.logic.Add(logic);
            }
            else if ((a_data.data.type == "Greater" && fD.deffuzied > a_data.data.value2) || 
                    (a_data.data.type == "Lesser" && fD.deffuzied < a_data.data.value2) || 
                    (a_data.data.type == "Equal" && fD.deffuzied == a_data.data.value2))
            {
                MonoScript l = (MonoScript)a_data.ins[1].data.value;
                fD.state = (States.StateMachineState)System.Activator.CreateInstance(l.GetClass());

                logs.Add($" {fD.deffuzied} is {(a_data.data.type == "Greater" ? ">" : (a_data.data.type == "Lesser" ? "<" : "=="))} {a_data.data.value2} !");
                logs.Add($" Accepted state: {l.name}");
            }
            else if (a_data.data.type == "HasState")
            {
                if (a_data.ins.Count > 1)
                {
                    // Because how the node graph works, it runs in order from far left to far right, however
                    // has state is a middle man for two logics therefore it should be able to execute far left then
                    // execute second logic left before it compares.

                    logs.Add($" Finding connections...");
                    int depth;
                    NodeBranch startBranch = BackBranchOfTree(a_data.ins[1], 1, out depth);

                    logs.Add($" Calculating functions...");
                    FunctionData data = RunFunction(startBranch, fD, depth);
                    fD.logic = data.logic;
                    // If the second datas not null and the primary data is null or the desireability is higher then the other set primary state.

                    if (data.state != null && (fD.state == null || fD.deffuzied < data.deffuzied))
                    {
                        logs.Add($" The secondary input ({data.deffuzied}) is more important than the primary input ({fD.deffuzied}) !");
                        fD.deffuzied = data.deffuzied;
                        fD.state = data.state;
                    }
                    else logs.Add($" The primary input ({fD.deffuzied}) is more important than the secondary input ({data.deffuzied}) !");
                }
            }

            if (a_data.outs == null || a_data.outs.Count < 1) return fD;
            else return RunFunction(a_data.outs[0], fD, a_depth);
        }

        public void Load()
        {
            logs = new List<string>();
            logs.Add($"Begin loading graph data into functionality...");

            bool usingEditorTime = !(Time.realtimeSinceStartup > 0);

            float startTime = usingEditorTime ? (float)EditorApplication.timeSinceStartup : Time.realtimeSinceStartup;

            // Starting node is the Graph Output ALWAYS. Unless manually edited...
            NodeBranch tree = BranchFromStartNode(_graph.nodes[0]);
            NodeBranch startBranch = BackBranchOfTree(tree, 0, out _);

            _outPut = RunFunction(startBranch);

            float endTime = (usingEditorTime ? (float)EditorApplication.timeSinceStartup : Time.realtimeSinceStartup) - startTime;

            logs.Add($"Successful load. Calculated in {endTime} ms.");
        }

        // Start is called before the first frame update
        public void Start()
        {
            Load();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}