using System;
using UnityEditor;
using UnityEngine;

namespace cc.creativecomputing.math.spline
{
    [UnityEditor.CustomEditor(typeof(CCSpline), true)]
    public class CCSplineEditor : UnityEditor.Editor
    {
        [MenuItem( "GameObject/Spline/Bezier", priority = 35 )]
        private static void NewBezierSpline()
        {
            var spline = new GameObject( "BezierSpline").AddComponent<CCBezierSpline>();
            spline.BeginEditSpline();
            spline.AddPoint(new Vector3(0,0,0));
            spline.AddPoint(new Vector3(2,0,0));
            spline.EndEditSpline();
            Undo.RegisterCreatedObjectUndo( spline, "Create Bezier Spline" );
            Selection.activeTransform = spline.transform;
        }
        [MenuItem( "GameObject/Spline/Linear", priority = 35 )]
        private static void NewLinearSpline()
        {
            var spline = new GameObject( "LinearSpline").AddComponent<CCLinearSpline>();
            
            spline.AddPoint(new Vector3(0,0,0));
            spline.AddPoint(new Vector3(2,0,0));
            
            Undo.RegisterCreatedObjectUndo( spline, "Create Linear Spline" );
            Selection.activeTransform = spline.transform;
        }
        
        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            var spline = target as CCSpline;
            if (spline == null) return;
            switch (spline.Type)
            {
                case CCSpline.CCSplineType.LINEAR:
                    var linearSpline = target as CCLinearSpline;
                    if (GUILayout.Button("Add Point")) {
                        Undo.RecordObject(spline, "Add Point");
                        var lastPoint = linearSpline.LastPoint;
                        spline.AddPoint(new Vector3(lastPoint.x + 1,lastPoint.y,lastPoint.z));
                        EditorUtility.SetDirty(spline);
                    }
                    break;
                case CCSpline.CCSplineType.BEZIER:
                    var bezierSpline = target as CCBezierSpline;
                    if (GUILayout.Button("Add Point")) {
                        Undo.RecordObject(spline, "Add Point");
                        var lastPoint = bezierSpline.LastPoint;
                        spline.AddPoint(new Vector3(lastPoint.x + 1,lastPoint.y,lastPoint.z));
                        spline.EndEditSpline();
                        EditorUtility.SetDirty(spline);
                    }
                    break;
            }
        }
      

        private CCSpline _spline;
        private Transform _handleTransform;
        private Quaternion _handleRotation;

        private const float HandleSize = 0.04f;
        private const float PickSize = 0.06f;
	
        private int _selectedIndex = -1;

        private bool _mirror = false;
        private bool _align = false;
        
        private void CheckHandles (int theIndexSet, int theCenterIndex, int theIndexEnforced, Vector3 theNewPosition)
        {
            if (!_mirror && !_align) return;
            
            
            Vector3 center = _spline[theCenterIndex];
            Vector3 enforcedTangent = center - _spline[theIndexSet];
            if (_align) {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(center, _spline[theIndexEnforced]);
            }
            _spline[theIndexSet] = theNewPosition;
            _spline[theIndexEnforced] = center + enforcedTangent;
            
        }
        
        private void ShowPoint(int theIndex)
        {
            var point = _handleTransform.TransformPoint(_spline[theIndex]);
            var size = HandleUtility.GetHandleSize(point);
            
            Handles.color = Color.white;
            if (Handles.Button(point, _handleRotation, HandleSize * size, PickSize * size, Handles.DotHandleCap))
            {
                _selectedIndex = theIndex;
            }

            if (_selectedIndex != theIndex) return;
            
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _handleRotation);
            
            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject(_spline, "Move Point");
            EditorUtility.SetDirty(_spline);
            
            var newPosition = _handleTransform.InverseTransformPoint(point);
            
            switch (_spline.Type)
            {
                case CCSpline.CCSplineType.LINEAR:
                    _spline[theIndex] = newPosition;
                    break;
                case CCSpline.CCSplineType.BEZIER:
                    switch (theIndex % 3)
                    {
                        case 0:
                            var delta = newPosition - _spline[theIndex];
                            if (theIndex > 0) {
                                _spline[theIndex - 1] += delta;
                            }
                            if (theIndex + 1 < _spline.Count) {
                                _spline[theIndex + 1] += delta;
                            }
                            _spline[theIndex] = newPosition;
                            break;
                        case 1:
                            _spline[theIndex] = newPosition;
                            if (theIndex == 1)
                            {
                                break;
                            }

                            CheckHandles(theIndex, theIndex - 1, theIndex - 2, newPosition);
                            break;
                        case 2:
                            _spline[theIndex] = newPosition;
                            if (theIndex == _spline.Count - 2)
                            {
                                break;
                            }
                    
                            CheckHandles(theIndex, theIndex + 1, theIndex + 2, newPosition);
                            break;
                    }
                    break;
            }
            
            _spline.ComputeTotalLength();
        }

        private void HandleKeyEvents()
        {
            if (Event.current == null) return;
            if (!Event.current.isKey) return;


            switch (Event.current.keyCode)
            {
                case KeyCode.M:
                    _mirror = Event.current.type == EventType.KeyDown;
                    break;
                case KeyCode.A:
                    _align = Event.current.type == EventType.KeyDown;
                    break;
            }
        }

        private void OnSceneGUI ()
        {
            HandleKeyEvents();
            
            _spline = target as CCSpline;
            if (_spline == null) return;
            _spline.Draw();
            _handleTransform = _spline.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;
            switch (_spline.Type)
            {
                case  CCSpline.CCSplineType.LINEAR:
                    break;
                case CCSpline.CCSplineType.BEZIER:
                    for (var i = 0; i < _spline.Count - 3;i+=3) {
                        var startAnchor = _handleTransform.TransformPoint(_spline[i]);
                        var startControl = _handleTransform.TransformPoint(_spline[i + 1]);
                        var endControl = _handleTransform.TransformPoint(_spline[i + 2]);
                        var endAnchor = _handleTransform.TransformPoint(_spline[i + 3]);
                        
                        Handles.DrawLine(startAnchor, startControl);
                        Handles.DrawLine(endAnchor, endControl);
                    }
                    break;
            }

          
            for (var i = 0; i < _spline.Count; i++) 
            {
                ShowPoint(i);
            }
                          
            Handles.color = Color.white;
            
        }
    }
}