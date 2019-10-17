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
	/// <para>
	/// In computer graphics splines are popular curves because of the simplicity of their construction, their ease and accuracy of evaluation, and their capacity to approximate complex
	/// shapes through curve fitting and interactive curve design.
	/// </para>
	/// <a href="http://en.wikipedia.org/wiki/Spline_(mathematics)">spline at wikipedia</a>
	/// </summary>
	public abstract class CCSpline
	{

		protected internal List<Vector3> _myPoints = new List<Vector3>();

		protected internal bool _myIsClosed;
		protected internal IList<float> _mySegmentsLength;

		public enum CCSplineType
		{
			LINEAR,
			CATMULL_ROM,
			BEZIER,
			NURB,
			BLEND
		}

		protected internal float _myTotalLength;
		protected internal CCSplineType _myType;

		protected internal bool _myIsModified = true;

		protected internal int _myInterpolationIncrease = 1;

		/// <summary>
		/// Create a spline </summary> </param>
		/// <param name="theSplineType"> the type of the spline <seealso cref= {CCSplineType} </seealso>
		/// <param name="theIsClosed"> true if the spline cycle. </param>
		public CCSpline(CCSplineType theSplineType, bool theIsClosed)
		{
			_myIsClosed = theIsClosed;
			_myType = theSplineType;
		}

		/// <summary>
		/// Create a spline </summary> </param>
		/// <param name="theSplineType"> the type of the spline <seealso cref= {CCSplineType} </seealso>
		/// <param name="theControlPoints"> an array of vector to use as control points of the spline </param>
		/// <param name="theIsClosed"> true if the spline cycle. </param>
		public CCSpline(CCSplineType theSplineType, Vector3[] theControlPoints, bool theIsClosed) : this(theSplineType, theIsClosed)
		{
			AddControlPoints(theControlPoints);
		}

		/// <summary>
		/// Create a spline </summary> </param>
		/// <param name="theSplineType"> the type of the spline <seealso cref= {CCSplineType} </seealso>
		/// <param name="theControlPoints"> a list of vector to use as control points of the spline </param>
		/// <param name="theIsClosed"> true if the spline cycle. </param>
		public CCSpline(CCSplineType theSplineType, IList<Vector3> theControlPoints, bool theIsClosed) : this(theSplineType, theIsClosed)
		{
			AddControlPoints(theControlPoints);
		}

		/// <summary>
		/// Use this method to mark the spline as modified, this is only necessary
		/// if you directly add points using the reference passed by the <seealso cref="#points()"/> method.
		/// </summary>
		public virtual void BeginEditSpline()
		{
			if (_myIsModified)
			{
				return;
			}

			_myIsModified = true;

			if (_myPoints.Count > 2 && _myIsClosed)
			{
				_myPoints.RemoveAt(_myPoints.Count - 1);
			}

		}

		public virtual void EndEditSpline()
		{
			if (!_myIsModified)
			{
				return;
			}

			_myIsModified = false;

			if (_myPoints.Count >= 2 && _myIsClosed)
			{
				_myPoints.Add(_myPoints[0]);
			}

			if (_myPoints.Count > 1)
			{
				ComputeTotalLentgh();
			}
        }

        /// <summary>
		/// Use this method to invert the order of the spline points
		/// </summary>
        public void Invert()
        {
            List<Vector3> myNewPoints = new List<Vector3>(_myPoints);
            _myPoints.Clear();
            for (int i = myNewPoints.Count - 1; i >= 0; i--)
            {
                _myPoints.Add(myNewPoints[i]);
            }

            if (_myPoints.Count > 1)
            {
                ComputeTotalLentgh();
            }
        }

        /// <summary>
        /// remove the controlPoint from the spline </summary>
        /// <param name="controlPoint"> the controlPoint to remove </param>
        public virtual void RemovePoint(Vector3 controlPoint)
		{
			BeginEditSpline();
			_myPoints.Remove(controlPoint);
		}

		/// <summary>
		/// Adds a controlPoint to the spline.
		/// <para>
		/// If you add one control point to a bezier spline and the added point is 
		/// not the first point of the spline, there are two more
		/// points added as control points these points will be the previous point
		/// and the added point, resulting in a straight line.
		/// </para> </summary>
		/// <param name="theControlPoint"> a position in world space </param>
		public virtual void AddPoint(Vector3 theControlPoint)
		{
			BeginEditSpline();
			_myPoints.Add(theControlPoint);
		}

		/// <summary>
		/// Adds the given control points to the spline </summary>
		/// <param name="theControlPoints"> </param>
		public virtual void AddControlPoints(params Vector3[] theControlPoints)
		{
			foreach (Vector3 myPoint in theControlPoints)
			{
				AddPoint(myPoint);
			}
		}

		/// <summary>
		/// Adds the given control points to the spline </summary>
		/// <param name="theControlPoints"> </param>
		public virtual void AddControlPoints(IList<Vector3> theControlPoints)
		{
			foreach (Vector3 myPoint in theControlPoints)
			{
				AddPoint(myPoint);
			}
		}

		/// <summary>
		/// returns this spline control points
		/// @return
		/// </summary>
		public virtual IList<Vector3> Points()
		{
			return _myPoints;
		}

        public Vector3 LastPoint()
        {
            if (_myPoints.Count == 0) return new Vector3();
            return _myPoints[_myPoints.Count - 1];
        }

        public  abstract void ComputeTotalLengthImpl();

		/// <summary>
		/// This method computes the total length of the curve.
		/// </summary>
		protected internal virtual void ComputeTotalLentgh()
		{
			_myTotalLength = 0;

			if (_mySegmentsLength == null)
			{
				_mySegmentsLength = new List<float>();
			}
			else
			{
				_mySegmentsLength.Clear();
			}
			ComputeTotalLengthImpl();
		}

		/// <summary>
		/// Interpolate a position on the spline </summary>
		/// <param name="theBlend"> a value from 0 to 1 that represent the position between the current control point and the next one </param>
		/// <param name="theControlPointIndex"> the current control point </param>
		/// <returns> the position </returns>
		public abstract Vector3 Interpolate(float theBlend, int theControlPointIndex);

        public virtual int InterpolationValues(float theBlend, out float theLocalBlend)
        {
            if (_mySegmentsLength.Count == 0) ComputeTotalLentgh();
            float myLength = _myTotalLength * Mathf.Clamp01(theBlend);
            float myReachedLength = 0;
            int myIndex = 0;
            while (myIndex < _mySegmentsLength.Count && myReachedLength + _mySegmentsLength[myIndex] < myLength)
            {
                myReachedLength += _mySegmentsLength[myIndex];
                myIndex++;
            }
            
            float myLocalLength = myLength - myReachedLength;
            theLocalBlend = myLocalLength / _mySegmentsLength[myIndex];
            return myIndex;
        }

		/// <summary>
		/// Interpolate a position on the spline </summary>
		/// <param name="theBlend"> a value from 0 to 1 that represent the position between the first control point and the last one </param>
		/// <returns> the position </returns>
		public virtual Vector3 Interpolate(float theBlend)
		{
            if (_myPoints.Count == 0)
            {
                return new Vector3();
            }
            if (_mySegmentsLength == null || _mySegmentsLength.Count == 0)
            {
                return new Vector3(_myPoints[0].x, _myPoints[0].y, _myPoints[0].z);
            }
            if (theBlend >= 1)
            {
                return LastPoint();
            }

           
			float myLocalBlend = 0;
            int myIndex = InterpolationValues(theBlend, out myLocalBlend);
			return Interpolate(myLocalBlend, myIndex * _myInterpolationIncrease);
		}

		/// <summary>
		/// returns true if the spline cycle
		/// @return
		/// </summary>
		public virtual bool Closed
		{
			get
			{
				return _myIsClosed;
			}
		}

		/// <summary>
		/// set to true to make the spline cycle </summary>
		/// <param name="theIsClosed"> </param>
		public virtual void IsClosed(bool theIsClosed)
		{
			if (theIsClosed == _myIsClosed)
			{
				return;
			}
			BeginEditSpline();
			_myIsClosed = theIsClosed;
			EndEditSpline();
		}

		/// <summary>
		/// return the total length of the spline
		/// @return
		/// </summary>
		public virtual float TotalLength()
		{
			return _myTotalLength;
		}

		/// <summary>
		/// return the type of the spline
		/// @return
		/// </summary>
		public virtual CCSplineType Type()
		{
			return _myType;
		}

		/// <summary>
		/// Returns the number of segments in this spline
		/// @return
		/// </summary>
		public virtual int NumberOfSegments()
		{
			return _mySegmentsLength.Count;
		}

		/// <summary>
		/// returns a list of float representing the segments length
		/// @return
		/// </summary>
		public virtual IList<float> SegmentsLengths()
		{
			return _mySegmentsLength;
		}

		public virtual Vector3 ClosestPoint(Vector3 thePoint)
		{
			float myMinDistanceSq = float.MaxValue;
			Vector3 myPoint = new Vector3();
			foreach (Vector3 myControlPoint in _myPoints)
			{
				float myDistSq = Vector3.Distance(thePoint,myPoint);
				if (myDistSq < myMinDistanceSq)
				{
					myMinDistanceSq = myDistSq;
					myPoint = myControlPoint;
				}
			}
			return myPoint;
		}

        /// <summary>
        /// Discretizes the spline on the count points
        /// </summary>
        /// <param name="theCount">resolution of the discretized spline</param>
        /// <returns>List of theCount points on the spline</returns>
        public List<Vector3> Discretize(int theCount)
        {
            List<Vector3> myResult = new List<Vector3>();
            for (int i = 0; i < theCount;i++)
            {
                float myT = CCMath.Map(i, 0, theCount - 1, 0, 1);
                myResult.Add(Interpolate(myT));
            }
            return myResult;
        }

		/// <summary>
		/// Removes all points from the spline
		/// </summary>
		public virtual void Clear()
		{
			_myPoints.Clear();
			if (_mySegmentsLength != null)
			{
				_mySegmentsLength.Clear();
			}
			_myTotalLength = 0;
		}

	}

}