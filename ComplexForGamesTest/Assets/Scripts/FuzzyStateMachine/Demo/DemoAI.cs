using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This little demo AI requires the use of demo variables and rules.
 */

namespace FuzzyStateMachine.Demo
{
    public class DemoAI : DemoDamageable
    {
        private StateMachineLoader fuzzyMachine;

        // TEST
        public float nearestfood = 0;
        public float nearestwater = 0;
        public float nearestbed = 0;
        public float hunger = 0;
        public float thirst = 0;
        public float exhaustion = 0;

        void Start()
        {
            fuzzyMachine = GetComponent<StateMachineLoader>();

            fuzzyMachine.Load(
                ("nearestfood", nearestfood),
                ("nearestwater", nearestwater),
                ("nearestbed", nearestbed),
                ("hunger", hunger), 
                ("exhaustion", exhaustion), 
                ("thirst", thirst), 
                ("health", GetHealth())
            );

            // Delete after thnx

            StateMachineDebugger debug = GetComponent<StateMachineDebugger>();
            debug.logic = fuzzyMachine._outPut.logic.ToArray();
            debug.debug = fuzzyMachine.logs;
            debug.unDesirability = fuzzyMachine._outPut.shapeSet.unDesirability;
            debug.desirability = fuzzyMachine._outPut.shapeSet.desirability;
            debug.veryDesirability = fuzzyMachine._outPut.shapeSet.veryDesirability;
            debug.deffuziedOutput = fuzzyMachine._outPut.deffuzied;
        }

        void Update()
        {

        }
    }
}