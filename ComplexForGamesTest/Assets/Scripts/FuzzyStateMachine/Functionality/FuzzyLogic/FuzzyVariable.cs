using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [CreateAssetMenu(fileName = "New Variables", menuName = "FuzzyStateMachine/Graph/Variable", order = 1)]
    public class FuzzyVariable : ScriptableObject
    {
        [System.Serializable]
        public struct Variables
        {
            public string name;
            public float input;
        }

        public Variables[] variables;

        internal Dictionary<string, float> m_variables = new Dictionary<string, float>(); // Our inputs.

        public void Init(params (string name, float input)[] a_inputs)
        {
            foreach(Variables var in variables)
            {
                float a = var.input;

                for (int i = 0; i < a_inputs.Length; i++)
                {
                    if (a_inputs[i].name == var.name)
                    {
                        a = a_inputs[i].input;
                        break;
                    }
                }

                this[var.name] = a;
            }
        }

        public Dictionary<string, float> Get()
        {
            return m_variables;
        }

        /// <summary>
        /// Get a variable by name.
        /// </summary>
        /// <param name="a_name">Variable name</param>
        public float GetVariable(string a_name)
        {
            return this[a_name];
        }

        /// <summary>
        /// Set a variable (used as an input Category in FuzzyRules)
        /// </summary>
        /// <param name="a_name">Variable name</param>
        /// <param name="a_input">Variable input</param>
        public void SetVariable(string a_name, float a_input)
        {
            this[a_name] = a_input;
        }

        public float this[string index]
        {
            get
            {
                return m_variables[index];
            }
            set
            {
                m_variables[index] = value;

                //Debug.Log($"{index} = {value}");
            }
        }

    }
}