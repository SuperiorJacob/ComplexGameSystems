﻿using System.Collections;
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
        public GameObject StateController { get; private set; }
        public Dictionary<string, float> inputVariables;
        public bool finished = false;
        public StateExecuteType executionType = StateExecuteType.Update;

        public StateMachineState() { }

        public virtual void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            inputVariables = a_inputVariables;

            if (a_obj != StateController) 
                StateController = a_obj;
        }

        public virtual Dictionary<string, float> Finish()
        {
            return inputVariables;
        }
    }
}