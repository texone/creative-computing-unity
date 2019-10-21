using System;
using System.Collections.Generic;
using UnityEngine;
using cc.creativecomputing.math.util;
using UnityEditor;

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

		private readonly IList<CCLine3> _myLines = new List<CCLine3>();

		public CCLinearSpline() : this(false)
		{
		}

		public CCLinearSpline(bool theIsClosed) : base(CCSplineType.LINEAR, theIsClosed)
		{
		}

		public CCLinearSpline(Vector3[] theControlPoints, bool theIsClosed) : base(CCSplineType.LINEAR, theControlPoints, theIsClosed)
		{
		}

		public CCLinearSpline(IList<Vector3> theControlPoints, bool theIsClosed) : base(CCSplineType.LINEAR, theControlPoints, theIsClosed)
		{
		}

		protected override void ComputeTotalLengthImpl()
		{
			_myLines.Clear();
			if (points.Count <= 1) return;
			
			for (var i = 0; i < points.Count - 1; i++)
			{
				var myLine = new CCLine3(points[i], points[i + 1]);
				var myLength = myLine.Length();
				segmentsLengths.Add(myLength);
				totalLength += myLength;
				_myLines.Add(myLine);
			}
		}

		protected override Vector3 Interpolate(float value, int currentControlPoint)
		{
			EndEditSpline();
			return Vector3.Lerp(points[currentControlPoint], points[currentControlPoint + 1], value);
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
			return ClosestPoint(thePoint, 0, _myLines.Count);
		}

		public virtual Vector3 ClosestPoint(Vector3 thePoint, int theStart, int theEnd)
		{
			if (theEnd < theStart)
			{
				theEnd += _myLines.Count;
			}
			var myClosestPoint = new Vector3();
			var myMinDistSq = float.MaxValue;

			for (var i = theStart; i < theEnd;i++)
			{
				var myLine = _myLines[i % _myLines.Count];
				var myPoint = myLine.ClosestPoint(thePoint);
				var myDistSq = Vector3.Distance(myPoint, thePoint);
				if (myDistSq < myMinDistSq)
				{
					myClosestPoint = myPoint;
					myMinDistSq = myDistSq;
				}
			}

			return myClosestPoint;
		}

		protected override void Clear()
		{
			base.Clear();
			_myLines.Clear();
		}

		public override void Draw()
		{
			Gizmos.color = Color.white;
			for (var i = 0; i < Count - 1;i++)
			{
				Handles.DrawLine(
					transform.TransformPoint(this[i]), 
					transform.TransformPoint(this[i + 1]));
			}
		}

		

		
    }

}