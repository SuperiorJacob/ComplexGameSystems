using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class DemoAttackState : StateMachineState
    {
        private Demo.DemoAI ai;
        private BoxCollider aiCollider;
        private Demo.DemoAI target;
        private BoxCollider targetCollider;

        private bool initializied = false;

        public DemoAttackState() { executionType = StateExecuteType.Update; icon = "»«"; iconColor = Color.red; }

        public override void Execute(GameObject a_obj, Dictionary<string, float> a_inputVariables)
        {
            base.Execute(a_obj, a_inputVariables);

            if (target == null)
                initializied = false;

            if (!initializied)
            {
                ai = StateController.GetComponent<FuzzyStateMachine.Demo.DemoAI>();
                ai.GetTarget();

                initializied = true;

                if (ai.target != null)
                {
                    target = ai.target.GetComponent<Demo.DemoAI>();
                    targetCollider = ai.target.GetComponentInChildren<BoxCollider>();
                    aiCollider = StateController.transform.GetComponentInChildren<BoxCollider>();
                }
            }

            if (target == null)
            {
                finished = true;
            }

            Vector3 goal = (target == null) ? ai.camp.position : ai.target.position;
            goal.y = StateController.transform.position.y;

            Vector3 forward = StateController.transform.forward * 0.8f;
            forward.y = 0;

            StateController.transform.LookAt(goal);
            StateController.transform.position = Vector3.MoveTowards(StateController.transform.position, goal - forward, Time.fixedDeltaTime * 3f * Mathf.Clamp(ai.GetHealth() / ai.GetMaxHealth(), 0.3f, 1));

            if (!finished && targetCollider != null && targetCollider.bounds.Intersects(aiCollider.bounds))
            {
                finished = true;
            }
        }

        public override Dictionary<string, float> Finish()
        {
            if (target != null)
            {
                Debug.Log($"{ai.name} Just Attacked {ai.target.name}");
                target.TakeDamage(10);
            }

            return inputVariables;
        }
    }
}
