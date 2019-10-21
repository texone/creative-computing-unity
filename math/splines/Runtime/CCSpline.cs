using System;
using System.Collections.Generic;
using UnityEngine;
using cc.creativecomputing.math.util;
using UnityEngine.Serialization;

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
	public abstract class CCSpline : MonoBehaviour
	{

		public List<Vector3> points = new List<Vector3>();

		public bool isClosed;
		public List<float> segmentsLengths = new List<float>();

		public enum CCSplineType
		{
			LINEAR,
			CATMULL_ROM,
			BEZIER,
			NURB,
			BLEND
		}

		public float totalLength;
		private readonly CCSplineType _type;

		protected bool isModified = true;

		protected int interpolationIncrease = 1;

		/// <summary>
		/// Create a spline </summary> 
		/// <param name="theSplineType"> the type of the spline <seealso cref= "CCSplineType" /></param>
		/// <param name="theIsClosed"> true if the spline cycle. </param>
		protected CCSpline(CCSplineType theSplineType, bool theIsClosed)
		{
			isClosed = theIsClosed;
			_type = theSplineType;
		}

		/// <summary>
		/// Create a spline </summary> 
		/// <param name="theSplineType"> the type of the spline <seealso cref= "CCSplineType"/> </param>
		/// <param name="theControlPoints"> an array of vector to use as control points of the spline </param>
		/// <param name="theIsClosed"> true if the spline cycle. </param>
		protected CCSpline(CCSplineType theSplineType, Vector3[] theControlPoints, bool theIsClosed) : this(theSplineType, theIsClosed)
		{
			AddControlPoints(theControlPoints);
		}

		/// <summary>
		/// Create a spline </summary> 
		/// <param name="theSplineType"> the type of the spline <seealso cref= "CCSplineType"/> </param>
		/// <param name="theControlPoints"> a list of vector to use as control points of the spline </param>
		/// <param name="theIsClosed">true if the spline cycle.</param>
		public CCSpline(CCSplineType theSplineType, IList<Vector3> theControlPoints, bool theIsClosed) : this(theSplineType, theIsClosed)
		{
			AddControlPoints(theControlPoints);
		}

		/// <summary>
		/// Use this method to mark the spline as modified, this is only necessary
		/// if you directly add points using the reference passed by the <seealso cref="Points()"/> method.
		/// </summary>
		public virtual void BeginEditSpline()
		{
			if (isModified)
			{
				return;
			}

			isModified = true;
			Debug.Log(points);
			if (points.Count > 2 && isClosed)
			{
				points.RemoveAt(points.Count - 1);
			}

		}

		public virtual void EndEditSpline()
		{
			if (!isModified)
			{
				return;
			}

			isModified = false;

			if (points.Count >= 2 && isClosed)
			{
				points.Add(points[0]);
			}

			if (points.Count > 1)
			{
				ComputeTotalLength();
			}
        }

        /// <summary>
		/// Use this method to invert the order of the spline points
		/// </summary>
        public void Invert()
        {
            var myNewPoints = new List<Vector3>(points);
            points.Clear();
            for (var i = myNewPoints.Count - 1; i >= 0; i--)
            {
                points.Add(myNewPoints[i]);
            }

            if (points.Count > 1)
            {
                ComputeTotalLength();
            }
        }

        /// <summary>
        /// remove the controlPoint from the spline </summary>
        /// <param name="controlPoint"> the controlPoint to remove </param>
        public virtual void RemovePoint(Vector3 controlPoint)
		{
			BeginEditSpline();
			points.Remove(controlPoint);
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
			points.Add(theControlPoint);
		}

		/// <summary>
		/// Adds the given control points to the spline </summary>
		/// <param name="theControlPoints"> </param>
		protected virtual void AddControlPoints(params Vector3[] theControlPoints)
		{
			foreach (var myPoint in theControlPoints)
			{
				AddPoint(myPoint);
			}
		}

		/// <summary>
		/// Adds the given control points to the spline </summary>
		/// <param name="theControlPoints"> </param>
		protected virtual void AddControlPoints(IEnumerable<Vector3> theControlPoints)
		{
			foreach (var myPoint in theControlPoints)
			{
				AddPoint(myPoint);
			}
		}

		/// <summary>
		/// returns this spline control points
		/// @return
		/// </summary>
		public  virtual IList<Vector3> Points => points;

		public int Count
		{
			get
			{
				if (points == null) return 0;
				return points.Count;
			}
		}

		public Vector3 this[int theIndex]
		{
			get => points[theIndex];
			set => points[theIndex] = value;
		}



		public Vector3 LastPoint => points.Count == 0 ? new Vector3() : points[points.Count - 1];

		protected abstract void ComputeTotalLengthImpl();

		/// <summary>
		/// This method computes the total length of the curve.
		/// </summary>
		public virtual void ComputeTotalLength()
		{
			totalLength = 0;

			if (segmentsLengths == null)
			{
				segmentsLengths = new List<float>();
			}
			else
			{
				segmentsLengths.Clear();
			}
			ComputeTotalLengthImpl();
		}

		/// <summary>
		/// Interpolate a position on the spline </summary>
		/// <param name="theBlend"> a value from 0 to 1 that represent the position between the current control point and the next one </param>
		/// <param name="theControlPointIndex"> the current control point </param>
		/// <returns> the position </returns>
		protected abstract Vector3 Interpolate(float theBlend, int theControlPointIndex);

		protected virtual int InterpolationValues(float theBlend, out float theLocalBlend)
        {
            if (segmentsLengths.Count == 0) ComputeTotalLength();
            var myLength = totalLength * Mathf.Clamp01(theBlend);
            float myReachedLength = 0;
            var myIndex = 0;
            while (myIndex < SegmentLengths.Count && myReachedLength + SegmentLengths[myIndex] < myLength)
            {
                myReachedLength += SegmentLengths[myIndex];
                myIndex++;
            }
            
            var myLocalLength = myLength - myReachedLength;
            theLocalBlend = myLocalLength / SegmentLengths[myIndex];
            return myIndex;
        }

		/// <summary>
		/// Interpolate a position on the spline </summary>
		/// <param name="theBlend"> a value from 0 to 1 that represent the position between the first control point and the last one </param>
		/// <returns> the position </returns>
		public virtual Vector3 Interpolate(float theBlend)
		{
            if (points.Count == 0)
            {
                return new Vector3();
            }
            if (points.Count == 1)
            {
	            return new Vector3(points[0].x, points[0].y, points[0].z);
            }
            if (theBlend >= 1)
            {
                return LastPoint;
            }


            if(totalLength == 0)ComputeTotalLength();
            var myIndex = InterpolationValues(theBlend, out var myLocalBlend);
			return transform.TransformPoint(Interpolate(myLocalBlend, myIndex * interpolationIncrease));
		}

		/// <summary>
		/// returns true if the spline cycle
		/// @return
		/// </summary>
		///
		/// 
		public virtual bool Closed
		{
			get => isClosed;

			set
			{
				if (value == isClosed)
				{
					return;
				}
				BeginEditSpline();
				isClosed = value;
				EndEditSpline();
			}
		}

		/// <summary>
		/// return the total length of the spline
		/// @return
		/// </summary>
		public virtual float TotalLength
		{
			get { return totalLength; }
		}

		/// <summary>
		/// return the type of the spline
		/// @return
		/// </summary>
		public CCSplineType Type => _type;

		/// <summary>
		/// Returns the number of segments in this spline
		/// @return
		/// </summary>
		public virtual int NumberOfSegments =>segmentsLengths.Count;

		/// <summary>
		/// returns a list of float representing the segments length
		/// @return
		/// </summary>
		public IList<float> SegmentLengths => segmentsLengths;

		public virtual Vector3 ClosestPoint(Vector3 thePoint)
		{
			var myMinDistanceSq = float.MaxValue;
			var myPoint = new Vector3();
			foreach (Vector3 myControlPoint in points)
			{
				var myDistSq = Vector3.Distance(thePoint,myPoint);
				if (myDistSq >= myMinDistanceSq) continue;
				
				myMinDistanceSq = myDistSq;
				myPoint = myControlPoint;
				
			}
			return myPoint;
		}

		public void ForEach(Action<Vector3> theAction)
		{
			foreach (var vector in points)
			{
				theAction.Invoke(vector);
			}
		}

        /// <summary>
        /// Discretizes the spline on the count points
        /// </summary>
        /// <param name="theCount">resolution of the discretized spline</param>
        /// <returns>List of theCount points on the spline</returns>
        public List<Vector3> Discretize(int theCount)
        {
            var myResult = new List<Vector3>();
            for (var i = 0; i < theCount;i++)
            {
                var myT = CCMath.Map(i, 0, theCount - 1, 0, 1);
                myResult.Add(Interpolate(myT));
            }
            return myResult;
        }

		/// <summary>
		/// Removes all points from the spline
		/// </summary>
		protected virtual void Clear()
		{
			points.Clear();
			segmentsLengths?.Clear();
			totalLength = 0;
		}
		
		public abstract void Draw();

		private void OnDrawGizmos()
		{
			Draw();
		}
	}

}