using UnityEngine;

namespace cc.creativecomputing.effects
{
    [AddComponentMenu("effects/effect/travel")]
    public class CCTravelEffect : CCEffect
    {
        [Range(-1, 3)]
        public float travelPosition = 0;
        [Range(0, 1)]
        public float travelRange = 0;

        public bool normed = true;

        public AnimationCurve interpolation;

        public CCEffectModulation modulation = new CCEffectModulation();

        public float Value(CCEffectData theData)
        {
            if (travelRange == 0) return 0;
            float d = (modulation.Modulation(theData) - travelPosition) / travelRange;
            d = Mathf.Clamp(d, 0, 1);
            d = interpolation.Evaluate(d);
            return d;
        }

        public override float Apply(CCEffectData theData)
        {
            float d = Value(theData); ;
            //d = (Mathf.Cos(d * Mathf.PI + Mathf.PI) + 1) / 2;
            //d = (Mathf.Cos(d * Mathf.PI + Mathf.PI) + 1) / 2;
            //d = (Mathf.Cos(d * Mathf.PI + Mathf.PI) + 1) / 2;
            //d = (Mathf.Cos(d * Mathf.PI + Mathf.PI) + 1) / 2;
            if (!normed) d = d * 2 - 1;
            //d *= amount;
            //d = Mathf.Clamp(d, 0, 1) * amount;
            //  * amount;
            return d;
        }
        public override void Simulate(float theDeltaTime) { }
    }
}