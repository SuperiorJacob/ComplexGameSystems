using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fix later
namespace FuzzyStateMachine
{
    [System.Serializable]
    public struct FuzzySet
    {
        public string name; // Membership name;
        public float u; // Membership valuable.
        public float x; // Real number.
    }

    [System.Serializable]
    public struct FuzzyVariable
    {
        public string name;
        public FuzzySet[] sets;
        public float min;
        public float max;
    }

    public class InferenceRule
    {
        public virtual FuzzySet ApplyRule(FuzzySet variable)
        {


            return default;
        }
    }

    // Default inference is Mamdami.
    public class Inference
    {
        private InferenceRule rule;
        private FuzzyVariable[] variables;

        public virtual void Fuzzify(FuzzyVariable a_variables)
        {

        }

        public virtual void Input(InferenceRule a_rule, params FuzzyVariable[] a_variables)
        {
            rule = a_rule;
            variables = a_variables;
        }

        // Change to array or something later
        public virtual FuzzySet CalculateRule()
        {
            return rule.ApplyRule(variables[0].sets[0]);
        }
    }

    public class FuzzyLogic
    {
        #region Membership
        public FuzzySet UpdateMembership(FuzzySet set)
        {
            return new FuzzySet();
        }
        public void MembershipAnd(FuzzySet a, FuzzySet b)
        {
            Mathf.Min(a.u);
        }
        #endregion

        public void Inference()
        {
            // Inputs
            // Rule Calculation
            // Output

            Inference inference = new Inference();
            inference.Input(new InferenceRule(), new FuzzyVariable(), new FuzzyVariable());
            inference.CalculateRule();

        }

        public void Init()
        {
            Inference();
        }

        public void Calculate()
        {

        }
    }
}