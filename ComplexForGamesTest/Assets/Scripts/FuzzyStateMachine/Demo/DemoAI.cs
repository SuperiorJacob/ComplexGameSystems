using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This little demo AI requires the use of demo variables and rules.
 */

namespace FuzzyStateMachine.Demo
{
    public class DemoAI : DemoDamageable
    {
        private StateMachineLoader fuzzyMachine;
        private StateMachineDebugger debug;

        // TEST
        public float simulationSpeed = 1;

        public float hunger = 100;
        public float thirst = 100;
        public float exhaustion = 100;

        public float nearestfood = 100;
        public float nearestwater = 100;
        public float nearestbed = 100;

        public Transform food;
        public Transform water;
        public Transform bed;

        public Dictionary<string, float> variables;

        private bool finished = false;

        public void UpdateBasedOnVariables()
        {
            nearestfood = variables["nearestfood"] * 100;
            nearestwater = variables["nearestwater"] * 100;
            nearestbed = variables["nearestbed"] * 100;
            hunger = variables["hunger"] * 100;
            exhaustion = variables["exhaustion"] * 100;
            thirst = variables["thirst"] * 100;
            SetHealth((int)(variables["health"] * 100));

            if (hunger < 0) hunger = 0;
            if (thirst < 0) thirst = 0;
            if (exhaustion < 0) exhaustion = 0;
        }

        public void Begin()
        {
            finished = false;

            fuzzyMachine = GetComponent<StateMachineLoader>();

            fuzzyMachine.Load(
                ("nearestfood", nearestfood),
                ("nearestwater", nearestwater),
                ("nearestbed", nearestbed),
                ("hunger", hunger),
                ("exhaustion", exhaustion),
                ("thirst", thirst),
                ("health", GetHealth())
            );

            variables = new Dictionary<string, float>();
            variables["nearestfood"] = nearestfood / 100;
            variables["nearestwater"] = nearestwater / 100;
            variables["nearestbed"] = nearestbed / 100;
            variables["hunger"] = hunger / 100;
            variables["exhaustion"] = exhaustion / 100;
            variables["thirst"] = thirst / 100;
            variables["health"] = GetHealth()/GetMaxHealth();

            if (TryGetComponent(out debug))
            {
                debug.Load();
            }
        }

        void Start()
        {
            StartCoroutine("UpdateData");
        }


        [ContextMenu("Update Values")]
        public void UpdateValues()
        {
            Begin();
        }

        IEnumerator UpdateData()
        {
            while (true)
            {
                hunger -= 0.5f * simulationSpeed;
                thirst -= 0.8f * simulationSpeed;

                if (hunger < 0) hunger = 0;
                if (thirst < 0) thirst = 0;
                if (exhaustion < 0) exhaustion = 0;

                if (thirst == 0 || hunger == 0)
                {
                    TakeDamage(thirst == 0 && hunger == 0 ? 10 : 1);
                }

                if (!finished)
                {
                    exhaustion -= 1 * simulationSpeed;

                    nearestfood = Vector3.Distance(transform.position, food.position);
                    nearestwater = Vector3.Distance(transform.position, water.position);
                    nearestbed = Vector3.Distance(transform.position, bed.position);

                    Begin();
                }
                else
                {
                    yield return new WaitForSeconds(1f);

                    Begin();
                }

                yield return new WaitForSeconds(1f);
            }
        }

        void Update()
        {
            if (finished)
                return;

            if (fuzzyMachine._outPut.state != null && fuzzyMachine._outPut.state.executionType == States.StateExecuteType.Update)
            {
                if (fuzzyMachine._outPut.state.finished)
                {
                    variables = fuzzyMachine._outPut.state.Finish();
                    UpdateBasedOnVariables();
                    finished = true;
                }
                else
                    fuzzyMachine._outPut.state.Execute(gameObject, variables);
            }
        }
    }
}