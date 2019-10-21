using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
	/// In the mathematical field of numerical analysis and in computer graphics, a 
	/// Bezier spline is a spline curve where each polynomial of the spline is in Bezier form.
	/// </para>
	/// <para>
	/// In other words, a Bezier spline is simply a series of Bezier curves joined end to end 
	/// where the last point of one curve coincides with the starting point of the next curve. 
	/// Usually cubic Bezier curves are used, and additional control points (called handles) 
	/// are added to define the shape of each curve.
	/// </para>
	/// <para>
	/// <a href="http://en.wikipedia.org/wiki/Bezier_spline">bezier spline at wikipedia</a>
	/// @author christianriekoff
	/// 
	/// </para>
	/// </summary>
	public class CCBezierSpline : CCSpline
	{


		public CCBezierSpline(bool theIsClosed) : base(CCSplineType.BEZIER, theIsClosed)
		{
			interpolationIncrease = 3;
		}

		public CCBezierSpline() : this(false)
		{
		}


		/// <summary>
		/// Create a spline
		/// </summary>
		/// <param name="theControlPoints">
		///            An array of vector to use as control points of the spline.
		///            The control points should be provided in the appropriate way. 
		///            Each point 'p' describing control position in the scene should 
		///            be surrounded by two handler points. 
		/// 
		///            This applies to every point except for the border points of the curve, 
		///            who should have only one handle point. The pattern should be as follows: 
		///            P0 - H0 : H1 - P1 - H1 : ... : Hn - Pn
		/// 
		///            n is the amount of 'P' - points. </param>
		/// <param name="theIsClosed">
		///            true if the spline cycle. </param>
		public CCBezierSpline(Vector3[] theControlPoints, bool theIsClosed) : base(CCSplineType.BEZIER, theControlPoints, theIsClosed)
		{
			interpolationIncrease = 3;
		}

		/// <summary>
		/// Create a spline
		/// </summary>
		/// <param name="theControlPoints">
		///            An array of vector to use as control points of the spline.
		///            The control points should be provided in the appropriate way. 
		///            Each point 'p' describing control position in the scene should 
		///            be surrounded by two handler points. 
		/// 
		///            This applies to every point except for the border points of the curve, 
		///            who should have only one handle point. The pattern should be as follows: 
		///            P0 - H0 : H1 - P1 - H1 : ... : Hn - Pn
		/// 
		///            n is the amount of 'P' - points. </param>
		/// <param name="theIsClosed">
		///            true if the spline cycle. </param>
		public CCBezierSpline(IList<Vector3> theControlPoints, bool theIsClosed) : base(CCSplineType.BEZIER, theControlPoints, theIsClosed)
		{
			interpolationIncrease = 3;
		}

		public override void AddPoint(Vector3 theControlPoint)
		{
			if (points.Count == 0)
			{
				points.Add(theControlPoint);
				return;
			}

			var myLastPoint = LastPoint;
			points.Add(Vector3.Lerp(myLastPoint, theControlPoint, 0.25f));
			points.Add(Vector3.Lerp(myLastPoint, theControlPoint, 0.75f));
			points.Add(theControlPoint);
			ComputeTotalLength();
		}

        /// <summary>
        /// Adds a new point and control points based on the curve tension
        /// </summary>
        /// <param name="theControlPoint">new point to add</param>
        /// <param name="theCurveTensionA">how much curve tension shoud be used for the outgoing control point</param>
        /// <param name="theCurveTensionB">how much curve tension shoud be used</param>
        public void AddPoint(Vector3 theControlPoint, float theCurveTensionA, float theCurveTensionB)
        {
            if (points.Count == 0)
            {
                points.Add(theControlPoint);
                return;
            }

            var myLastPoint = points[points.Count - 1];
            var myLastControlPoint = points[points.Count - 2];
            points.Add(Vector3.LerpUnclamped(myLastControlPoint, myLastPoint, 1 + theCurveTensionA));
            points.Add(Vector3.Lerp(myLastPoint, theControlPoint, theCurveTensionB));
            points.Add(theControlPoint);
            ComputeTotalLength();
        }

        /// <summary>
        /// This method adds new control points to the spline. The start control point
        /// of the spline will be calculated based on the end control point of the previous 
        /// spline. The first given point will be
        /// the end control point of the spline, the second point the end point. </summary>
        /// <param name="theControlPoint1"> second control point of the spline </param>
        /// <param name="theControlPoint2"> end point of the spline </param>
        public virtual void AddControlPoints(Vector3 theControlPoint1, Vector3 theControlPoint2)
		{
			if (points.Count > 2)
			{
				points.Add(points[points.Count - 1] + (points[points.Count - 1] - (points[points.Count - 2])));
			}
			else
			{
				points.Add(theControlPoint2);
				return;
			}

			points.Add(theControlPoint1);
			points.Add(theControlPoint2);
			
			ComputeTotalLength();
		}

		/// <summary>
		/// Adds a new bezier segment to the spline. The first and second point are optional and can
		/// be null. When point 1 is null the last point of the spline will be used as control point, 
		/// if point 2 is null the given point 3 will be taken as end control point. </summary>
		/// <param name="thePoint1"> start control point can be null </param>
		/// <param name="thePoint2"> end control point can be null </param>
		/// <param name="thePoint3"> end point </param>
		public virtual void AddControlPoints(Vector3 thePoint1, Vector3 thePoint2, Vector3 thePoint3)
		{
			points.Add(thePoint1);
			points.Add(thePoint2);
			points.Add(thePoint3);
			ComputeTotalLength();
		}

		/// <summary>
		/// Compute the length on a bezier spline between control point 1 and 2 </summary>
		/// <param name="theP0"> control point 0 </param>
		/// <param name="theP1"> control point 1 </param>
		/// <param name="theP2"> control point 2 </param>
		/// <param name="theP3"> control point 3 </param>
		/// <returns> the length of the segment </returns>
		private static float BezierLength(Vector3 theP0, Vector3 theP1, Vector3 theP2, Vector3 theP3)
		{
			const float delta = 0.01f;
			float t = 0.0f, result = 0.0f;
			var v1 = theP0;
			while (t <= 1.0f)
			{
				var v2 = CCVector3.BezierPoint(theP0, theP1, theP2, theP3, t);
				result += Vector3.Distance(v1,v2);
				v1.Set(v2.x,v2.y,v2.z);
				t += delta;
			}
			return result;
		}

		/// <summary>
		/// This method calculates the bezier curve length.
		/// </summary>
		protected override void ComputeTotalLengthImpl()
		{
			if (points.Count <= 1) return;
			for (var i = 0; i < points.Count - 3; i += 3)
			{
				var l = BezierLength(points[i], points[i + 1], points[i + 2], points[i + 3]);
				segmentsLengths.Add(l);
				totalLength += l;
			}
		}

		protected override Vector3 Interpolate(float value, int currentControlPoint)
		{
			if (currentControlPoint + 3 >= points.Count)
			{
				return points[currentControlPoint];
			}
			return CCVector3.BezierPoint(points[currentControlPoint], points[currentControlPoint + 1], points[currentControlPoint + 2], points[currentControlPoint + 3], value);
		}

        // evaluate a point on a bezier-curve. t goes from 0 to 1.0
        private static void Bezier( Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t, out Vector3 inControlPoint, out Vector3 anchorPoint, out Vector3 outControlPoint)
        {
            var ab = Vector3.Lerp(a, b, t);           // point between a and b (green)
            var bc = Vector3.Lerp(b, c, t);           // point between b and c (green)
            var cd = Vector3.Lerp(c, d, t);           // point between c and d (green)

            inControlPoint = Vector3.Lerp(ab, bc, t);       // point between ab and bc (blue)
            outControlPoint = Vector3.Lerp(bc, cd, t);       // point between bc and cd (blue)
            anchorPoint = Vector3.Lerp(inControlPoint, outControlPoint, t);   // point on the bezier-curve (black)
        }

        /// <summary>
        /// Interpolate a position on the spline </summary>
        /// <param name="theBlend"> </param>
        /// <param name="theBlendA"> a value from 0 to 1 that represent the position between the first control point and the last one</param>
        /// <param name="theBlendB"></param>
        /// <returns> The Subspline from blend a to blend n</returns>
        public virtual CCBezierSpline SubSpline(float theBlendA, float theBlendB)
        {

            if (points.Count == 0)
            {
                return null;
            }
            if (segmentsLengths == null || segmentsLengths.Count == 0)
            {
                return null;
            }


            int indexA = InterpolationValues(theBlendA, out var myLocalBlendA) * interpolationIncrease;
            int indexB = InterpolationValues(theBlendB, out var myLocalBlendB) * interpolationIncrease;

            Bezier(points[indexA], points[indexA + 1], points[indexA + 2], points[indexA + 3], myLocalBlendA, out _, out var anchorA, out var outControlA);

            var myResult = new CCBezierSpline();
            myResult.BeginEditSpline();
            myResult.points.Add(anchorA);

            myResult.points.Add(indexA == indexB ? Vector3.Lerp(anchorA, outControlA, myLocalBlendB) : outControlA);

            if (indexB > indexA + 3)
            { 
                var inControlA0 = points[indexA + 3 - 1];
                var anchorA0 = points[indexA + 3];
                var outControlA0 = points[indexA + 3 + 1];

                myResult.points.Add(Vector3.Lerp(inControlA0, anchorA0, myLocalBlendA));
                myResult.points.Add(anchorA0);
                myResult.points.Add(outControlA0);
            
           
                for (var i = indexA + 6; i < indexB; i += 3)
                {
	                var inControl = points[i - 1];
	                var anchor = points[i];
	                var outControl = points[i + 1];

                    myResult.points.Add(inControl);
                    myResult.points.Add(anchor);
                    myResult.points.Add(outControl);
                }

                var inControlB0 = points[indexB - 1];
                var anchorB0 = points[indexB];
                var outControlB0 = points[indexB + 1];

                myResult.points.Add(inControlB0);
                myResult.points.Add(anchorB0);
                myResult.points.Add(Vector3.Lerp(anchorB0, outControlB0, myLocalBlendB));
            }else if (indexB == indexA + 3)
            {
	            var inControlA0 = points[indexA + 3 - 1];
	            var anchorA0 = points[indexA + 3];
	            var outControlA0 = points[indexA + 3 + 1];

                myResult.points.Add(Vector3.Lerp(inControlA0, anchorA0, myLocalBlendA));
                myResult.points.Add(anchorA0);
                myResult.points.Add(Vector3.Lerp(anchorA0, outControlA0, myLocalBlendB));
            }

            Bezier(points[indexB], points[indexB + 1], points[indexB + 2], points[indexB + 3], myLocalBlendB, out var inControlB, out var anchorB, out _);

            myResult.points.Add(indexA == indexB ? Vector3.Lerp(inControlB, anchorB, myLocalBlendA) : inControlB);
            myResult.points.Add(anchorB);

            myResult.EndEditSpline();
            return myResult;
        }
        
        public int CurveCount {
	        get
	        {
		        if (Count < 4) return 0;
		        return (Count - 1) / 3;
	        }
        }
        
        private static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
	        t = Mathf.Clamp01(t);
	        var oneMinusT = 1f - t;
	        return
		        3f * oneMinusT * oneMinusT * (p1 - p0) +
		        6f * oneMinusT * t * (p2 - p1) +
		        3f * t * t * (p3 - p2);
        }
        
        public Vector3 Velocity (float theBlend) {
	        int i;
	        if (theBlend >= 1f) {
		        theBlend = 1f;
		        i = Count - 4;
	        }
	        else {
		        theBlend = CCMath.Saturate(theBlend) * CurveCount;
		        i = (int)theBlend;
		        theBlend -= i;
		        i *= 3;
	        }
	        return transform.TransformPoint(GetFirstDerivative(
				this[i], 
				this[i + 1], 
				this[i + 2], 
				this[i + 3], theBlend)
	               ) - transform.position;
        }

        public override void Draw()
        {
            if (Count < 4) return;

            for (var i = 0; i < points.Count - 3;i+=3) {
                var startAnchor = transform.TransformPoint(this[i]);
                var startControl = transform.TransformPoint(this[i + 1]);
                var endControl = transform.TransformPoint(this[i + 2]);
                var endAnchor = transform.TransformPoint(this[i + 3]);
                Handles.DrawBezier(startAnchor, endAnchor, startControl, endControl, Color.white, null, 2);
            }
        }

        public CCBezierSpline Clone()
        {
            CCBezierSpline myResult = new CCBezierSpline();
            myResult.BeginEditSpline();
            points.ForEach(p => myResult.points.Add(p));
            myResult.EndEditSpline();
            return myResult;
        }
    }

}