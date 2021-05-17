using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class DemoAttackState : StateMachineState
    {
        private Demo.DemoAI ai;
        private Demo.DemoAI target;
        private bool initializied = false;

        public DemoAttackState() { executionType = StateExecuteType.Update; }

        public override void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            base.Execute(a_obj, a_inputVariables);

            if (!initializied)
            {
                ai = StateController.GetComponent<FuzzyStateMachine.Demo.DemoAI>();
                ai.GetTarget();

                initializied = true;

                if (ai.target != null)
                    target = ai.target.GetComponent<Demo.DemoAI>();
            }

            Vector3 goal = (target == null) ? ai.camp.position : ai.target.position;

            StateController.transform.LookAt(goal);
            StateController.transform.position = Vector3.MoveTowards(StateController.transform.position, goal, Time.fixedDeltaTime * 10f);

            if ((StateController.transform.position - goal).sqrMagnitude < 0.001)
            {
                Debug.Log($"Just Attacked {StateController.gameObject}");
                finished = true;
            }
        }

        public override Dictionary<string, float> Finish()
        {
            if (target != null)
            {
                target.TakeDamage(10);
            }

            return inputVariables;
        }
    }
}