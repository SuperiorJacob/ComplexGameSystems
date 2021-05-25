using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public enum StateExecuteType
    {
        RunTime,
        Update,
        FixedUpdate,
    }

    public class StateMachineState
    {
        #region Fields

        /// <summary>
        /// Current state controller aka the game object using this state.
        /// </summary>
        public GameObject StateController { get; private set; }

        /// <summary>
        /// The input variables that will be used and outputted by the state possibly?
        /// </summary>
        public Dictionary<string, float> inputVariables;

        /// <summary>
        /// How will the state execute?
        /// </summary>
        public StateExecuteType executionType = StateExecuteType.Update;

        /// <summary>
        /// The colour of the cool icon above the AI when the state is active.
        /// </summary>
        public Color iconColor = Color.white;

        /// <summary>
        /// The icon above the AI when the state is active.
        /// </summary>
        public string icon = "";

        public bool finished = false;

        #endregion

        public StateMachineState() { }

        /// <summary>
        /// Called every @ExecutionType and will take in these inputs.
        /// </summary>
        /// <param name="a_obj">The gameobject executing this</param>
        /// <param name="a_inputVariables">Te variables required in this execution</param>
        public virtual void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            inputVariables = a_inputVariables;

            if (a_obj != StateController) 
                StateController = a_obj;
        }

        /// <summary>
        /// Complete the state task, do something.
        /// </summary>
        /// <returns>The output data</returns>
        public virtual Dictionary<string, float> Finish()
        {
            return inputVariables;
        }
    }
}
