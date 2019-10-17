using System;

namespace cc.creativecomputing.math.signal
{
	
	[Serializable]
	public class CCSwitchSignal : CCSignal
	{

		public CCSignalType signal = CCSignalType.SIMPLEX;

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return signal.signal().SignalImpl(theX, theY, theZ);
		}

		public override float[] SignalImpl(float theX, float theY)
		{
			return signal.signal().SignalImpl(theX, theY);
		}

		public override float[] SignalImpl(float theX)
		{
			return signal.signal().SignalImpl(theX);
		}

	}

}