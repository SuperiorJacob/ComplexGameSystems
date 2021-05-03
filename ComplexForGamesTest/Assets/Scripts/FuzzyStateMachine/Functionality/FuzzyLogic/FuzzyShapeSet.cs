using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [CreateAssetMenu(fileName = "New Shape Set", menuName = "FuzzyStateMachine/Graph/ShapeSet", order = 1)]
    public class FuzzyShapeSet : ScriptableObject
    {
        [Header("Must have desirability!")]
        public Vector3 unDesirability;
        public Vector3 desirability;
        public Vector3 veryDesirability;
        [Space()]
        public FuzzyMember[] fuzzyMembers;

        public virtual void Init()
        {

        }

        public FuzzyMember[] LoadShapeSet()
        {
            List<FuzzyMember> set = new List<FuzzyMember>()
            {
                new Variable.FuzzyMember("undesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.red, unDesirability.x, unDesirability.x, unDesirability.y, unDesirability.z),
                new Variable.FuzzyMember("desirable", "desire", Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, desirability.x, desirability.y, desirability.z),
                new Variable.FuzzyMember("verydesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.green, veryDesirability.x, veryDesirability.y, veryDesirability.z, veryDesirability.z)
            };

            foreach (FuzzyMember fM in fuzzyMembers)
            {
                set.Add(fM);
            }

            return set.ToArray();
        }
    }
}