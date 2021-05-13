using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class GetWaterState : StateMachineState
    {
        private Vector3 goal = new Vector3(45, 0, 0);

        public GetWaterState() { executionType = StateExecuteType.Update; }

        public override void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            base.Execute(a_obj, a_inputVariables);

            StateController.transform.LookAt(goal);
            StateController.transform.position = Vector3.MoveTowards(StateController.transform.position, goal, Time.deltaTime * (20 * 0.5f + a_inputVariables["exhaustion"]));

            if (Vector3.Distance(StateController.transform.position, goal) < 1)
            {
                finished = true;
            }
        }

        public override Dictionary<string, float> Finish()
        {
            inputVariables["thirst"] += 0.1f;

            return inputVariables;
        }
    }
}