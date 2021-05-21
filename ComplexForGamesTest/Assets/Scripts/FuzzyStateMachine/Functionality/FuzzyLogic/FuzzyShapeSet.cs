using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [CreateAssetMenu(fileName = "New Shape Set", menuName = "FuzzyStateMachine/Graph/ShapeSet", order = 1)]
    public class FuzzyShapeSet : ScriptableObject
    {
        [Header("Must have desirability!")]
        [Tooltip("Un Desirability of doing the action.")] public Vector3 unDesirability;
        [Tooltip("Desirability of doing the action.")] public Vector3 desirability;
        [Tooltip("Very Desirability of doing the action.")] public Vector3 veryDesirability;
        [Space()]
        [Tooltip("Fuzzy member data used for visualizing and calculation.")] public FuzzyMember[] fuzzyMembers;

        /// <summary>
        /// Load fuzzymembersfrom a shape set
        /// </summary>
        /// <returns></returns>
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
                set.Add(new Variable.FuzzyMember(fM.name, fM.category, fM.type, fM.color, fM.shape));
            }

            return set.ToArray();
        }
    }
}
