using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class DemoRetreatState : StateMachineState
    {
        private Demo.DemoAI ai;
        private bool initializied = false;

        public DemoRetreatState() { executionType = StateExecuteType.Update; }

        public override void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            base.Execute(a_obj, a_inputVariables);

            if (!initializied)
            {
                ai = StateController.GetComponent<FuzzyStateMachine.Demo.DemoAI>();
                initializied = true;
            }

            StateController.transform.LookAt(ai.camp.position);
            StateController.transform.position = Vector3.MoveTowards(StateController.transform.position, ai.camp.position, Time.fixedDeltaTime * 10f);

            if (Vector3.Distance(StateController.transform.position, ai.camp.position) < 0.1f)
            {
                finished = true;
            }
        }

        public override Dictionary<string, float> Finish()
        {
            if (ai.camp.position != null)
            {
                inputVariables["health"] = 100;
                inputVariables["hunger"] = 1;
            }

            return inputVariables;
        }
    }
}