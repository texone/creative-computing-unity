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
using UnityEngine;

namespace cc.creativecomputing.math.signal
{

	/// <summary>
	/// @author christianriekoff
	/// 
	/// </summary>
	[Serializable]
	public class CCSlopedSinSignal : CCSignal
	{

		

        [Range(0, 1)]
        public float slope = 0;

        private float Sin(float theValue)
        {
            theValue %= 1;
            float myResult = 0;
            float mySinTime = 1 - slope;
            if (theValue < slope / 2)
            {
                myResult = 1;
            }
            else if (theValue < slope / 2 + mySinTime / 2)
            {
                myResult = Mathf.Cos((theValue - slope / 2 )/ mySinTime * 2 * Mathf.PI);
            }
            else if (theValue < slope + mySinTime / 2)
            {
                myResult = -1;
            }
            else
            {
                myResult = Mathf.Cos((theValue - slope) / mySinTime * 2 * Mathf.PI);
            }

            if (normed)
            {
                myResult = (myResult + 1) / 2;
            }
            return myResult;
        }

        /* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#noiseImpl(float, float, float)
		 */
        public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{(Sin(theX) * Sin(theY) * Sin(theZ))};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{(Sin(theX) * Sin(theY))};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			return new float[]{ Sin(theX)};
		}

	}

}