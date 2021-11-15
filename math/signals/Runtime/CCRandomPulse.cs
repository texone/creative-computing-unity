using UnityEngine;
using System;
using cc.creativecomputing.math.util;

namespace cc.creativecomputing.math.signal
{
	
	[Serializable]
	public class CCRandomPulse : CCSimplexNoise
	{

		[Range(0, 1)]
		public float _cRatio = 0.5f;

		private float[] Step(float[] theValues) {
			var myResult = new float[theValues.Length];
		
			for(var i = 0; i < theValues.Length;i++) {
				myResult[i] = CCMath.Step(_cRatio, theValues[i]);
			}
		
			return myResult;
		}
		
		public override float[] SignalImpl(float theX)
		{
			return Step(base.SignalImpl(theX));
		}

		public override float[] SignalImpl(float theX, float theY)
		{
			return Step(base.SignalImpl(theX, theY));
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return Step(base.SignalImpl(theX, theY, theZ));
		}
	}

}