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

	/// <summary>
	/// In computer graphics, Catmull-Rom splines are frequently used to get smooth interpolated 
	/// motion between key frames. For example, most camera path animations generated from discrete 
	/// key-frames are handled using Catmull-Rom splines. They are popular mainly for being relatively 
	/// easy to compute, guaranteeing that each key frame position will be hit exactly, and also 
	/// guaranteeing that the tangents of the generated curve are continuous over multiple segments.
	/// @author christianriekoff
	/// 
	/// </summary>
	public class CCCatmulRomSpline : CCSpline
	{
		private float _myCurveTension = 0.5f;

		public CCCatmulRomSpline(float theCurveTension, bool theIsClosed) : base(CCSplineType.CATMULL_ROM, theIsClosed)
		{
			_myCurveTension = theCurveTension;
		}

		public CCCatmulRomSpline(Vector3[] theControlPoints, float theCurveTension, bool theIsClosed) : base(CCSplineType.CATMULL_ROM, theControlPoints, theIsClosed)
		{
			_myCurveTension = theCurveTension;
		}

		public CCCatmulRomSpline(IList<Vector3> theControlPoints, float theCurveTension, bool theIsClosed) : base(CCSplineType.CATMULL_ROM, theControlPoints, theIsClosed)
		{
		}

		public override void BeginEditSpline()
		{
			if (_myIsModified)
			{
				return;
			}

			_myIsModified = true;

			if (_myPoints.Count < 2)
			{
				return;
			}

			_myPoints.RemoveAt(0);
			_myPoints.RemoveAt(_myPoints.Count - 1);

			if (_myIsClosed)
			{
				_myPoints.RemoveAt(_myPoints.Count - 1);
			}
		}

		public override void EndEditSpline()
		{
			if (!_myIsModified)
			{
				return;
			}

			_myIsModified = false;

			if (_myPoints.Count < 2)
			{
				return;
			}
			if (_myIsClosed)
			{
				_myPoints.Insert(0,_myPoints[_myPoints.Count - 1]);
				_myPoints.Add(_myPoints[1]);
				_myPoints.Add(_myPoints[2]);
			}
			else
			{
				_myPoints.Insert(0,_myPoints[0]);
				_myPoints.Add(_myPoints[_myPoints.Count - 1]);
			}
			ComputeTotalLentgh();
		}

		/// <summary>
		/// Compute the length on a catmull rom spline between control point 1 and 2 </summary>
		/// <param name="theP0"> control point 0 </param>
		/// <param name="theP1"> control point 1 </param>
		/// <param name="theP2"> control point 2 </param>
		/// <param name="theP3"> control point 3 </param>
		/// <param name="theStartRange"> the starting range on the segment (use 0) </param>
		/// <param name="theEndRange"> the end range on the segment (use 1) </param>
		/// <param name="theCurveTension"> the curve tension </param>
		/// <returns> the length of the segment </returns>
		private float CatmullRomLength(Vector3 theP0, Vector3 theP1, Vector3 theP2, Vector3 theP3, float theStartRange, float theEndRange, float theCurveTension)
		{

			float epsilon = 0.001f;
			float middleValue = (theStartRange + theEndRange) * 0.5f;
			Vector3 start = theP1;
			if (theStartRange != 0)
			{
				start = CCVector3.CatmulRomPoint(theP0, theP1, theP2, theP3, theStartRange, theCurveTension);
			}
			Vector3 end = theP2;
			if (theEndRange != 1)
			{
				end = CCVector3.CatmulRomPoint(theP0, theP1, theP2, theP3, theEndRange, theCurveTension);
			}
			Vector3 middle = CCVector3.CatmulRomPoint(theP0, theP1, theP2, theP3, middleValue, theCurveTension);
			float l = Vector3.Distance(end,start);
			float l1 = Vector3.Distance(middle,start);
			float l2 = Vector3.Distance(end, middle);
			float len = l1 + l2;
			if (l + epsilon < len)
			{
				l1 = CatmullRomLength(theP0, theP1, theP2, theP3, theStartRange, middleValue, theCurveTension);
				l2 = CatmullRomLength(theP0, theP1, theP2, theP3, middleValue, theEndRange, theCurveTension);
			}
			l = l1 + l2;
			return l;
		}

		/// <summary>
		/// Compute the length on a catmull rom spline between control point 1 and 2 </summary>
		/// <param name="theP0"> control point 0 </param>
		/// <param name="theP1"> control point 1 </param>
		/// <param name="theP3"> control point 2 </param>
		/// <param name="theP4"> control point 3 </param>
		/// <param name="theCurveTension"> the curve tension </param>
		/// <returns> the length of the segment </returns>
		private float CatmullRomLength(Vector3 theP0, Vector3 theP1, Vector3 theP3, Vector3 theP4, float theCurveTension)
		{
			return CatmullRomLength(theP0, theP1, theP3, theP4, 0, 1, theCurveTension);
		}

		public override void ComputeTotalLengthImpl()
		/// <summary>
		/// This method computes the Catmull Rom curve length.
		/// </summary>
		{
			if (_myPoints.Count > 3)
			{
				for (int i = 0; i < _myPoints.Count - 3; i++)
				{
					float l = CatmullRomLength(_myPoints[i], _myPoints[i + 1], _myPoints[i + 2], _myPoints[i + 3], _myCurveTension);
					_mySegmentsLength.Add(l);
					_myTotalLength += l;
				}
			}
		}

		public override Vector3 Interpolate(float value, int currentControlPoint)
		{
			EndEditSpline();
			if (currentControlPoint + 3 >= _myPoints.Count)
			{
				return _myPoints[currentControlPoint];
			}
			return CCVector3.CatmulRomPoint(_myPoints[currentControlPoint], _myPoints[currentControlPoint + 1], _myPoints[currentControlPoint + 2], _myPoints[currentControlPoint + 3], value, _myCurveTension);


		}
	//	@Override
	//	public Vector3 interpolate(float value, int currentControlPoint) {
	//		endEditSpline();
	//		return cubicInterpolate(
	//			_myPoints.get(currentControlPoint), 
	//			_myPoints.get(currentControlPoint + 1), 
	//			_myPoints.get(currentControlPoint + 2), 
	//			_myPoints.get(currentControlPoint + 3), 
	//			value, _myCurveTension
	//		);
	//		
	//		
	//	}
	//	 private Vector3 cubicInterpolate( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t , float theTension)
	//     {
	//		 
	//           return new Vector3(
	//        		   p1.x + 0.5f * t * (p2.x - p0.x + t * (2 * p0.x - 5 * p1.x + 4 * p2.x - p3.x + t * (3 * (p1.x - p2.x) + p3.x - p0.x))),
	//        		   p1.y + 0.5f * t * (p2.y - p0.y + t * (2 * p0.y - 5 * p1.y + 4 * p2.y - p3.y + t * (3 * (p1.y - p2.y) + p3.y - p0.y))),
	//        		   p1.z + 0.5f * t * (p2.z - p0.z + t * (2 * p0.z - 5 * p1.z + 4 * p2.z - p3.z + t * (3 * (p1.z - p2.z) + p3.z - p0.z)))
	//        	);
	//     }

		public virtual Vector3 ClosestPoint(Vector3 theClosestPoint, int theStart, int theEnd)
		{
			if (_myPoints.Count < 4)
			{
				return new Vector3();
			}

			if (theStart > theEnd)
			{
				int myTemp = theStart;
				theStart = theEnd;
				theEnd = myTemp;
			}

			Vector3 myNearestPoint = new Vector3();
			float myMinDistance = float.MaxValue;

			for (int i = theStart; i < theEnd; i++)
			{
				for (int j = 0; j < 60; j++)
				{
					Vector3 myTest = Interpolate(j / 60f, i);
					float myDistance = Vector3.Distance(myTest,theClosestPoint);

					if (myDistance < myMinDistance)
					{
						myMinDistance = myDistance;
						myNearestPoint = myTest;
					}
				}
			}

			return myNearestPoint;
		}

		public override Vector3 ClosestPoint(Vector3 theClosestPoint)
		{
			return ClosestPoint(theClosestPoint, 0, _myPoints.Count - 3);
		}

		/// <summary>
		/// returns the curve tension
		/// 
		/// @return
		/// </summary>
		public virtual float curveTension()
		{
			return _myCurveTension;
		}

		/// <summary>
		/// sets the curve tension
		/// </summary>
		/// <param name="_myCurveTension">
		///            the tension </param>
		public virtual void curveTension(float theCurveTension)
		{
			_myCurveTension = theCurveTension;
			ComputeTotalLentgh();
		}



		public override IList<Vector3> Points()
		{
			if (_myPoints.Count < 2)
			{
				return _myPoints;
			}
			return _myPoints.GetRange(1, _myPoints.Count - 1);
		}

		/// <summary>
		/// For the catmulrom spline two extra vertices are inserted at the end 
		/// and the beginning of the curve to create a nice curve. To get all points
		/// used for drawing and interpolating the curve call this method instead of
		/// <seealso cref="#points()"/> </summary>
		/// <returns> all vertices used to draw the curve </returns>
		public virtual IList<Vector3> curvePoints()
		{
			return _myPoints;
		}
    }

}