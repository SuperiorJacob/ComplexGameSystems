using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using FuzzyStateMachine.Variable;

namespace FuzzyStateMachine
{
    [CustomEditor(typeof(FuzzyShapeSet))]
    [CanEditMultipleObjects]
    public class StateMachineShapeVisualiserEditor : Editor
    {
        private FuzzyShapeSet fshs;

        private Vector3[] verticesBuffer;
        float minX = 0,
            minY = -1,
            maxX = 10,
            maxY = 1;

        Rect rect;
        float rangeX = 10;
        float rangeY = 10;

        Dictionary<string, List<FuzzyMember>> shapes;

        Vector3 UnitToGraph(float x, float y)
        {
            x = Mathf.Lerp(rect.x, rect.xMax, (x - minX) / rangeX);
            y = Mathf.Lerp(rect.yMax, rect.y, (y - minY) / rangeY);

            return new Vector3(x, y, 0);
        }

        void DrawLine(Color color, float width, params Vector2[] points)
        {
            verticesBuffer = new Vector3[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                verticesBuffer[i] = UnitToGraph(points[i].x, points[i].y);
            }

            Handles.color = color;
            Handles.DrawAAPolyLine(width, points.Length, verticesBuffer);
        }

        void DrawRect(float x1, float y1, float x2, float y2, Color fill, Color line)
        {
            verticesBuffer = new Vector3[4];

            verticesBuffer[0] = UnitToGraph(x1, y1);
            verticesBuffer[1] = UnitToGraph(x2, y1);
            verticesBuffer[2] = UnitToGraph(x2, y2);
            verticesBuffer[3] = UnitToGraph(x1, y2);

            Handles.DrawSolidRectangleWithOutline(verticesBuffer, fill, line);
        }

        void Draw(List<FuzzyMember> members)
        {
            DrawRect(minX, minY, maxX, maxY, Color.black, Color.black);

            foreach (var fuz in members)
            {
                DrawLine(fuz.color, 2, fuz.Visualise());
            }
        }

        public void CreateVisualiser(string name, List<FuzzyMember> todraw)
        {
            GUI.backgroundColor = Color.gray;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label(name);
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 15f);
                rect = GUILayoutUtility.GetRect(128, 80);
            }

            rangeX = maxX - minX;
            rangeY = maxY - minY;

            Draw(todraw);

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                foreach (var fuz in todraw)
                {
                    Color prev = GUI.color;
                    GUI.color = fuz.color;
                    GUILayout.Label(fuz.name);
                    GUI.color = prev;
                }

                GUILayout.Space(EditorGUI.indentLevel * 8f);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (fshs == null)
            {
                fshs = (FuzzyShapeSet)target;

                shapes = new Dictionary<string, List<FuzzyMember>>();

                FuzzyMember[] fuzzies = fshs.LoadShapeSet();

                foreach (FuzzyMember fuz in fuzzies)
                {
                    if (!shapes.TryGetValue(fuz.category, out _))
                    {
                        shapes[fuz.category] = new List<FuzzyMember>();
                    }

                    shapes[fuz.category].Add(fuz);
                }
            }

            GUILayout.Space(10f);

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Visualized");

                GUILayout.Space(EditorGUI.indentLevel * 8f);
            }

            foreach (var cat in shapes)
            {
                CreateVisualiser(cat.Key, cat.Value);
            }
        }
    }
}