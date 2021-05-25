using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using FuzzyStateMachine.Variable;

namespace FuzzyStateMachine
{
    [CustomEditor(typeof(FuzzyShapeSet))]
    [CanEditMultipleObjects] // Allows us to have multiple shapes open at once!
    public class StateMachineShapeVisualiserEditor : Editor
    {
        #region Fields

        // The loaded shape set.
        private FuzzyShapeSet _fshs;

        private Vector3[] _verticesBuffer;
        private float _minX = 0,
            _minY = -1,
            _maxX = 10,
            _maxY = 1;

        private Rect _rect;
        private float _rangeX = 10;
        private float _rangeY = 10;

        // The shapes to be used.
        private Dictionary<string, List<FuzzyMember>> _shapes;

        #endregion

        /// <summary>
        /// Plot the coordinates on the graph and lerp them too
        /// </summary>
        /// <param name="a_x">X position</param>
        /// <param name="a_y">Y position</param>
        /// <returns>Graph position</returns>
        private Vector3 UnitToGraph(float a_x, float a_y)
        {
            a_x = Mathf.Lerp(_rect.x, _rect.xMax, (a_x - _minX) / _rangeX);
            a_y = Mathf.Lerp(_rect.yMax, _rect.y, (a_y - _minY) / _rangeY);

            return new Vector3(a_x, a_y, 0);
        }

        /// <summary>
        /// Draw a visual line.
        /// </summary>
        /// <param name="a_color">The color of the line.</param>
        /// <param name="a_width">The width of the line</param>
        /// <param name="a_points">The point of each line</param>
        private void DrawLine(Color a_color, float a_width, params Vector2[] a_points)
        {
            _verticesBuffer = new Vector3[a_points.Length];

            for (int i = 0; i < a_points.Length; i++)
            {
                _verticesBuffer[i] = UnitToGraph(a_points[i].x, a_points[i].y);
            }

            Handles.color = a_color;
            Handles.DrawAAPolyLine(a_width, a_points.Length, _verticesBuffer);
        }

        /// <summary>
        /// Draw a rectangle with fill color and outline color
        /// </summary>
        /// <param name="a_x1">Left</param>
        /// <param name="a_y1">Top</param>
        /// <param name="a_x2">Right</param>
        /// <param name="a_y2">Bottom</param>
        /// <param name="a_fill">Color fill of the rectangle</param>
        /// <param name="a_line">Color oiutline of the rectangle</param>
        private void DrawRect(float a_x1, float a_y1, float a_x2, float a_y2, Color a_fill, Color a_line)
        {
            _verticesBuffer = new Vector3[4];

            _verticesBuffer[0] = UnitToGraph(a_x1, a_y1);
            _verticesBuffer[1] = UnitToGraph(a_x2, a_y1);
            _verticesBuffer[2] = UnitToGraph(a_x2, a_y2);
            _verticesBuffer[3] = UnitToGraph(a_x1, a_y2);

            Handles.DrawSolidRectangleWithOutline(_verticesBuffer, a_fill, a_line);
        }

        /// <summary>
        /// Draw each member shape
        /// </summary>
        /// <param name="a_members">The members to draw</param>
        private void Draw(List<FuzzyMember> a_members)
        {
            DrawRect(_minX, _minY, _maxX, _maxY, Color.black, Color.black);

            foreach (FuzzyMember fuz in a_members)
            {
                DrawLine(fuz.color, 2, fuz.Visualise());
            }
        }

        /// <summary>
        /// Create the shape visualiser
        /// </summary>
        /// <param name="a_name">Name of the category</param>
        /// <param name="a_todraw">Shape to draw</param>
        public void CreateVisualiser(string a_name, List<FuzzyMember> a_todraw)
        {
            // Top bar of the visualiser
            GUI.backgroundColor = Color.gray;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label(a_name);
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);
                _rect = GUILayoutUtility.GetRect(128, 80);
            }

            _rangeX = _maxX - _minX;
            _rangeY = _maxY - _minY;

            // Draw the visualed members

            Draw(a_todraw);

            // Draw the bottom bar which represents each member

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                foreach (FuzzyMember fuz in a_todraw)
                {
                    Color prev = GUI.color;
                    GUI.color = fuz.color;
                    GUILayout.Label(fuz.name);
                    GUI.color = prev;
                }

                GUILayout.Space(EditorGUI.indentLevel * 8f);
            }
        }

        /// <summary>
        /// When the inspector loads this, draw our Shape visualiser
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // If our shape set doesnt exist, load it.

            if (_fshs == null)
            {
                _fshs = (FuzzyShapeSet)target;

                _shapes = new Dictionary<string, List<FuzzyMember>>();

                FuzzyMember[] fuzzies = _fshs.LoadShapeSet();

                foreach (FuzzyMember fuz in fuzzies)
                {
                    if (!_shapes.TryGetValue(fuz.category, out _))
                    {
                        _shapes[fuz.category] = new List<FuzzyMember>();
                    }

                    _shapes[fuz.category].Add(fuz);
                }
            }

            GUILayout.Space(10f);

            // Visualize everything

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Visualized");

                GUILayout.Space(EditorGUI.indentLevel * 8f);
            }

            foreach (KeyValuePair<string, List<FuzzyMember>> cat in _shapes)
            {
                CreateVisualiser(cat.Key, cat.Value);
            }
        }
    }
}
