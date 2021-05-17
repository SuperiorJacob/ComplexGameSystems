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

        public float hunger = 0;
        public float courage = 0;
        public float leadership = 0;
        public float maxTeammates = 5;
        public float teammates = 5;

        public Transform camp;
        public Transform target;

        public Dictionary<string, float> variables;

        [System.NonSerialized] public float overrideHealth = 0;

        private bool finished = false;

        public void UpdateBasedOnVariables()
        {
            hunger = variables["hunger"] * 100;
            courage = variables["courage"] * 100;
            leadership = variables["leadership"] * 100;

            if (hunger < 0) hunger = 0;
        }

        public void Begin()
        {
            GetTarget();

            teammates = GameObject.FindGameObjectsWithTag(gameObject.tag).Length;
            
            courage = Mathf.Clamp(
                (leadership > 60 && teammates < (maxTeammates / 2) ? 100 // If is a leader, and teammates are low, enrage.
                : ((teammates / maxTeammates) + (GetHealth() / GetMaxHealth())) / 2) * (1 + GetLeaders()) * 100, // if not leader, courage is dependant.
                0, 100); 

            finished = false;

            fuzzyMachine = GetComponent<StateMachineLoader>();

            float tm = (teammates / maxTeammates);

            fuzzyMachine.Load(
                ("hunger", hunger),
                ("courage", courage),
                ("leadership", leadership),
                ("teammates", tm * 100),
                ("health", GetHealth())
            );

            variables = new Dictionary<string, float>();
            variables["hunger"] = hunger / 100;
            variables["courage"] = courage / 100;
            variables["leadership"] = leadership / 100;
            variables["teammates"] = tm;
            variables["health"] = GetHealth() / 100;

            if (TryGetComponent(out debug))
            {
                debug.Load();
            }
        }

        void Start()
        {
            maxTeammates = GameObject.FindGameObjectsWithTag(gameObject.tag).Length;
            teammates = maxTeammates;

            // This reassures we wont have more then 1 full leader in a group.
            leadership = Random.Range(0, 100);

            StartCoroutine("UpdateData");
        }


        [ContextMenu("Update Values")]
        public void UpdateValues()
        {
            Begin();
        }

        public (float reach, Transform enemy) GetTarget()
        {
            float max = 10000;
            Transform t = null;

            foreach (DemoAI agent in FindObjectsOfType<DemoAI>())
            {
                if (agent.tag != gameObject.tag)
                {
                    float dist = Vector3.Distance(agent.transform.position, transform.position);
                    if (dist < max)
                    {
                        max = dist;
                        t = agent.transform;
                    }
                }
            }

            target = t;

            return (max, t);
        }

        public float GetLeaders()
        {
            float leaders = 0;
            foreach (DemoAI agent in FindObjectsOfType<DemoAI>())
            {
                if (!agent.IsDead && agent.tag == gameObject.tag)
                {
                    // If our leadership is greater then 60% then they are most likely a leader.
                    if (agent.GetHealth() > 0 && agent.leadership > 60)
                    {
                        leaders += 0.25f;
                    }
                }
            }

            return leaders;
        }

        IEnumerator UpdateData()
        {
            while (true)
            {
                hunger -= 0.01f * simulationSpeed;

                if (hunger < 0) hunger = 0;

                if (hunger == 0)
                {
                    TakeDamage(1);
                }

                if (!finished)
                {
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
            if (GetHealth() <= 0)
            {
                Destroy(gameObject);
                return;
            }

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