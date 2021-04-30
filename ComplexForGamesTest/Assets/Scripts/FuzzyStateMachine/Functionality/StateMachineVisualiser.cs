using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyStateMachine
{
    [CustomEditor(typeof(StateMachineDebugger))]
    public class StateMachineVisualiser : Editor
    {
        StateMachineDebugger smd;

        // Vertex buffers
        private Vector3[] verticesBuffer;

        float minX = 0,
            minY = -1,
            maxX = 10, 
            maxY = 1;

        Rect rect;
        float rangeX = 10;
        float rangeY = 10;

        Vector3 UnitToGraph(float x, float y)
        {
            x = Mathf.Lerp(rect.x, rect.xMax, (x - minX) / rangeX);
            y = Mathf.Lerp(rect.yMax, rect.y, (y - minY) / rangeY);

            return new Vector3(x, y, 0);
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

        Color changeAlpha(Color col, float alpha)
        {
            return new Color(col.r, col.g, col.b, alpha);
        }

        void Draw(string cat, Dictionary<int, Variable.FuzzyMember> todraw)
        {
            DrawRect(minX, minY, maxX, maxY, Color.black, Color.black);

            foreach (var fuz in todraw)
            {
                DrawLine(fuz.Value.color, 2, fuz.Value.Visualise());

                //float f = (fuz.Value.lastCheck * fuz.Value.max)/ 10;

                //DrawLine(changeAlpha(fuz.Value.color, 0.5f), 2, new Vector2(f, 1), new Vector2(f, -1));

                //float d = fuz.Value.shape[0] > smd.desirability.z ? -smd.desirability.z : smd.desirability.x;
                //float o = fuz.Value.shape[0] / 10;

                //DrawRect(smd.deffuziedOutput/10 - 0.1f, fuz.Value.lastCheck - 0.1f, smd.deffuziedOutput/10 + 0.1f, fuz.Value.lastCheck + 0.1f, fuz.Value.color, Color.black);
            }

            float x = smd.deffuziedOutput / 10;
            if (cat != "desire")
            {
                x = smd.logic.rule.inputs[cat] / 10;
            }
            
            DrawLine(Color.white, 2, new Vector2(x, 1), new Vector2(x, -1));
            //DrawLine(Color.gray, 2, new Vector2(smd.input / 10, 1), new Vector2(smd.input / 10, -1));

        }

        public void CreateVisualiser(string name, Dictionary<int, Variable.FuzzyMember> todraw)
        {
            MonoBehaviour monoBev = (MonoBehaviour)target;
            smd = monoBev.GetComponent<StateMachineDebugger>();

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

            Draw(name, todraw);

            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                foreach (var fuz in todraw)
                {
                    Color prev = GUI.color;
                    GUI.color = fuz.Value.color;
                    GUILayout.Label(fuz.Value.name);
                    GUI.color = prev;
                }

                GUILayout.Space(EditorGUI.indentLevel * 8f);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foreach (var cat in smd.logic.rule.GetMembersByCategory())
            {
                CreateVisualiser(cat.Key, cat.Value);
            }
        }
    }
}