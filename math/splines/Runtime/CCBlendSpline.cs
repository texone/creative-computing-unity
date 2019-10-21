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
		private readonly CCSpline _mySpline1;
		private readonly CCSpline _mySpline2;

		public CCBlendSpline(CCSpline theSpline1, CCSpline theSpline2) : base(CCSplineType.BLEND, false)
		{

			_mySpline1 = theSpline1;
			_mySpline2 = theSpline2;
		}

		public float Blend { get; set; } = 0;

		protected override void ComputeTotalLengthImpl()
		{

		}

		public override float TotalLength => CCMath.Blend(_mySpline1.TotalLength, _mySpline2.TotalLength, Blend);


		public override int NumberOfSegments => CCMath.Max(_mySpline1.NumberOfSegments, _mySpline2.NumberOfSegments);
		

		protected override Vector3 Interpolate(float theBlend, int theControlPointIndex)
		{
			return new Vector3();
		}

		public override Vector3 Interpolate(float theBlend)
		{
			return Vector3.Lerp(_mySpline1.Interpolate(theBlend), _mySpline2.Interpolate(theBlend), Blend);
		}

		public virtual Vector3 Interpolate(float theBlendSpline, float theBlendPoint)
		{
			var myVector0 = _mySpline1.Interpolate(theBlendPoint);
			var myVector1 = _mySpline2.Interpolate(theBlendPoint);
			return Vector3.Lerp(myVector0, myVector1, theBlendSpline);

		}

		public override void Draw()
		{
		}
	}

}