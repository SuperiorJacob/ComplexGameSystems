using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyStateMachine
{
    [RequireComponent(typeof(StateMachineLoader))]
    public class StateMachineDebugger : MonoBehaviour
    {
        [Header("Desire")]
        public Vector3 unDesirability;
        public Vector3 desirability;
        public Vector3 veryDesirability;
        public float deffuziedOutput;

        // Loader output
        public List<string> debug;

        public Dictionary<string, Variable.FuzzyMember> fuzzies = new Dictionary<string, Variable.FuzzyMember>();

        public FuzzyLogic[] logic;

        [ContextMenu("Perform")]
        public void Perform()
        {
            StateMachineLoader loader = GetComponent<StateMachineLoader>();
            loader.Start();

            logic = loader._outPut.logic.ToArray();
            debug = loader.logs;
            unDesirability = loader._outPut.shapeSet.unDesirability;
            desirability = loader._outPut.shapeSet.desirability;
            veryDesirability = loader._outPut.shapeSet.veryDesirability;
            deffuziedOutput = loader._outPut.deffuzied;
        }
    }
}

