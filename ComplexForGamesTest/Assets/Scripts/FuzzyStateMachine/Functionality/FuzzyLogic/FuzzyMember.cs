using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Variable
{
    [System.Serializable]
    public class FuzzyMember
    {
        public enum FuzzyShapeType
        {
            Singleton,
            LeftShoulder,
            RightShoulder,
            Triangle,
            Trapezium
        }

        public string name;
        public string category;
        public float[] shape;
        public FuzzyShapeType type;
        public Color color = Color.white;

        [HideInInspector] public int index; // Used by rules.
        [HideInInspector] public float min;
        [HideInInspector] public float max;
        [HideInInspector] public float lastCheck = 0;

        private System.Func<float, float>[] memberShipFunctions;

        public FuzzyMember(string a_name, string a_cat, FuzzyShapeType a_type, Color a_color, params float[] a_shape)
        {
            int count = (a_type == FuzzyShapeType.Singleton) ? 1 :
                ((a_type == FuzzyShapeType.Triangle) ? 3 : 4);

            if (a_shape.Length < count)
            {
                throw new System.Exception($"Your shape type {a_type.ToString()} does not have enough graph points {a_shape.Length} / {count}"); // It's beautiful :)
            }

            name = a_name;
            category = a_cat;
            type = a_type;
            color = a_color;
            shape = new float[count];

            for (int i = 0; i < count; i++)
            {
                shape[i] = a_shape[i];
            }

            min = Mathf.Min(shape);
            max = Mathf.Max(shape);

            // Cursed :) Please fix this later... probably just use if statements...
            memberShipFunctions = new System.Func<float, float>[5];

            memberShipFunctions[(int)FuzzyShapeType.Singleton] = GetSingletonMembership;
            memberShipFunctions[(int)FuzzyShapeType.LeftShoulder] = GetLeftShoulderMembership;
            memberShipFunctions[(int)FuzzyShapeType.RightShoulder] = GetRightShoulderMembership;
            memberShipFunctions[(int)FuzzyShapeType.Triangle] = GetTriangleMembership;
            memberShipFunctions[(int)FuzzyShapeType.Trapezium] = GetTrapeziumMembership;
        }

        public Vector2[] Visualise()
        {
            Vector2[] visualized = new Vector2[0];

            if (type == FuzzyShapeType.LeftShoulder)
            {
                visualized = new Vector2[] 
                { 
                    new Vector2(shape[0]/10, 0.8f),
                    new Vector2(shape[2]/10, 0.8f),
                    new Vector2(shape[3]/10, -1)
                };
            }
            else if (type == FuzzyShapeType.RightShoulder)
            {
                visualized = new Vector2[]
                {
                    new Vector2(shape[0]/10, -1),
                    new Vector2(shape[1]/10, 0.8f),
                    new Vector2(shape[3]/10, 0.8f)
                };
            }
            else if (type == FuzzyShapeType.Triangle)
            {
                visualized = new Vector2[]
                {
                    new Vector2(shape[0]/10, -1),
                    new Vector2(shape[1]/10, 0.8f),
                    new Vector2(shape[2]/10, -1),
                };
            }

            return visualized;
        }

        public float GetTrapeziumMembership(float x)
        {
            return x / shape[0];
        }

        public float GetSingletonMembership(float x)
        {
            return x / shape[0];
        }

        public float GetLeftShoulderMembership(float x)
        {
            float result;
            if (x <= min)
            {
                result = 1;
            }
            else if (x >= max)
            {
                result = 0;
            }
            else
            {
                result = (x / (max - min)) - (min / (max - min));
            }

            return result;
        }

        public float GetLeftShoulderBarCenter()
        {
            return ((max - shape[2]) / 2) + 2.5f;
        }

        public float GetRightShoulderMembership(float x)
        {
            float result;
            if (x <= min)
            {
                result = 0;
            }
            else if (x >= max)
            {
                result = 1;
            }
            else
            {
                result = -(x / (max - min)) + (min / (max - min)); // bye bye negative
            }

            return result;
        }

        public float GetRightShoulderBarCenter()
        {
            return (shape[1] + (max - shape[1])/2) - 2.5f;
        }

        public float GetTriangleMembership(float x)
        {
            float absolute = shape[1];

            float result;
            if (x > max || x < min)
            {
                result = 0;
            }
            else if (x < absolute)
            {
                result = (x - min) / (absolute - min);
            }
            else if (x > absolute)
            {
                result = (max - x) / (max - absolute);
            }
            else
            {
                result = 1;
            }

            return result;
        }

        public float GetTriangleCenter()
        {
            return shape[1];
        }

        public float GetCenter()
        {
            return type == FuzzyShapeType.LeftShoulder ? GetLeftShoulderBarCenter() :
                (type == FuzzyShapeType.RightShoulder ? GetRightShoulderBarCenter() :
                (type == FuzzyShapeType.Triangle ? GetTriangleCenter() : max));
        }

        public float GetMembership(float x)
        {
            lastCheck = Mathf.Abs(memberShipFunctions[(int)type](x));
            return lastCheck;
        }

        public float GetMaxMembership()
        {
            return Mathf.Max(shape);
        }

        public float GetMinMembership()
        {
            return Mathf.Min(shape);
        }
    }
}