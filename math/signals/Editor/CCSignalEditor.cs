using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cc.creativecomputing.math.signal
{
    [CustomEditor(typeof(CCSignal), true)]
    [CanEditMultipleObjects]
    public class CCSignalEditor : Editor
    {
        GraphDrawer _graph;
        

        void OnEnable()
        {
            _graph = new GraphDrawer();
            
        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _graph.DrawGraph((CCSignal)target);


            EditorGUILayout.Space();

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }

    // A utility class for drawing component curves.
    public class GraphDrawer
    {
        #region Public Methods

        public void DrawGraph(CCSignal signal)
        {
            _rectGraph = GUILayoutUtility.GetRect(128, 80);

            // Background
            DrawRect(0, 0, 1, 1, 0.1f, 0.4f);

            // Horizontal line
            var lineColor = Color.white * 0.4f;
            DrawLine(0, 0.5f, 1, 0.5f, lineColor);

            // Vertical lines
            DrawLine(0.25f, 0, 0.25f, 1, lineColor);
            DrawLine(0.50f, 0, 0.50f, 1, lineColor);
            DrawLine(0.75f, 0, 0.75f, 1, lineColor);

            // R/G/B curves
            DrawGradientCurve(signal, Color.red);
        }

        void DrawGradientCurve(CCSignal signal, Color color)
        {
            for (var i = 0; i < _curveResolution; i++)
            {
                var x = (float)i / (_curveResolution - 1);
                var theta = x;
                var y = signal.normed ? signal.Value(theta) : (signal.Value(theta) + 1) / 2;
                _curveVertices[i] = PointInRect(x, Mathf.Clamp01(y));
            }

            Handles.color = color;
            Handles.DrawAAPolyLine(2.0f, _curveResolution, _curveVertices);
        }

        #endregion

        #region Graph Functions

        // Number of vertices in curve
        const int _curveResolution = 96;

        // Vertex buffers
        Vector3[] _rectVertices = new Vector3[4];
        Vector3[] _lineVertices = new Vector3[2];
        Vector3[] _curveVertices = new Vector3[_curveResolution];

        Rect _rectGraph;

        // Transform a point into the graph rect.
        Vector3 PointInRect(float x, float y)
        {
            x = Mathf.Lerp(_rectGraph.x, _rectGraph.xMax, x);
            y = Mathf.Lerp(_rectGraph.yMax, _rectGraph.y, y);
            return new Vector3(x, y, 0);
        }

        // Draw a line in the graph rect.
        void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            _lineVertices[0] = PointInRect(x1, y1);
            _lineVertices[1] = PointInRect(x2, y2);
            Handles.color = color;
            Handles.DrawAAPolyLine(2.0f, _lineVertices);
        }

        // Draw a rect in the graph rect.
        void DrawRect(float x1, float y1, float x2, float y2, float fill, float line)
        {
            _rectVertices[0] = PointInRect(x1, y1);
            _rectVertices[1] = PointInRect(x2, y1);
            _rectVertices[2] = PointInRect(x2, y2);
            _rectVertices[3] = PointInRect(x1, y2);

            Handles.DrawSolidRectangleWithOutline(
                _rectVertices,
                fill < 0 ? Color.clear : Color.white * fill,
                line < 0 ? Color.clear : Color.white * line
            );
        }

        #endregion
    }
}