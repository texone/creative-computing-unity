using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace cc.creativecomputing.ui
{
    public enum CCUIButtonMode
    {
        AlwaysEnabled,
        EnabledInPlayMode,
        DisabledInPlayMode
    }

    [Flags]
    public enum CCUIButtonSpacing
    {
        None = 0,
        Before = 1,
        After = 2
    }

    /// <summary>
    /// Attribute to create a button in the inspector for calling the method it is attached to.
    /// The method must have no arguments.
    /// </summary>
    /// <example>
    /// [CCUIButton]
    /// public void MyMethod()
    /// {
    ///     Debug.Log("Clicked!");
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class CCUIButtonAttribute : Attribute
    {
        private string name = null;
        private CCUIButtonMode mode = CCUIButtonMode.AlwaysEnabled;
        private CCUIButtonSpacing spacing = CCUIButtonSpacing.None;

        public string Name { get { return name; } }
        public CCUIButtonMode Mode { get { return mode; } }
        public CCUIButtonSpacing Spacing { get { return spacing; } }

        public CCUIButtonAttribute()
        {
        }

        public CCUIButtonAttribute(string theName)
        {
            name = theName;
        }

        public CCUIButtonAttribute(CCUIButtonMode theMode)
        {
            mode = theMode;
        }

        public CCUIButtonAttribute(CCUIButtonSpacing theSpacing)
        {
            spacing = theSpacing;
        }

        public CCUIButtonAttribute(string theName, CCUIButtonMode theMode)
        {
            name = theName;
            mode = theMode;
        }

        public CCUIButtonAttribute(string theName, CCUIButtonSpacing theSpacing)
        {
            name = theName;
            spacing = theSpacing;
        }

        public CCUIButtonAttribute(string theName, CCUIButtonMode theMode, CCUIButtonSpacing theSpacing)
        {
            name = theName;
            mode = theMode;
            spacing = theSpacing;
        }
    }
}
