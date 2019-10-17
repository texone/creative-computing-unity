using UnityEngine;
using System.Collections.Generic;
using cc.creativecomputing.math.util;

namespace cc.creativecomputing.util
{
    [System.Serializable]
    public struct CCDataHistory 
    {

        public float max;

        public List<List<float>> data;

        public void Add(float theValue)
        {
            Add(0, theValue);
        }

        public void Add(int theStream, float theValue)
        {
            if (data == null) return;
            while (data.Count <= theStream)
            {
                data.Add(new List<float>());
            }
            data[theStream].Add(theValue);
        }

        
    }
}
