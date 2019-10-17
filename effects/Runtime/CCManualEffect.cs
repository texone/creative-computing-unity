using UnityEngine;

using System.Collections.Generic;
namespace cc.creativecomputing.effects
{
    // [ExecuteAlways]
    [AddComponentMenu("effects/effect/manual")]
    public class CCManualEffect : CCEffect
    {
        [Range(-1,1)]
        public List<float> amounts = new List<float>();

        private void Start()
        {
            amounts.Clear();
            for (int i = 0; i < 20;i++)
            {
                amounts.Add(0);
            }
        }

        public override float Apply(CCEffectData theObject)
        {
            if (theObject.id >= amounts.Count) return 0;
            return amounts[theObject.id]; 
        }

        public override void Simulate(float theDeltaTime) { } 
    }
}