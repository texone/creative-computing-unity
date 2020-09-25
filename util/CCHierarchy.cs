using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.util
{
    public class CCHierarchy {
        
        public delegate void Apply(GameObject theObject);

        public static void ApplyToChildren(GameObject theObject, Apply theFunction)
        {
            if (!theObject) return;

            foreach (Transform child in theObject.transform)
            {
                theFunction(child.gameObject); 
            }
        }

        public static void RemoveChildrenPlayMode(GameObject theObject, params string[] theNames)
        {
            foreach (var name in theNames)
            {
                Object.Destroy(theObject.transform.Find(name).gameObject);
            }
        }
        
        public static void RemoveChildrenPlayMode(GameObject theObject)
        {
            for (var i = theObject.transform.childCount - 1; i >= 0; i--) {
                Object.Destroy( theObject.transform.GetChild( i ).gameObject );
            }
        }
        
        public static void RemoveChildrenEditor(GameObject theObject, params string[] theNames)
        {
            foreach (var name in theNames)
            {
                Object.DestroyImmediate(theObject.transform.Find(name).gameObject);
            }
        }
        
        public static void RemoveChildrenEditor(GameObject theObject)
        {
            for (var i = theObject.transform.childCount - 1; i >= 0; i--) {
                Object.DestroyImmediate( theObject.transform.GetChild( i ).gameObject );
            }
        }
    }
}

