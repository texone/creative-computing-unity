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
	public class CCSlopedTriSignal : CCSignal
	{


		private float triValue(float theInput)
		{
			theInput += 0.5f;
			if (theInput < 0)
			{
				theInput = -theInput + 0.25f;
			}
			theInput = (theInput * 4) % 4;
			float myResult = 0;
			if (theInput < 1)
			{
				myResult = 0;
			}
			else if (theInput < 2)
			{
				myResult = (theInput - 1);
			}
			else if (theInput < 3)
			{
				myResult = 1;
			}
			else
			{
				myResult = 1 - (theInput - 3);
			}

			if (!normed)
			{
				myResult = myResult * 2 - 1;
			}
			return myResult;
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(triValue(theX) + triValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(triValue(theX) + triValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{triValue(theX)};
		}

	}

}