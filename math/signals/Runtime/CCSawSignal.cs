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

	/// <summary>
	/// @author christianriekoff
	/// 
	/// </summary>
	[Serializable]
	public class CCSawSignal : CCSignal
	{

		private float saw(float theValue)
		{
			float myResult = 0;
			if (theValue < 0)
			{
				myResult = -theValue % 1;
			}
			else
			{
				myResult = 1 - theValue % 1;
			}
			if (!normed)
			{
				myResult = myResult * 2 - 1;
			}
			return myResult;
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(saw(theX) + saw(theY) + saw(theZ) / 3f)};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(saw(theX) +  saw(theY) / 2)};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{saw(theX)};
		}

	}

}