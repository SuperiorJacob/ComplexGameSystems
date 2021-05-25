using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace FuzzyStateMachine
{
    [RequireComponent(typeof(StateMachineDebugger))]
    public class StateMachineVisualiser : MonoBehaviour
    {
    }

    [CustomEditor(typeof(StateMachineVisualiser))]
    [CanEditMultipleObjects] // Allows us to have multiple shapes open at once!
    public class StateMachineVisualiserEditor : Editor
    {
        #region Fields

        private StateMachineDebugger _smd;

        // Vertex buffers
        private Vector3[] _verticesBuffer;

        private bool _visualised = false;

        private Vector2 _scrollPos;
        private FuzzyRuleSet _ruleSet;
        private FuzzyLogic _logic;

        private float _minX = 0,
            _minY = -1,
            _maxX = 10, 
            _maxY = 1;

        private Rect _rect;
        private float _rangeX = 10;
        private float _rangeY = 10;

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
        /// Drawing a category based visualisation
        /// </summary>
        /// <param name="a_cat">Category name</param>
        /// <param name="a_todraw">Category shapes</param>
        private void Draw(string a_cat, Dictionary<int, Variable.FuzzyMember> a_todraw)
        {
            DrawRect(_minX, _minY, _maxX, _maxY, Color.black, Color.black);

            foreach (KeyValuePair<int, Variable.FuzzyMember> fuz in a_todraw)
            {
                DrawLine(fuz.Value.color, 2, fuz.Value.Visualise());
            }

            float x = _logic.lastDefuzz / 10;
            if (a_cat != "desire")
                x = _ruleSet.inputs[a_cat] / 10;
            
            DrawLine(Color.white, 2, new Vector2(x, 1), new Vector2(x, -1));

        }

        /// <summary>
        /// Toggles our visualiser
        /// </summary>
        public void Visualize()
        {
            _visualised = !_visualised;
        }

        /// <summary>
        /// Create the shape visualiser
        /// </summary>
        /// <param name="a_name">Name of the category</param>
        /// <param name="a_todraw">Shape to draw</param>
        public void CreateVisualiser(string a_name, Dictionary<int, Variable.FuzzyMember> a_todraw)
        {
            // Visualiser title box

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

            // Draw the graph

            Draw(a_name, a_todraw);

            // Graph legend / exit bar

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                foreach (var fuz in a_todraw)
                {
                    Color prev = GUI.color;
                    GUI.color = fuz.Value.color;
                    GUILayout.Label(fuz.Value.name);
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

            using (new GUILayout.HorizontalScope())
            {
                // Tests the loader code
                if (GUILayout.Button("Perform"))
                {
                    MonoBehaviour monoBev = (MonoBehaviour)target; 
                    _smd = monoBev.GetComponent<StateMachineDebugger>();

                    _smd.Perform();
                }

                // Visualises the graphs after execution
                if (GUILayout.Button("Visualize"))
                {
                    if (_visualised)
                    {
                        Visualize();
                    }
                    else
                    {
                        MonoBehaviour monoBev = (MonoBehaviour)target;
                        _smd = monoBev.GetComponent<StateMachineDebugger>();

                        Debug.Log(_smd.transform.name);

                        if (_smd.logic == null || _smd.logic.Length == 0)
                        {
                            _smd.Perform();
                        }

                        Visualize();
                    }
                }
            }

            if (_visualised)
            {
                // Draw output logs
                GUILayout.Label($"Command Output:");

                using (EditorGUILayout.HorizontalScope h = new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos, GUILayout.Width(h.rect.width), GUILayout.Height(100)))
                    {
                        _scrollPos = scrollView.scrollPosition;

                        for (int i = 0; i < _smd.debug.Count; i++)
                        {
                            string label = _smd.debug[i];
                            bool error = label.Substring(0, label.IndexOf(" ")) == "Error:";

                            if (error)
                            {
                                GUI.backgroundColor = new Color(1, 0.1f, 0.1f, 1);
                            }
                            else
                            {
                                float w = label[0] == ' ' ? 0 : 1f;
                                GUI.backgroundColor = new Color(w, w, w, (i % 2 == 0 ? 0.8f : 1f));
                            }

                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                GUILayout.Label(label);
                            }
                        }
                    }
                }

                GUILayout.Label($"Visualised Rules:");

                // Visualised graph

                string lrn = _smd.mainData.ruleSet != null ? _smd.mainData.ruleSet.GetType().Name : "";
                foreach (FuzzyLogic log in _smd.logic)
                {
                    FuzzyRuleSet logicRuleSet = log.rule;
                    string lrsn = logicRuleSet.GetType().Name;

                    if (lrn == lrsn) GUI.backgroundColor = Color.green;
                    else GUI.backgroundColor = Color.gray;

                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Label(lrsn);

                        GUILayout.Space(EditorGUI.indentLevel * 8f);
                    }

                    _ruleSet = logicRuleSet;
                    _logic = log;
                    foreach (KeyValuePair<string, Dictionary<int, Variable.FuzzyMember>> cat in logicRuleSet.GetMembersByCategory())
                    {
                        CreateVisualiser(cat.Key, cat.Value);
                    }

                    using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                    { }
                }

            }
        }
    }
}
