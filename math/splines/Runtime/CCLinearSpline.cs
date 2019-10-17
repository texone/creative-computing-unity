using System.Collections.Generic;
using UnityEngine;
using cc.creativecomputing.math.util;

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


	public class CCLinearSpline : CCSpline
	{

		private IList<CCLine3> _myLines = new List<CCLine3>();

		public CCLinearSpline(bool theIsClosed) : base(CCSplineType.LINEAR, theIsClosed)
		{
		}

		public CCLinearSpline(Vector3[] theControlPoints, bool theIsClosed) : base(CCSplineType.LINEAR, theControlPoints, theIsClosed)
		{
		}

		public CCLinearSpline(IList<Vector3> theControlPoints, bool theIsClosed) : base(CCSplineType.LINEAR, theControlPoints, theIsClosed)
		{
		}

		public override void ComputeTotalLengthImpl()
		{
			_myLines.Clear();
			if (_myPoints.Count > 1)
			{
				for (int i = 0; i < _myPoints.Count - 1; i++)
				{
					CCLine3 myLine = new CCLine3(_myPoints[i], _myPoints[i + 1]);
					float myLength = myLine.Length();
					_mySegmentsLength.Add(myLength);
					_myTotalLength += myLength;
					_myLines.Add(myLine);
				}
			}
		}

		public override Vector3 Interpolate(float value, int currentControlPoint)
		{
			EndEditSpline();
			return Vector3.Lerp(_myPoints[currentControlPoint], _myPoints[currentControlPoint + 1], value);
		}



	//	public Tuple<Integer, Float>closestInterpolation(Vector3 thePoint,  int theStart, int theEnd){
	//		if(theEnd < theStart)theEnd += _myLines.size();
	//		
	//		int myIndex = theStart;
	//		float myBlend = 0;
	//		
	//		float myMinDistSq = Float.MAX_VALUE;
	//		
	//		for(int i = theStart; i < theEnd;i++){
	//			CCLine3 myLine = _myLines.get(i % _myLines.size());
	//			Vector3 myPoint = myLine.closestPoint(thePoint);
	//			float myDistSq = myPoint.distanceSquared(thePoint);
	//			if(myDistSq < myMinDistSq){
	//				myIndex = i % _myLines.size();
	//				myBlend = myLine.closestPointBlend(thePoint);
	//				myMinDistSq = myDistSq;
	//			}
	//		}
	//		
	//		return new CCTuple<Integer, Float>(myIndex, myBlend);
	//	}

		public override Vector3 ClosestPoint(Vector3 thePoint)
		{
			return closestPoint(thePoint, 0, _myLines.Count);
		}

		public virtual Vector3 closestPoint(Vector3 thePoint, int theStart, int theEnd)
		{
			if (theEnd < theStart)
			{
				theEnd += _myLines.Count;
			}
			Vector3 myClosestPoint = new Vector3();
			float myMinDistSq = float.MaxValue;

			for (int i = theStart; i < theEnd;i++)
			{
				CCLine3 myLine = _myLines[i % _myLines.Count];
				Vector3 myPoint = myLine.ClosestPoint(thePoint);
				float myDistSq = Vector3.Distance(myPoint, thePoint);
				if (myDistSq < myMinDistSq)
				{
					myClosestPoint = myPoint;
					myMinDistSq = myDistSq;
				}
			}

			return myClosestPoint;
		}
		public override void Clear()
		{
			base.Clear();
			_myLines.Clear();
		}
    }

}