using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    public class FuzzyRuleSet
    {
        #region Fields

        /// <summary>
        /// How important is this rule set?
        /// </summary>
        public float weight = 1;

        /// <summary>
        /// Rule Category, Rule Input
        /// </summary>
        public Dictionary<string, float> inputs;

        /// <summary>
        /// Fuzzy Variables in this ruleset
        /// </summary>
        public Dictionary<int, Variable.FuzzyMember> variables = new Dictionary<int, Variable.FuzzyMember>();

        /// <summary>
        /// The variable lookup table for variables.
        /// </summary>
        public Dictionary<string, int> variablesIndexed = new Dictionary<string, int>();

        /// <summary>
        /// The cattegory lookup for variables
        /// </summary>
        public Dictionary<string, int> categories = new Dictionary<string, int>();

        #endregion

        public FuzzyRuleSet() { }

        /// <summary>
        /// Load a new membership shape from construction
        /// </summary>
        /// <param name="a_members">Membership shape</param>
        public FuzzyRuleSet(params Variable.FuzzyMember[] a_members)
        {
            NewSet(a_members);
        }

        /// <summary>
        /// Setup this rules weight
        /// </summary>
        public virtual void SetupWeight()
        {
            weight = 1;
        }

        /// <summary>
        /// Load a membership shape
        /// </summary>
        /// <param name="a_members">Membership shape</param>
        public void NewSet(params Variable.FuzzyMember[] a_members)
        {
            SetupWeight();

            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }
        }

        /// <summary>
        /// Simple indexing for implementation of multiple lookups
        /// </summary>
        /// <param name="a_index">The index to lookup / create</param>
        /// <returns>The fuzzy member</returns>
        public Variable.FuzzyMember this[int a_index]
        {
            get 
            {
                return variables[a_index]; 
            }
            set
            {
                //Debug.Log($"IND: {index}. NAME: {value.name}. CAT: {value.category}");

                value.index = a_index;
                variables[a_index] = value;
                variablesIndexed[value.name] = a_index;

                if (!categories.TryGetValue(value.category, out int catCount))
                {
                    categories[value.category] = 1;
                }
                else
                {
                    categories[value.category] = catCount + 1;
                }
            }
        }

        /// <summary>
        /// Lookup table for string that finds the int indexer.
        /// </summary>
        /// <param name="a_index">String index</param>
        /// <returns>Fuzzy Member</returns>
        public Variable.FuzzyMember this[string a_index]
        {
            get
            {
                return variables[variablesIndexed[a_index]];
            }
        }

        /// <summary>
        /// Get the members by category.
        /// </summary>
        /// <returns>Every member in a dictionary categorised</returns>
        public Dictionary<string, Dictionary<int, Variable.FuzzyMember>> GetMembersByCategory()
        {
            Dictionary<string, Dictionary<int, Variable.FuzzyMember>> membersByCat = new Dictionary<string, Dictionary<int, Variable.FuzzyMember>>();

            // Category declaration.
            foreach (KeyValuePair<string, int> cat in categories)
            {
                membersByCat[cat.Key] = new Dictionary<int, Variable.FuzzyMember>();
            }

            // Member declaration in the category.
            foreach (KeyValuePair<int, Variable.FuzzyMember> member in variables)
            {
                Variable.FuzzyMember memb = member.Value;
                membersByCat[memb.category][memb.index] = memb;
            }

            return membersByCat;
        }

        /// <summary>
        /// The membership of said member
        /// </summary>
        /// <param name="a_member">Membership of member graph</param>
        /// <returns>Float memberhsip</returns>
        public float Is(string a_member)
        {
            Variable.FuzzyMember member = this[a_member];

            return member.GetMembership(inputs[member.category]);
        }

        /// <summary>
        /// Rule a bunch of IS rules.
        /// </summary>
        /// <param name="a_rules">Rules to calculate</param>
        /// <returns>Max of the rules</returns>
        public float Rule(params float[] a_rules)
        {
            return Mathf.Max(a_rules);
        }

        /// <summary>
        /// Get the membership of a value.
        /// </summary>
        /// <param name="a_name">Membership name</param>
        /// <param name="a_pass">The pass of it</param>
        /// <returns>Membership float</returns>
        public float GetMembership(string a_name, float a_pass)
        {
            return (this[a_name] != null) ? this[a_name].GetMembership(a_pass) : 0f;
        }

        /// <summary>
        /// Undesirability of a ruleset
        /// </summary>
        public virtual float UnDesirableRules(out float a_max)
        {
            a_max = 0.1f;
            return 0.1f;
        }

        /// <summary>
        /// Desirability of a ruleset
        /// </summary>
        public virtual float DesirableRules(out float a_max)
        {
            a_max = 0.5f;
            return 0.5f;
        }

        /// <summary>
        /// Very Desirability of a ruleset
        /// </summary>
        public virtual float VeryDesirableRules(out float a_max)
        {
            a_max = 0.9f;
            return 0.9f;
        }

        /// <summary>
        /// Check if there is enough in the desire category.
        /// </summary>
        public void CheckDesireCategory()
        {
            if (categories.TryGetValue("desire", out int desireCount))
            {
                if (desireCount < 1)
                    throw new System.Exception($"You have not setup the required DESIRE category!");
            }
        }

        /// <summary>
        /// Calculate the desirability and outputs in our ruleset
        /// </summary>
        public void Calculate(out float a_unDesirable, out float a_desirable, out float a_veryDesirable, out float a_maxUnDesirable, out float a_maxDesirable, out float a_maxVeryDesirable, Dictionary<string, float> a_inputs)
        {
            inputs = a_inputs;

            a_unDesirable = UnDesirableRules(out a_maxUnDesirable);
            a_desirable = DesirableRules(out a_maxDesirable);
            a_veryDesirable = VeryDesirableRules(out a_maxVeryDesirable);
        }
    }
}
