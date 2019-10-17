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

using UnityEngine;

using cc.creativecomputing.math.util;
namespace cc.creativecomputing.math.filter
{

    /// <summary>
    /// @author christianriekoff
    /// 
    /// </summary>
    [AddComponentMenu("Filter/Exponential Smoothing")]
    public class CCExponentialSmoothingFilter : CCFilter
	{

		[Range(0, 1)]
		public float weight;

        [Range(0, 50)]
        public float skipRange = 0;

		

		private float[] _myValues;


		public override float Process(int theChannel, float theData, float theTime)
		{
			if (_myValues == null || theChannel >= _myChannels || _myValues.Length < _myChannels)
			{
				_myChannels = theChannel + 1;
				_myValues = new float[_myChannels];
			}
			if (_myValues[theChannel] == 0)
			{
				_myValues[theChannel] = theData;
				return _myValues[theChannel];
			}
			if (CCMath.Abs(_myValues[theChannel] - theData) > skipRange && skipRange > 0)
			{
				_myValues[theChannel] = theData;
				return _myValues[theChannel];
			}

            _myValues[theChannel] = CCMath.Blend(theData, _myValues[theChannel], weight);
            if (bypass) return theData;
			return _myValues[theChannel];
		}
	}

}