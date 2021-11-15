using UnityEngine;
using System.Collections;
using cc.creativecomputing.math.signal;

namespace cc.creativecomputing.effects
{
    [AddComponentMenu("effects/effect/signal")]
    public class CCSignalEffect : CCEffect
    {

        public CCEffectModulation phase = new CCEffectModulation();
        public CCEffectModulation amp = new CCEffectModulation();

        public CCSignal signal;

        public Vector3 speed = new Vector3();

        public bool useSpeed = false;

        public Vector3 offset = new Vector3();

        public override void Simulate(float theDeltaTime)
        {
            if (useSpeed) offset += theDeltaTime * speed; 
        }

        private float[] Values(CCEffectData theData)
        {
            
            return signal.Values(
                phase.X(theData) + offset.x,
                phase.Y(theData) + offset.y,
                phase.Z(theData) + offset.z + phase.ModulationWithoutPosition(theData)
            );
        }

        public override float Apply(CCEffectData theData)
        {
            if (!signal) return 0;
            return Values(theData)[0] * amp.Modulation(theData);


        }
    }
}
