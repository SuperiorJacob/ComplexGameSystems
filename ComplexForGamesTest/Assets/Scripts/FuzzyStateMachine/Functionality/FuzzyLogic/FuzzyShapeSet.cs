using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [CreateAssetMenu(fileName = "New Shape Set", menuName = "FuzzyStateMachine/Graph/ShapeSet", order = 1)]
    public class FuzzyShapeSet : ScriptableObject
    {
        #region Fields
        [Header("Must have desirability!")]
        [Tooltip("The shape of quality possessed by our ruleset that should be avoided.")] public Vector3 unDesirability;
        [Tooltip("The shape of desired quality of the ruleset.")] public Vector3 desirability;
        [Tooltip("The shape of quality that is the most desired from the ruleset.")] public Vector3 veryDesirability;
        [Space()]
        [Tooltip("Fuzzy member data used for visualizing and calculation.")] public FuzzyMember[] fuzzyMembers;
        #endregion

        /// <summary>
        /// Load fuzzymembersfrom a shape set
        /// </summary>
        /// <returns></returns>
        public FuzzyMember[] LoadShapeSet()
        {
            // Load the desirability shapes
            List<FuzzyMember> set = new List<FuzzyMember>()
            {
                new Variable.FuzzyMember("undesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.red, unDesirability.x, unDesirability.x, unDesirability.y, unDesirability.z),
                new Variable.FuzzyMember("desirable", "desire", Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, desirability.x, desirability.y, desirability.z),
                new Variable.FuzzyMember("verydesirable", "desire", Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.green, veryDesirability.x, veryDesirability.y, veryDesirability.z, veryDesirability.z)
            };

            // Add in the objects shapes.
            foreach (FuzzyMember fM in fuzzyMembers)
            {
                set.Add(fM);
            }

            // Return it as an array because it's faster
            return set.ToArray();
        }
    }
}
