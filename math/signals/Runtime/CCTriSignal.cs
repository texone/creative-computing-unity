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
namespace cc.creativecomputing.math.signal
{
	using UnityEngine;
	using System;

	/// <summary>
	/// @author christianriekoff
	/// 
	/// </summary>
	[Serializable]
	public class CCTriSignal : CCSignal
	{
		[Range(0, 1)]
		public float ratio = 0.5f;

		public CCTriSignal() : base()
		{
		}

		private float TriValue(float theInput)
		{
			theInput += 0.25f;
			theInput %= 1;
			if (theInput < 0)
			{
				theInput = 1 + theInput;
			}

			float myResult = theInput / ratio;
			if (theInput > ratio)
			{
				myResult = 1 - (theInput - ratio) / (1 - ratio);
			}

			if (!normed)
			{
				myResult = myResult * 2 - 1;
			}
			return myResult;
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(TriValue(theX) + TriValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(TriValue(theX) + TriValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{TriValue(theX)};
		}

	}

}