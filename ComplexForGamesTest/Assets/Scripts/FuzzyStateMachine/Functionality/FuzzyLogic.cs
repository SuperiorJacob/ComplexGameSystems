using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fix later
namespace FuzzyStateMachine
{
    public class FuzzyLogic
    {
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
    }
}