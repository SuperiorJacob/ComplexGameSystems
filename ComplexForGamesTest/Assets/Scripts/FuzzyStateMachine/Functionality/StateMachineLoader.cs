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
        public float deffuzifiedOutput;

        public void Load()
        {
            StateMachineGraph.NodeData data = _graph.nodes[0];

            // FIX THIS PLEASE

            foreach (var node in _graph.nodes)
            {
                if (node.type == typeof(FuzzyLogic).FullName)
                {
                    //System.Type typ = System.Type.GetType(node.type);
                    //_logic = (FuzzyLogic)System.Activator.CreateInstance(typ);
                    //_logic = new FuzzyLogic();

                    data = node;

                    continue;
                }
                else if (data.name != "Graph Output")
                {
                    // :) fix
                    System.Type typ = System.Type.GetType(node.type);

                    if (!(node.ports.Length > 0) || !(node.ports[0].connections.Length > 0)) continue;

                    int o = node.ports[0].connections[0].output;

                    if (o == data.ports[0].id)
                    {
                        _variables = (Variable.FuzzyVariable)System.Activator.CreateInstance(typ);
                    }
                    else if (o == data.ports[1].id)
                    {
                        _shapeSet = (Variable.FuzzyShapeSet)node.value;
                    }
                    else if (o == data.ports[2].id)
                    {
                        _rules = (FuzzyRuleSet)System.Activator.CreateInstance(typ);
                    }
                }
            }

            _shapeSet.Init();
            _rules = new Rules.Example.GetBlanket(_shapeSet.LoadShapeSet());
            //_rules.NewSet(_shapeSet.LoadShapeSet());
            _logic = new FuzzyLogic(_rules);
            _logic.variables = _variables;
        }

        // Start is called before the first frame update
        void Start()
        {
            Load();

            _logic.Calculate();
            deffuzifiedOutput = _logic.Deffuzify();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}