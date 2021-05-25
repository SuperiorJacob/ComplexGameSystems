using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyStateMachine
{
    public class StateMachineLoader : MonoBehaviour
    {
        #region Structs

        /// <summary>
        /// Node branch used in the node tree, contains node data and output and inputs.
        /// </summary>
        public struct NodeBranch
        {
            public StateMachineGraph.NodeData data;
            public List<NodeBranch> outs;
            public List<NodeBranch> ins;
        }

        /// <summary>
        /// Function data, has the branch, and a bunch of variables calculated and outputted during @RunFunction
        /// </summary>
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

        #endregion

        #region Fields

        [Tooltip("Graph input used to be loaded (without one nothing happens)")] public StateMachineGraph _graph;
        [Tooltip("The output of the loaded data")] public FunctionData _outPut;

        /// <summary>
        /// The loaded crisp overrides.
        /// </summary>
        public (string name, float input)[] crispOverrides;

        /// <summary>
        /// List of logs loaded and sent to the debugger.
        /// </summary>
        [HideInInspector] public List<string> logs = new List<string>();

        #endregion

        /// <summary>
        /// Branches backwards from start node and creates a list of nodes heading towards the final node (the start node).
        /// </summary>
        /// <param name="a_startNode">The final node, where to start from going backwards</param>
        /// <returns>A node branch tree</returns>
        public NodeBranch BranchFromStartNode(StateMachineGraph.NodeData a_startNode)
        {
            // Define our data lookup tables.
            Dictionary<int, StateMachineGraph.NodeData> portIDToNode = new Dictionary<int, StateMachineGraph.NodeData>();
            Dictionary<StateMachineGraph.NodeData, NodeBranch> branchByNode = new Dictionary<StateMachineGraph.NodeData, NodeBranch>();

            // Load lookup tables.
            foreach (StateMachineGraph.NodeData n in _graph.nodes)
            {
                branchByNode[n] = new NodeBranch { data = n, ins = new List<NodeBranch>(), outs = new List<NodeBranch>() };

                for (int i = 0; i < n.ports.Length; i++)
                {
                    portIDToNode[n.ports[i].id] = n;
                }
            }

            // Using the lookup tables, connect them together.
            foreach (StateMachineGraph.NodeData n in _graph.nodes)
            {
                NodeBranch b = branchByNode[n];

                for (int i = 0; i < n.ports.Length; i++)
                {
                    portIDToNode[n.ports[i].id] = n;
                }

                // Add connections so when we loop through it, we get the right values.
                for (int i = 0; i < n.ports.Length; i++)
                {
                    StateMachineGraph.PortData port = n.ports[i];

                    if (port.connections != null && port.connections.Length != 0)
                    {
                        for (int p = 0; p < port.connections.Length; p++)
                        {
                            StateMachineGraph.PortConnector c = port.connections[p];

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
                    else
                        logs.Add($"Error: For some reason port ({b.data.name}.[{port.id}]{port.name}) is unused. Ignoring.");
                }

                branchByNode[n] = b;
            }

            return branchByNode[a_startNode];
        }

        /// <summary>
        /// Previous by the input of a branch.
        /// </summary>
        /// <param name="a_branch">The branch we want to check the input to.</param>
        /// <returns>The input aka the previous branch.</returns>
        public NodeBranch PreviousByIn(NodeBranch a_branch)
        {
            return a_branch.ins[0];
        }

        /// <summary>
        /// Goes to the back of a tree based on a branch and desired depth.
        /// </summary>
        /// <param name="a_tree">The tree we want to go to the back of.</param>
        /// <param name="a_prev">The prev branch</param>
        /// <param name="a_depth">How deep do we want to go?</param>
        /// <param name="a_outDepth">The current depth</param>
        /// <returns>The final node branch that is a functionality!</returns>
        public NodeBranch BackBranchOfTree(NodeBranch a_tree, NodeBranch a_prev, int a_depth, out int a_outDepth)
        {
            a_depth += 1;
            a_outDepth = a_depth;

            if (a_tree.ins == null || a_tree.ins.Count < 1)
            {
                a_outDepth = a_depth - 1;
                return a_prev;
            }

            NodeBranch n = PreviousByIn(a_tree);

            return BackBranchOfTree(n, a_tree, a_depth, out a_outDepth);
        }

        /// <summary>
        /// Runs the Node function.
        /// </summary>
        /// <param name="a_data">The branch to execute on</param>
        /// <param name="a_pass">Passed function data we want to compare to</param>
        /// <param name="a_depth">Stop at a certain depth</param>
        /// <returns>Function data after all executions</returns>
        public FunctionData RunFunction(NodeBranch a_data, FunctionData a_pass = default, int a_depth = -1)
        {
            FunctionData fD = a_pass;

            if (fD.logic == null) fD.logic = new List<FuzzyLogic>();

            if (a_depth > 0) a_depth--;
            if (a_depth == 0) return fD;

            fD.branch = a_data;

            logs.Add($"Calculating function: {a_data.data.name}");

            bool calculated = false;
            if (System.Type.GetType(a_data.data.type) == typeof(FuzzyLogic))
            {
                fD.variables = null;
                fD.ruleSet = null;
                fD.shapeSet = null;

                MonoScript l = (MonoScript)a_data.data.value;
                FuzzyLogic logic = (FuzzyLogic)System.Activator.CreateInstance(l.GetClass());

                bool executionFailure = false;

                logs.Add($"Initializing Logic: {a_data.data.type}");

                foreach (NodeBranch ins in a_data.ins)
                {
                    if (ins.data.type == typeof(Variable.FuzzyVariable).FullName)
                    {
                        fD.variables = (Variable.FuzzyVariable)ins.data.value;
                        logs.Add($" Loading variables: {fD.variables.name}");
                    }
                    else if (ins.data.type == typeof(Variable.FuzzyShapeSet).FullName)
                    {
                        fD.shapeSet = (Variable.FuzzyShapeSet)ins.data.value;
                        logs.Add($" Loading shape set: {fD.shapeSet.name}");
                    }
                    else if (ins.data.type == typeof(FuzzyRuleSet).FullName)
                    {
                        l = (MonoScript)ins.data.value;
                        fD.ruleSet = (FuzzyRuleSet)System.Activator.CreateInstance(l.GetClass());
                        logs.Add($" Loading rules: {l.name}");
                    }
                }

                if (fD.variables == null) 
                { 
                    executionFailure = true; 
                    logs.Add($"Error: The variables do not exist, canceling calculation."); 
                }
                else 
                { 
                    fD.variables.Init(crispOverrides); 
                    logs.Add($" Initializing variables..."); 
                }

                if (fD.ruleSet == null) 
                { 
                    executionFailure = true; 
                    logs.Add($"Error: The rules do not exist, canceling calculation."); 
                }
                else 
                { 
                    fD.ruleSet.NewSet(fD.shapeSet.LoadShapeSet()); 
                    logs.Add($" Loading set..."); 
                }

                if (!executionFailure)
                {
                    logic.rule = fD.ruleSet;
                    logic.variables = fD.variables;

                    logs.Add($" Calculating logic...");
                    logic.Calculate();

                    fD.deffuzied = logic.Defuzzify();
                    logs.Add($" Deffuzified: {fD.deffuzied}");

                    if (float.IsNaN(fD.deffuzied))
                    {
                        logs.Add($"Error: Deffuzified value returned NaN, canceling calculation.");
                        logs.Add($"Error: Logic calculation has been terminated due to an error.");
                    }
                    else
                        fD.logic.Add(logic);

                    calculated = true;
                }
                else
                    logs.Add($"Error: Logic calculation has been terminated due to an error.");
            }
            else if ((a_data.data.type == "Greater" && fD.deffuzied > a_data.data.value2) || 
                    (a_data.data.type == "Lesser" && fD.deffuzied < a_data.data.value2) || 
                    (a_data.data.type == "Equal" && fD.deffuzied == a_data.data.value2))
            {
                logs.Add($" {fD.deffuzied} is {(a_data.data.type == "Greater" ? ">" : (a_data.data.type == "Lesser" ? "<" : "=="))} {a_data.data.value2} !");

                if (a_data.ins.Count == 2)
                {
                    MonoScript l = (MonoScript)a_data.ins[1].data.value;
                    fD.state = (States.StateMachineState)System.Activator.CreateInstance(l.GetClass());
                    logs.Add($" Accepted state: {l.name}");
                }
                else 
                    logs.Add($" No state found using old state.");

                calculated = true;
            }
            else if (a_data.data.type == "HasState")
            {
                if (a_data.ins.Count > 1)
                {
                    /* Because how the node graph works, it runs in order from far left to far right, however
                     has state is a middle man for two logics therefore it should be able to execute far left then
                     execute second logic left before it compares. */

                    logs.Add($" Finding connections...");
                    int depth;
                    NodeBranch startBranch = BackBranchOfTree(a_data.ins[1], a_data.ins[1], 1, out depth);

                    logs.Add($" Calculating functions...");
                    FunctionData data = RunFunction(startBranch, fD, depth);

                    // If the second datas not null and the primary data is null or the desireability is higher then the other set primary state.

                    logs.Add($"Finalizing connection data...");
                    logs.Add($" Comparing (primary) {(fD.deffuzied * fD.ruleSet.weight)} < (secondary) {(data.deffuzied * data.ruleSet.weight)}");

                    if ((fD.deffuzied * fD.ruleSet.weight) < (data.deffuzied * data.ruleSet.weight))
                    {
                        logs.Add($" The secondary input ({data.deffuzied}) is more important than the primary input ({fD.deffuzied}) !");

                        fD = data;
                    }
                    else
                        logs.Add($" The primary input ({fD.deffuzied}) is more important than the secondary input ({data.deffuzied}) !");

                    calculated = true;
                }
            }
            else if (a_data.data.type == "")
            {
                logs.Add(
                    $" Logics calculated: {fD.logic.Count}" +
                    $"\n Deffuzied Desire: {fD.deffuzied} / 75.5" +
                    $"\n Rule Set: {(fD.ruleSet != null ? fD.ruleSet.GetType().Name : "")}" +
                    $"\n\n State Info:" +
                    $"\n\t Name: {fD.state}" +
                    $"\n\t Execution Type: {(fD.state != null ? fD.state.executionType.ToString() : "")}"
                );

                _outPut = fD;

                return fD;
            }
            else
            {
                logs.Add($" No functionality needed to take place.");
                calculated = true;
            }

            if (!calculated)
                logs.Add($"Error: this function [{a_data.data.name}] has failed to execute !");

            return (a_data.outs == null || a_data.outs.Count < 1) ? fD : RunFunction(a_data.outs[0], fD, a_depth);
        }

        /// <summary>
        /// Loads all data from a node graph scriptable object and overrides desirable inputs.
        /// </summary>
        /// <param name="a_inputs">Input overriding</param>
        public void Load(params (string name, float input)[] a_inputs)
        {
            crispOverrides = a_inputs;

            logs = new List<string>{ $"Begin loading graph data into functionality..." };

            bool usingEditorTime = !(Time.realtimeSinceStartup > 0);

            float startTime = usingEditorTime ? (float)EditorApplication.timeSinceStartup : Time.realtimeSinceStartup;

            if (_graph == null)
            {
                logs.Add($"Error: Graph does not exist?");
            }
            else
            {
                // Starting node is the Graph Output ALWAYS. Unless manually edited...
                NodeBranch tree = BranchFromStartNode(_graph.nodes[0]);
                NodeBranch startBranch = BackBranchOfTree(tree, tree, 0, out _);

                RunFunction(startBranch);
            }

            float endTime = (usingEditorTime ? (float)EditorApplication.timeSinceStartup : Time.realtimeSinceStartup) - startTime;

            logs.Add($"Successful load. Calculated in {endTime} ms.");
        }
    }
}
