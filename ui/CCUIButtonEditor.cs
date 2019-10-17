using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

namespace cc.creativecomputing.ui
{
    /// <summary>
    /// Custom inspector for Object including derived classes.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class CCUIButtonEditor : Editor
    {
        public void DrawEasyButtons()
        {
            // Loop through all methods with no parameters
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetParameters().Length == 0);

            foreach (var method in methods)
            {
                // Get the ButtonAttribute on the method (if any)
                var ba = (CCUIButtonAttribute)Attribute.GetCustomAttribute(method, typeof(CCUIButtonAttribute));

                if (ba != null)
                {
                    // Determine whether the button should be enabled based on its mode
                    var wasEnabled = GUI.enabled;
                    GUI.enabled = ba.Mode == CCUIButtonMode.AlwaysEnabled
                        || (EditorApplication.isPlaying ? ba.Mode == CCUIButtonMode.EnabledInPlayMode : ba.Mode == CCUIButtonMode.DisabledInPlayMode);


                    if (((int)ba.Spacing & (int)CCUIButtonSpacing.Before) != 0) GUILayout.Space(10);

                    // Draw a button which invokes the method
                    var buttonName = String.IsNullOrEmpty(ba.Name) ? ObjectNames.NicifyVariableName(method.Name) : ba.Name;
                    if (GUILayout.Button(buttonName))
                    {
                        foreach (var t in targets)
                        {
                            method.Invoke(t, null);
                        }
                    }

                    if (((int)ba.Spacing & (int)CCUIButtonSpacing.After) != 0) GUILayout.Space(10);

                    GUI.enabled = wasEnabled;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            this.DrawEasyButtons();

            // Draw the rest of the inspector as usual
            DrawDefaultInspector();
        }
    }
}