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

        #region Fields
        public string name;
        public string category;
        public FuzzyShapeType type;
        public Color color = Color.white;

        public float[] shape;

        [HideInInspector] public int index;
        [HideInInspector] public float min; // Lowest we can go
        [HideInInspector] public float max; // Highest we can go
        [HideInInspector] public float slope; // Slope calculated and halfed.
        [HideInInspector] public float lastCheck = 0;
        #endregion

        /// <summary>
        /// Create a fuzzy member.
        /// </summary>
        /// <param name="a_name">Its name</param>
        /// <param name="a_cat">The category</param>
        /// <param name="a_type">The type input</param>
        /// <param name="a_color">The member line colour</param>
        /// <param name="a_shape">The shape array</param>
        public FuzzyMember(string a_name, string a_cat, FuzzyShapeType a_type, Color a_color, params float[] a_shape)
        {
            // How big should it be.
            int count = (a_type == FuzzyShapeType.Singleton) ? 1 :
                ((a_type == FuzzyShapeType.Triangle) ? 3 : 4);

            // If it's shape count is below the required amount it will fail.
            if (a_shape.Length < count)
            {
                throw new System.Exception($"Your shape type {a_type.ToString()} does not have enough graph points {a_shape.Length} / {count}");
            }

            name = a_name;
            category = a_cat;
            type = a_type;
            color = a_color;

            shape = a_shape;
            min = Mathf.Min(shape);
            max = Mathf.Max(shape);

            // Gets the desired slope
            slope = type == FuzzyShapeType.LeftShoulder ? GetSlope(shape[2], 0, shape[3], 1) :
                (type == FuzzyShapeType.RightShoulder ? GetSlope(shape[0], 1, shape[1], 0) : 0);
        }

        /// <summary>
        /// Visualise the membership shape
        /// </summary>
        /// <returns>A vector2 array of visualization</returns>
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

        /// <summary>
        /// Grab the membership of a value for a trapezium.
        /// </summary>
        /// <param name="x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetTrapeziumMembership(float x)
        {
            return 0;
        }

        /// <summary>
        /// Grab the membership of a value for a singleton.
        /// </summary>
        /// <param name="a_x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetSingletonMembership(float a_x)
        {
            return a_x == shape[0] ? 1 : 0;
        }

        /// <summary>
        /// Grab the membership of a value for a left shoulder.
        /// </summary>
        /// <param name="a_x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetLeftShoulderMembership(float a_x)
        {
            float result;
            if (a_x <= min)
            {
                result = 1;
            }
            else if (a_x >= max)
            {
                result = 0;
            }
            else
            {
                result = (a_x / (max - min)) - (min / (max - min));
            }

            return result;
        }

        /// <summary>
        /// Center of the left shoulder shape.
        /// </summary>
        public float GetLeftShoulderBarCenter()
        {
            return (max - shape[2]) + slope;
        }

        /// <summary>
        /// Grab the membership of a value for a right shoulder.
        /// </summary>
        /// <param name="a_x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetRightShoulderMembership(float a_x)
        {
            float result;
            if (a_x <= min)
            {
                result = 0;
            }
            else if (a_x >= max)
            {
                result = 1;
            }
            else
            {
                result = -(a_x / (max - min)) + (min / (max - min)); // bye bye negative
            }

            return result;
        }

        /// <summary>
        /// Center of the right shoulder shape.
        /// </summary>
        public float GetRightShoulderBarCenter()
        {
            return (shape[1] + (max - shape[1])/2) - slope;
        }

        /// <summary>
        /// Grab the membership of a value for a triangle.
        /// </summary>
        /// <param name="a_x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetTriangleMembership(float a_x)
        {
            float absolute = shape[1];

            float result;
            if (a_x > max || a_x < min)
            {
                result = 0;
            }
            else if (a_x < absolute)
            {
                result = (a_x - min) / (absolute - min);
            }
            else if (a_x > absolute)
            {
                result = (max - a_x) / (max - absolute);
            }
            else
            {
                result = 1;
            }

            return result;
        }

        /// <summary>
        /// Center of the shape triangle.
        /// </summary>
        /// <returns>The middle of the triangle</returns>
        public float GetTriangleCenter()
        {
            return shape[1];
        }

        /// <summary>
        /// Get the center of the current shapetype.
        /// </summary>
        /// <returns>A calculated center of the shapetype.</returns>
        public float GetCenter()
        {
            return type == FuzzyShapeType.LeftShoulder ? GetLeftShoulderBarCenter() :
                (type == FuzzyShapeType.RightShoulder ? GetRightShoulderBarCenter() :
                (type == FuzzyShapeType.Triangle ? GetTriangleCenter() : max));
        }

        /// <summary>
        /// Get's the slope of the bar, very useful for perfect center calculation.
        /// </summary>
        public float GetSlope(float a_x1, float a_y1, float a_x2, float a_y2)
        {
            return (Mathf.Abs((a_y2 - a_y1) / (a_x2 - a_x1)) * 100) / 2;
        }

        /// <summary>
        /// Grab the membership of the current shape type.
        /// </summary>
        /// <param name="a_x">Crisp input</param>
        /// <returns>The membership</returns>
        public float GetMembership(float a_x)
        {
            lastCheck = Mathf.Abs(type == FuzzyShapeType.LeftShoulder ? GetLeftShoulderMembership(a_x) :
                (type == FuzzyShapeType.RightShoulder ? GetRightShoulderMembership(a_x) :
                (type == FuzzyShapeType.Triangle ? GetTriangleMembership(a_x) : max)));

            return lastCheck;
        }
    }
}
