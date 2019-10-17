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
	public class CCSmoothSawSignal : CCSignal
	{
		
        [Range(0, 1)]
        public float delta = 0.01f;

        private float trg(float theInput)
        {
            return 1 - 2 * Mathf.Acos((1 - delta) * Mathf.Sin(2 * Mathf.PI  * theInput)) / Mathf.PI;
        }

		private float SawValue(float theInput)
		{
            float myResult = (trg((2 * theInput - 1)/ 4) * Mathf.Pow(theInput / 2,2))/4 + 0.5f; //1 - Mathf.Acos((1 - delta) * Mathf.Sin(2 * Mathf.PI  * theInput)) / Mathf.PI;

            if (!normed)
			{
				myResult = myResult * 2 - 1;
			}
			return myResult;
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(SawValue(theX) + SawValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(SawValue(theX) + SawValue(theY)) / 2};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{ SawValue(theX)};
		}

	}

}