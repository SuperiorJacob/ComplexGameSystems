using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    public class FuzzyRuleSet
    {
        // Please don't hit me I like easy accessability. ;)

        // Rule Category , Rule Input
        public Dictionary<string, float> inputs;

        public Dictionary<int, Variable.FuzzyMember> variables = new Dictionary<int, Variable.FuzzyMember>();
        public Dictionary<string, int> variablesIndexed = new Dictionary<string, int>();
        public Dictionary<string, int> categories = new Dictionary<string, int>();

        public FuzzyRuleSet() { }

        public FuzzyRuleSet(params Variable.FuzzyMember[] a_members)
        {
            NewSet(a_members);
        }

        public void NewSet(params Variable.FuzzyMember[] a_members)
        {
            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }
        }

        public Variable.FuzzyMember this[int index]
        {
            get 
            {
                return variables[index]; 
            }
            set
            {
                //Debug.Log($"IND: {index}. NAME: {value.name}. CAT: {value.category}");

                value.index = index;
                variables[index] = value;
                variablesIndexed[value.name] = index;

                int catCount = 0;
                if (!categories.TryGetValue(value.category, out catCount))
                {
                    categories[value.category] = 1;
                }
                else
                {
                    categories[value.category] = catCount + 1;
                }
            }
        }

        public Variable.FuzzyMember this[string index]
        {
            get
            {
                //Debug.Log($"IND: {index}. VAR: {variablesIndexed[index]}");
                return variables[variablesIndexed[index]];
            }
        }

        public Dictionary<string, Dictionary<int, Variable.FuzzyMember>> GetMembersByCategory()
        {
            Dictionary<string, Dictionary<int, Variable.FuzzyMember>> membersByCat = new Dictionary<string, Dictionary<int, Variable.FuzzyMember>>();

            foreach (var cat in categories)
            {
                membersByCat[cat.Key] = new Dictionary<int, Variable.FuzzyMember>();
            }

            foreach (var member in variables)
            {
                Variable.FuzzyMember memb = member.Value;
                membersByCat[memb.category][memb.index] = memb;
            }

            return membersByCat;
        }

        public float Is(string a_member)
        {
            Variable.FuzzyMember member = this[a_member];

            return member.GetMembership(inputs[member.category]);
        }

        public float Rule(params float[] rules)
        {
            return Mathf.Max(rules);
        }

        public float GetMembership(string a_name, int a_pass)
        {
            return (this[a_name] != null) ? this[a_name].GetMembership(a_pass) : 0f;
        }

        public virtual float UnDesirableRules(out float a_max)
        {
            a_max = 0.1f;
            return 0.1f;
        }

        public virtual float DesirableRules(out float a_max)
        {
            a_max = 0.5f;
            return 0.5f;
        }

        public virtual float VeryDesirableRules(out float a_max)
        {
            a_max = 0.9f;
            return 0.9f;
        }

        public void Calculate(out float a_unDesirable, out float a_desirable, out float a_veryDesirable, out float a_maxUnDesirable, out float a_maxDesirable, out float a_maxVeryDesirable, Dictionary<string, float> a_inputs)
        {
            Debug.Log("Calculating rules");

            inputs = a_inputs;

            a_unDesirable = UnDesirableRules(out a_maxUnDesirable);
            a_desirable = DesirableRules(out a_maxDesirable);
            a_veryDesirable = VeryDesirableRules(out a_maxVeryDesirable);

            Debug.Log($"UnDesirable: {a_unDesirable} / {a_maxUnDesirable}");
            Debug.Log($"Desirable: {a_desirable} / {a_maxDesirable}");
            Debug.Log($"VeryDesirable: {a_veryDesirable} / {a_maxVeryDesirable}");
        }
    }
}