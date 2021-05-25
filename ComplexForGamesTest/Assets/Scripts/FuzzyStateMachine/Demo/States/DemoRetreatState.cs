using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class DemoRetreatState : StateMachineState
    {
        private Demo.DemoAI ai;

        private bool initializied = false;

        public DemoRetreatState() { executionType = StateExecuteType.Update; icon = "♥"; iconColor = Color.white; }

        public override void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            base.Execute(a_obj, a_inputVariables);

            if (!initializied)
            {
                ai = StateController.GetComponent<FuzzyStateMachine.Demo.DemoAI>();
                initializied = true;
            }

            Vector3 goal = ai.camp.position;
            goal.y = StateController.transform.position.y;

            if (Vector3.Distance(StateController.transform.position, goal) < 0.01f)
            {
                finished = true;
            }
            else
            {
                StateController.transform.LookAt(goal);
                StateController.transform.position = Vector3.MoveTowards(StateController.transform.position, goal, Time.fixedDeltaTime * 3f * Mathf.Clamp(ai.GetHealth()/ai.GetMaxHealth(), 0.3f, 1));
            }
        }

        public override Dictionary<string, float> Finish()
        {
            if (ai.camp.position != null)
            {
                ai.SetHealth(ai.GetMaxHealth());
                inputVariables["hunger"] = 1;
            }

            return inputVariables;
        }
    }
}
