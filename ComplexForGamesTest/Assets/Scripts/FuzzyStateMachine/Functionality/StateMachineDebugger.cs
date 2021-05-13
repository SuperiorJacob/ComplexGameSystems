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
        [HideInInspector] public List<string> debug;

        [HideInInspector] public FuzzyLogic[] logic;

        [HideInInspector] public StateMachineLoader.FunctionData mainData;

        private StateMachineLoader loader;

        public void Load()
        {
            if (loader == null) 
                loader = GetComponent<StateMachineLoader>();

            mainData = loader._outPut;
            logic = mainData.logic.ToArray();
            debug = loader.logs;
            unDesirability = mainData.shapeSet.unDesirability;
            desirability = mainData.shapeSet.desirability;
            veryDesirability = mainData.shapeSet.veryDesirability;
            deffuziedOutput = mainData.deffuzied;
        }

        [ContextMenu("Perform")]
        public void Perform()
        {
            if (loader == null)
                loader = GetComponent<StateMachineLoader>();

            loader.Load();
            Load();
        }
    }
}

