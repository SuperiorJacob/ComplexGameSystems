using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    public class StateMachineDebugger : MonoBehaviour
    {
        [Tooltip("aduahuaf")]
        //public FuzzyLogic logic;
        public float[] crispSet = { 1, 3, 5, 7, 9 };
        public float[] fuzzySet;

        public Dictionary<string, float> rules = new Dictionary<string, float>();

        void TestRules()
        {
            rules["Freezing"] = 0.85f;
            rules["Cold"] = 0.6f;
            rules["Hot"] = 0.4f;
            rules["Boiling"] = 0.25f;

            foreach (float set in fuzzySet)
            {
                foreach (KeyValuePair<string, float> rule in rules)
                {
                    if (set > rule.Value)
                    {
                        Debug.Log("Set:" + set + "; Rule: " + rule.Key + " passed");
                        break;
                    }
                    else Debug.Log("Set:" + set + "; Rule: " + rule.Key + " failed");
                }
            }
        }

        void TestFuzzify()
        {
            fuzzySet = new float[crispSet.Length];

            float min = Mathf.Min(crispSet);
            float max = Mathf.Max(crispSet);

            //float range = Mathf.Abs(max - min);

            for (int i = 0; i < crispSet.Length; i++)
            {
                fuzzySet[i] = crispSet[i] / max;
            }
        }

        void Start()
        {
            TestFuzzify();
            //TestRules();
            // Set Builder Notation A = {x:p(x)}
        }

        void Update()
        {

        }
    }
}