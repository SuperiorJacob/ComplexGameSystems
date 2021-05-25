using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* This little demo AI requires the use of demo variables and rules.
 */

namespace FuzzyStateMachine.Demo
{
    public class DemoAI : DemoDamageable
    {
        private StateMachineLoader _fuzzyMachine;
        private StateMachineDebugger _debug;

        // TEST
        public float simulationSpeed = 1;

        public float hunger = 0;
        public float courage = 0;
        public float leadership = 0;
        public float maxTeammates = 5;
        public float teammates = 5;

        public Transform camp;
        public Transform target;
        public Text symbol;

        public Dictionary<string, float> variables;

        [System.NonSerialized] public float overrideHealth = 0;

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

            float tm = (teammates / maxTeammates);

            _fuzzyMachine.Load(
                ("hunger", hunger),
                ("courage", courage),
                ("leadership", leadership),
                ("teammates", tm * 100),
                ("health", GetHealth())
            );

            variables = new Dictionary<string, float>
            {
                ["hunger"] = hunger / 100,
                ["courage"] = courage / 100,
                ["leadership"] = leadership / 100,
                ["teammates"] = tm,
                ["health"] = GetHealth() / 100
            };

            if (TryGetComponent(out _debug))
            {
                _debug.Load();
            }

            string item = _fuzzyMachine._outPut.state == null ? "" : _fuzzyMachine._outPut.state.icon;
            Color col = _fuzzyMachine._outPut.state == null ? Color.white : _fuzzyMachine._outPut.state.iconColor;

            if (leadership > 60)
            {
                item = "§\n" + item;
            }
            symbol.fontSize = 100;
            symbol.text = item;
            symbol.color = col;
        }

        void Start()
        {
            _fuzzyMachine = GetComponent<StateMachineLoader>();

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
                if (_fuzzyMachine._outPut.state == null)
                {
                    Begin();
                }
                else if (_fuzzyMachine._outPut.state.finished)
                {
                    variables = _fuzzyMachine._outPut.state.Finish();

                    UpdateBasedOnVariables();

                    Begin();
                }

                variables["hunger"] -= 0.05f * simulationSpeed;

                if (variables["hunger"] < 0) variables["hunger"] = 0;

                if (variables["hunger"] == 0)
                {
                    TakeDamage(1);
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

            if (_fuzzyMachine._outPut.state != null && _fuzzyMachine._outPut.state.executionType == States.StateExecuteType.Update)
            {
                _fuzzyMachine._outPut.state.Execute(gameObject, variables);
            }
        }

    }
}
