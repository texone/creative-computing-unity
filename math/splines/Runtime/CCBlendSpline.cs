using System.Collections.Generic;
using cc.creativecomputing.math.util;
using UnityEngine;

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
namespace cc.creativecomputing.math.spline
{

	public class CCBlendSpline : CCSpline
	{

		private float _myBlend = 0;

		private CCSpline _mySpline1;
		private CCSpline _mySpline2;

		public CCBlendSpline(CCSpline theSpline1, CCSpline theSpline2) : base(CCSplineType.BLEND, false)
		{

			_mySpline1 = theSpline1;
			_mySpline2 = theSpline2;
		}

		public virtual void blend(float theBlend)
		{
			_myBlend = theBlend;
		}

		public override void ComputeTotalLengthImpl()
		{

		}

		public override float TotalLength()
		{
			return CCMath.Blend(_mySpline1.TotalLength(), _mySpline2.TotalLength(), _myBlend);
		}

		public override int NumberOfSegments()
		{
			return CCMath.Max(_mySpline1.NumberOfSegments(), _mySpline2.NumberOfSegments());
		}

		public override Vector3 Interpolate(float theBlend, int theControlPointIndex)
		{
			return new Vector3();
		}

		public override Vector3 Interpolate(float theBlend)
		{
			return Vector3.Lerp(_mySpline1.Interpolate(theBlend), _mySpline2.Interpolate(theBlend), _myBlend);
		}

		public virtual Vector3 Interpolate(float theBlendSpline, float theBlendPoint)
		{
			Vector3 myVector0 = _mySpline1.Interpolate(theBlendPoint);
			Vector3 myVector1 = _mySpline2.Interpolate(theBlendPoint);
			if (myVector0 == null || myVector1 == null)
			{
				return new Vector3();
			}
			return Vector3.Lerp(myVector0, myVector1, theBlendSpline);

		}
    }

}