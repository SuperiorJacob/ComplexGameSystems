using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    public class StateMachineLoader : MonoBehaviour
    {
        public StateMachineGraph _graph;
        public FuzzyLogic _logic;
        public FuzzyRuleSet _rules;
        public Variable.FuzzyVariable _variables;
        public Variable.FuzzyShapeSet _shapeSet;
        public Dictionary<string, States.StateMachineState> _states = new Dictionary<string, States.StateMachineState>();

        public float deffuzifiedOutput;

        public void Load()
        {
            StateMachineGraph.NodeData data = new StateMachineGraph.NodeData { };

            // FIX THIS PLEASE

            foreach (var node in _graph.nodes)
            {
                if (System.Type.GetType(node.type) == typeof(FuzzyLogic))
                {
                    //System.Type typ = System.Type.GetType(node.type);
                    //_logic = (FuzzyLogic)System.Activator.CreateInstance(typ);
                    //_logic = new FuzzyLogic();

                    data = node;

                    _logic = (FuzzyLogic)System.Activator.CreateInstance(((UnityEditor.MonoScript)data.value).GetClass());

                    break;
                }
            }

            if (data.value != null)
            {
                foreach (var node in _graph.nodes)
                {
                    if (data.name != "Graph Output")
                    {
                        // :) fix
                        if (!(node.ports.Length > 0) || !(node.ports[0].connections.Length > 0)) continue;

                        int o = node.ports[0].connections[0].output;

                        if (o == data.ports[0].id)
                        {
                            _variables = (Variable.FuzzyVariable)node.value;
                        }
                        else if (o == data.ports[1].id)
                        {
                            _shapeSet = (Variable.FuzzyShapeSet)node.value;
                        }
                        else if (o == data.ports[2].id)
                        {
                            _rules = (FuzzyRuleSet)System.Activator.CreateInstance(((UnityEditor.MonoScript)node.value).GetClass());
                        }
                        else if (o == data.ports[3].id)
                        {
                            UnityEditor.MonoScript typ = (UnityEditor.MonoScript)node.value;

                            _states[typ.name] = (States.StateMachineState)System.Activator.CreateInstance(typ.GetClass());
                        }
                    }
                }

                if (_rules != null && _shapeSet != null)
                {
                    _variables.Init();
                    _rules.NewSet(_shapeSet.LoadShapeSet());
                    _logic.rule = _rules;
                    _logic.variables = _variables;
                }
                else throw new System.Exception("You must have shape, rules in your graph!");
            }
            else throw new System.Exception("Your graph doesnt have fuzzy logic!");
        }

        // Start is called before the first frame update
        public void Start()
        {
            Load();

            if (_logic != null)
            {
                _logic.Calculate();
                deffuzifiedOutput = _logic.Deffuzify();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}