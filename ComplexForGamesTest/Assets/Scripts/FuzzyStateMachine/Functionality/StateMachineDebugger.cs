using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    [RequireComponent(typeof(StateMachineLoader))]
    public class StateMachineDebugger : MonoBehaviour
    {
        [Header("Test")]
        public Vector3 unDesirability;
        public Vector3 desirability;
        public Vector3 veryDesirability;
        public float deffuziedOutput;

        public Dictionary<string, Variable.FuzzyMember> fuzzies = new Dictionary<string, Variable.FuzzyMember>();

        public FuzzyLogic logic;

        [ContextMenu("Perform")]
        public void Perform()
        {
            StateMachineLoader loader = GetComponent<StateMachineLoader>();
            loader.Start();

            logic = loader._logic;

            unDesirability = loader._shapeSet.unDesirability;
            desirability = loader._shapeSet.desirability;
            veryDesirability = loader._shapeSet.veryDesirability;

            deffuziedOutput = loader.deffuzifiedOutput;
        }
    }
}

