using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fix later
namespace FuzzyStateMachine
{
    public class FuzzyLogic
    {
        #region Fields

        /// <summary>
        /// The rule loaded into logic
        /// </summary>
        public FuzzyRuleSet rule;

        /// <summary>
        /// Output variables used by logic
        /// </summary>
        public Variable.FuzzyVariable variables;

        /// <summary>
        /// Desirability outputs from shape set
        /// </summary>
        public float unDesirable, desirable, veryDesirable, maxUnDesirable, maxDesirable, maxVeryDesirable, lastDefuzz;

        #endregion

        public FuzzyLogic() {}

        /// <summary>
        /// Load a ruleset
        /// </summary>
        /// <param name="a_rule">The ruleset to load</param>
        public FuzzyLogic(FuzzyRuleSet a_rule)
        {
            rule = a_rule;
        }

        /// <summary>
        /// Fuzzy AND (x AND y)
        /// </summary>
        /// <returns>Minimum value</returns>
        public static float AND(params float[] a_vars)
        {
            return Mathf.Min(a_vars);
        }

        /// <summary>
        /// Fuzzy OR (x OR y)
        /// </summary>
        /// <returns>Maximum value</returns>
        public static float OR(params float[] a_vars)
        {
            return Mathf.Max(a_vars);
        }

        /// <summary>
        /// Fuzzy NOT (opposite of X)
        /// </summary>
        /// <returns>Opposite of value</returns>
        public static float NOT(float a_a)
        {
            return (1 - a_a);
        }

        /// <summary>
        /// Calculate our rules using defined variables.
        /// </summary>
        public void Calculate()
        {
            rule.Calculate(out unDesirable, out desirable, out veryDesirable, out maxUnDesirable, out maxDesirable, out maxVeryDesirable, variables.Get());
        }

        /// <summary>
        /// Calculate the rules using overrided crisp values
        /// </summary>
        /// <param name="a_crispSet">The override crisp values</param>
        public void Calculate(Dictionary<string, float> a_crispSet)
        {
            rule.Calculate(out unDesirable, out desirable, out veryDesirable, out maxUnDesirable, out maxDesirable, out maxVeryDesirable, a_crispSet);
        }

        /// <summary>
        /// Mandami deffuzification (implemented in this logic)
        /// </summary>
        /// <returns>Defuzzified values</returns>
        public virtual float Defuzzify()
        {
            float deffuz = maxUnDesirable * unDesirable + maxDesirable * desirable + maxVeryDesirable * veryDesirable;
            deffuz /= (unDesirable + desirable + veryDesirable);

            lastDefuzz = deffuz;

            return deffuz;
        }
    }
}
