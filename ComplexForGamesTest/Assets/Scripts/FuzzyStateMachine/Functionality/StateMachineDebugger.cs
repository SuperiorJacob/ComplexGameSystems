using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyStateMachine
{
    [RequireComponent(typeof(StateMachineLoader))]
    public class StateMachineDebugger : MonoBehaviour
    {
        #region Fields
        [Header("Desire")]
        [Tooltip("The shape of quality possessed by our ruleset that should be avoided.")] public Vector3 unDesirability;
        [Tooltip("The shape of desired quality of the ruleset.")] public Vector3 desirability;
        [Tooltip("The shape of quality that is the most desired from the ruleset.")] public Vector3 veryDesirability;
        [Tooltip("The logic's calculation of all desirability.")] public float deffuziedOutput;

        /// <summary>
        /// Output logs of our Loaders calculations.
        /// </summary>
        [HideInInspector] public List<string> debug;

        /// <summary>
        /// Decided logic output from the previous calculation.
        /// </summary>
        [HideInInspector] public FuzzyLogic[] logic;

        /// <summary>
        /// Output data from our previous calculation.
        /// </summary>
        [HideInInspector] public StateMachineLoader.FunctionData mainData;

        private StateMachineLoader _loader;
        #endregion

        /// <summary>
        /// Grab all data out of the loader to debug.
        /// </summary>
        public void Load()
        {
            // It's ugly but loads all the data into our fields so the visualiser can read it and can other apps.
            if (_loader == null) 
                _loader = GetComponent<StateMachineLoader>();

            mainData = _loader._outPut;
            logic = mainData.logic.ToArray();
            debug = _loader.logs;
            unDesirability = mainData.shapeSet.unDesirability;
            desirability = mainData.shapeSet.desirability;
            veryDesirability = mainData.shapeSet.veryDesirability;
            deffuziedOutput = mainData.deffuzied;
        }

        /// <summary>
        /// Perform loader execution and load debug data.
        /// </summary>
        [ContextMenu("Perform")]
        public void Perform()
        {
            if (_loader == null)
                _loader = GetComponent<StateMachineLoader>();

            _loader.Load();
            Load();
        }
    }
}

