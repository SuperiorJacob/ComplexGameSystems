using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [CreateAssetMenu(fileName = "New Variables", menuName = "FuzzyStateMachine/Graph/Variable", order = 1)]
    public class FuzzyVariable : ScriptableObject
    {
        /// <summary>
        /// Variable inputs
        /// </summary>
        [System.Serializable]
        public struct Variables
        {
            [Tooltip("Name of the variable")] public string name;
            [Tooltip("The float input of the variable")] public float input;
        }

        [Tooltip("An array of variables to use later")] public Variables[] variables;

        // Variable inputs.
        [System.NonSerialized] private Dictionary<string, float> _variables;

        /// <summary>
        /// Initialize the variables
        /// </summary>
        /// <param name="a_inputs">Inputs to initialize with</param>
        public void Init(params (string name, float input)[] a_inputs)
        {
            _variables = new Dictionary<string, float>();

            foreach (Variables var in variables)
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

        /// <summary>
        /// Get the variable dictionary
        /// </summary>
        /// <returns>The variable dictionary</returns>
        public Dictionary<string, float> Get()
        {
            return _variables;
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

        /// <summary>
        /// Get a variable by index
        /// </summary>
        /// <param name="a_index">String to index</param>
        /// <returns>A float variable</returns>
        public float this[string a_index]
        {
            get
            {
                return _variables[a_index];
            }
            set
            {
                _variables[a_index] = value;

                //Debug.Log($"{index} = {value}");
            }
        }

    }
}
