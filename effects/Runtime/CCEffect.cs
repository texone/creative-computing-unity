using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace cc.creativecomputing.effects
{
    public abstract class CCEffect : MonoBehaviour
    {

        [Range(-1, 1)]
        public float amount = 0;

        [HideInInspector]
        public CCEffects effects;

        //[HideInInspector]
        public List<float> dataAmounts = new List<float>();

        public abstract float Apply(CCEffectData theObject);

        public abstract void Simulate(float theDeltaTime);
    }
}
