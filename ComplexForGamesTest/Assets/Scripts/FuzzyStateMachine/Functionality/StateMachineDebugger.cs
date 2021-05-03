using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    [RequireComponent(typeof(StateMachineLoader))]
    public class StateMachineDebugger : MonoBehaviour
    {
        [Header("Test")]
        public float temperatureInput = 60;
        public float distanceInput = 60;
        public float deffuziedOutput = 0;
        public Vector3 unDesirability;
        public Vector3 desirability;
        public Vector3 veryDesirability;

        public Dictionary<string, Variable.FuzzyMember> fuzzies = new Dictionary<string, Variable.FuzzyMember>();

        public FuzzyLogic logic;

        [ContextMenu("Perform")]
        public void Perform()
        {
            // Category is extremely important as custom fuzzy rules RELY on them!
            logic = new FuzzyLogic(new Rules.Example.GetBlanket(
                    // MUST HAVE DESIRE
                    new Variable.FuzzyMember("undesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.red, unDesirability.x, unDesirability.x, unDesirability.y, unDesirability.z),
                    new Variable.FuzzyMember("desirable", "desire", Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, desirability.x, desirability.y, desirability.z),
                    new Variable.FuzzyMember("verydesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.green, veryDesirability.x, veryDesirability.y, veryDesirability.z, veryDesirability.z),

                    // OUR TWO RULES INPUTTED BELOW
                    new Variable.FuzzyMember("cold", "temperature", Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.cyan, 0, 0, 20, 40),
                    new Variable.FuzzyMember("warm", "temperature", Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, 30, 50, 70),
                    new Variable.FuzzyMember("hot", "temperature", Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.red, 50, 80, 100, 100),

                    new Variable.FuzzyMember("close", "distance", Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.cyan, 0, 0, 10, 50),
                    new Variable.FuzzyMember("medium", "distance", Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, 10, 50, 90),
                    new Variable.FuzzyMember("far", "distance", Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.red, 50, 90, 100, 100)
                ));

            logic.Calculate(new Dictionary<string, float>()
                {
                    { "temperature", temperatureInput },
                    { "distance", distanceInput  }
                }
            );
            deffuziedOutput = logic.Deffuzify();
        }
    }
}

