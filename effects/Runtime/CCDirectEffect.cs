using UnityEngine;

namespace cc.creativecomputing.effects
{

    [AddComponentMenu("effects/effect/direct")]
    public class CCDirectEffect : CCEffect
    {

        public CCEffectModulation modulation = new CCEffectModulation();


        public override float Apply(CCEffectData theData)
        {

            return modulation.Modulation(theData);
        }

        public override void Simulate(float theDeltaTime) { }
    }
}