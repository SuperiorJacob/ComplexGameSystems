using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fix later
namespace FuzzyStateMachine
{
    public class FuzzyLogic
    {
        public FuzzyRuleSet rule;

        public float unDesirable, desirable, veryDesirable, maxUnDesirable, maxDesirable, maxVeryDesirable, lastDefuzz;

        public FuzzyLogic(FuzzyRuleSet a_rule)
        {
            rule = a_rule;
        }

        public static float AND(params float[] vars)
        {
            return Mathf.Min(vars);
        }

        public static float OR(params float[] vars)
        {
            return Mathf.Max(vars);
        }

        public static float NOT(float a)
        {
            return (1 - a);
        }

        public void Calculate(Dictionary<string, float> a_crispSet)
        {
            rule.Calculate(out unDesirable, out desirable, out veryDesirable, out maxUnDesirable, out maxDesirable, out maxVeryDesirable, a_crispSet);
        }

        public float Deffuzify()
        {
            float deffuz = maxUnDesirable * unDesirable + maxDesirable * desirable + maxVeryDesirable * veryDesirable;
            deffuz /= (unDesirable + desirable + veryDesirable);

            lastDefuzz = deffuz;

            return deffuz;
        }
    }
}