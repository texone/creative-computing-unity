/*
 * Copyright (c) 2013 christianr.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the GNU Lesser Public License v3
 * which accompanies this distribution, and is available at
 * http://www.gnu.org/licenses/lgpl-3.0.html
 * 
 * Contributors:
 *     christianr - initial API and implementation
 */

using System;

namespace cc.creativecomputing.math.signal
{
	using UnityEngine;

	/// <summary>
	/// @author christianriekoff
	/// 
	/// </summary>
	
	[Serializable]
	public class CCSquareSignal : CCSignal
	{
		[Range(0,1)]
		public float pulseWidth = 0.5f;

		private float square(float theValue)
		{
			if (theValue < 0)
			{
				theValue = -theValue + 0.5f;
			}
			if (theValue % 1 < pulseWidth)
			{
				return 1;
			}
			else
			{
				return normed ? 0 : -1;
			}
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#noiseImpl(float, float, float)
		 */
		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(square(theX) + square(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(square(theX) + square(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{square(theX)};
		}

	}

}