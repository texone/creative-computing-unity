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
			_myInterpolationIncrease = 3;
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
			_myInterpolationIncrease = 3;
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
			_myInterpolationIncrease = 3;
		}

		public override void AddPoint(Vector3 theControlPoint)
		{
			if (_myPoints.Count == 0)
			{
				_myPoints.Add(theControlPoint);
				return;
			}

			Vector3 myLastPoint = _myPoints[_myPoints.Count - 1];
			_myPoints.Add(Vector3.Lerp(myLastPoint, theControlPoint, 0.25f));
			_myPoints.Add(Vector3.Lerp(myLastPoint, theControlPoint, 0.75f));
			_myPoints.Add(theControlPoint);
		}

        /// <summary>
        /// Adds a new point and control points based on the curve tension
        /// </summary>
        /// <param name="theControlPoint">new point to add</param>
        /// <param name="theCurveTensionA">how much curve tension shoud be used for the outgoing control point</param>
        /// <param name="theCurveTensionB">how much curve tension shoud be used</param>
        public void AddPoint(Vector3 theControlPoint, float theCurveTensionA, float theCurveTensionB)
        {
            if (_myPoints.Count == 0)
            {
                _myPoints.Add(theControlPoint);
                return;
            }

            Vector3 myLastPoint = _myPoints[_myPoints.Count - 1];
            Vector3 myLastControlPoint = _myPoints[_myPoints.Count - 2];
            _myPoints.Add(Vector3.LerpUnclamped(myLastControlPoint, myLastPoint, 1 + theCurveTensionA));
            _myPoints.Add(Vector3.Lerp(myLastPoint, theControlPoint, theCurveTensionB));
            _myPoints.Add(theControlPoint);
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
			if (_myPoints.Count > 2)
			{
				_myPoints.Add(_myPoints[_myPoints.Count - 1] + (_myPoints[_myPoints.Count - 1] - (_myPoints[_myPoints.Count - 2])));
			}
			else
			{
				_myPoints.Add(theControlPoint2);
				return;
			}

			_myPoints.Add(theControlPoint1);
			_myPoints.Add(theControlPoint2);
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
			if (thePoint1 != null)
			{
				_myPoints.Add(thePoint1);
			}
			else
			{
				_myPoints.Add(_myPoints[_myPoints.Count - 1]);
			}
			if (thePoint2 != null)
			{
				_myPoints.Add(thePoint2);
			}
			else
			{
				_myPoints.Add(thePoint3);
			}
			_myPoints.Add(thePoint3);
		}

		/// <summary>
		/// Compute the length on a bezier spline between control point 1 and 2 </summary>
		/// <param name="theP0"> control point 0 </param>
		/// <param name="theP1"> control point 1 </param>
		/// <param name="theP2"> control point 2 </param>
		/// <param name="theP3"> control point 3 </param>
		/// <returns> the length of the segment </returns>
		public static float BezierLength(Vector3 theP0, Vector3 theP1, Vector3 theP2, Vector3 theP3)
		{
			float delta = 0.01f, t = 0.0f, result = 0.0f;
			Vector3 v1 = theP0, v2 = new Vector3();
			while (t <= 1.0f)
			{
					v2 = CCVector3.BezierPoint(theP0, theP1, theP2, theP3, t);
					result += Vector3.Distance(v1,v2);
					v1.Set(v2.x,v2.y,v2.z);
					t += delta;
			}
			return result;
		}

		/// <summary>
		/// This method calculates the bezier curve length.
		/// </summary>
		public override void ComputeTotalLengthImpl()
		{
			if (_myPoints.Count > 1)
			{
				for (int i = 0; i < _myPoints.Count - 3; i += 3)
				{
					float l = BezierLength(_myPoints[i], _myPoints[i + 1], _myPoints[i + 2], _myPoints[i + 3]);
					_mySegmentsLength.Add(l);
					_myTotalLength += l;
				}
			}
		}

		public override Vector3 Interpolate(float value, int currentControlPoint)
		{
			if (currentControlPoint + 3 >= _myPoints.Count)
			{
				return _myPoints[currentControlPoint];
			}
			return CCVector3.BezierPoint(_myPoints[currentControlPoint], _myPoints[currentControlPoint + 1], _myPoints[currentControlPoint + 2], _myPoints[currentControlPoint + 3], value);
		}

		public override IList<Vector3> Points()
		{
			return _myPoints;
		}

        // evaluate a point on a bezier-curve. t goes from 0 to 1.0
        private void Bezier( Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t, out Vector3 inControlPoint, out Vector3 anchorPoint, out Vector3 outControlPoint)
        {
            Vector3 ab = Vector3.Lerp(a, b, t);           // point between a and b (green)
            Vector3 bc = Vector3.Lerp(b, c, t);           // point between b and c (green)
            Vector3 cd = Vector3.Lerp(c, d, t);           // point between c and d (green)

            inControlPoint = Vector3.Lerp(ab, bc, t);       // point between ab and bc (blue)
            outControlPoint = Vector3.Lerp(bc, cd, t);       // point between bc and cd (blue)
            anchorPoint = Vector3.Lerp(inControlPoint, outControlPoint, t);   // point on the bezier-curve (black)
        }

        /// <summary>
        /// Interpolate a position on the spline </summary>
        /// <param name="theBlend"> a value from 0 to 1 that represent the position between the first control point and the last one </param>
        /// <returns> the position </returns>
        public virtual CCBezierSpline SubSpline(float theBlendA, float theBlendB)
        {

            if (_myPoints.Count == 0)
            {
                return null;
            }
            if (_mySegmentsLength == null || _mySegmentsLength.Count == 0)
            {
                return null;
            }


            float myLocalBlendA;
            int indexA = InterpolationValues(theBlendA, out myLocalBlendA) * _myInterpolationIncrease;
            float myLocalBlendB;
            int indexB = InterpolationValues(theBlendB, out myLocalBlendB) * _myInterpolationIncrease;

            Vector3 inControlA;
            Vector3 anchorA;
            Vector3 outControlA;
            Bezier(_myPoints[indexA], _myPoints[indexA + 1], _myPoints[indexA + 2], _myPoints[indexA + 3], myLocalBlendA, out inControlA, out anchorA, out outControlA);

            CCBezierSpline myResult = new CCBezierSpline();
            myResult.BeginEditSpline();
            myResult._myPoints.Add(anchorA);

            if (indexA == indexB)
            {
                myResult._myPoints.Add(Vector3.Lerp(anchorA, outControlA, myLocalBlendB));
            }
            else
            {
                myResult._myPoints.Add(outControlA);
            }

            if (indexB > indexA + 3)
            { 
                Vector3 inControlA0 = _myPoints[indexA + 3 - 1];
                Vector3 anchorA0 = _myPoints[indexA + 3];
                Vector3 outControlA0 = _myPoints[indexA + 3 + 1];

                myResult._myPoints.Add(Vector3.Lerp(inControlA0, anchorA0, myLocalBlendA));
                myResult._myPoints.Add(anchorA0);
                myResult._myPoints.Add(outControlA0);
            
           
                for (int i = indexA + 6; i < indexB; i += 3)
                {
                    Vector3 inControl = _myPoints[i - 1];
                    Vector3 anchor = _myPoints[i];
                    Vector3 outControl = _myPoints[i + 1];

                    myResult._myPoints.Add(inControl);
                    myResult._myPoints.Add(anchor);
                    myResult._myPoints.Add(outControl);
                }

                Vector3 inControlB0 = _myPoints[indexB - 1];
                Vector3 anchorB0 = _myPoints[indexB];
                Vector3 outControlB0 = _myPoints[indexB + 1];

                myResult._myPoints.Add(inControlB0);
                myResult._myPoints.Add(anchorB0);
                myResult._myPoints.Add(Vector3.Lerp(anchorB0, outControlB0, myLocalBlendB));
            }else if (indexB == indexA + 3)
            {
                Vector3 inControlA0 = _myPoints[indexA + 3 - 1];
                Vector3 anchorA0 = _myPoints[indexA + 3];
                Vector3 outControlA0 = _myPoints[indexA + 3 + 1];

                myResult._myPoints.Add(Vector3.Lerp(inControlA0, anchorA0, myLocalBlendA));
                myResult._myPoints.Add(anchorA0);
                myResult._myPoints.Add(Vector3.Lerp(anchorA0, outControlA0, myLocalBlendB));
            }

            Vector3 inControlB;
            Vector3 anchorB;
            Vector3 outControlB;
            Bezier(_myPoints[indexB], _myPoints[indexB + 1], _myPoints[indexB + 2], _myPoints[indexB + 3], myLocalBlendB, out inControlB, out anchorB, out outControlB);

            if (indexA == indexB)
            {
                myResult._myPoints.Add(Vector3.Lerp(inControlB, anchorB, myLocalBlendA));
            }
            else
            {
                myResult._myPoints.Add(inControlB);
            }
            myResult._myPoints.Add(anchorB);

            myResult.EndEditSpline();
            return myResult;
        }

        public void Draw(Color theColor, float theWidth, bool drawHandles = false)
        {
            if (_myPoints.Count < 4) return;

            for (int i = 0; i < _myPoints.Count - 3;i+=3) {
                Vector3 startAncor = _myPoints[i];
                Vector3 startControl = _myPoints[i + 1];
                Vector3 endControl = _myPoints[i + 2];
                Vector3 endAnchor = _myPoints[i + 3];
                //Handles.DrawBezier(startAncor, endAnchor, startControl, endControl, theColor, null, theWidth);

                if (!drawHandles) continue;
                /*
                Handles.color = theColor;
                Handles.DrawSolidDisc(startAncor, new Vector3(0,0,1), 0.05f);
                Handles.DrawSolidDisc(endAnchor, new Vector3(0, 0, 1), 0.05f);
                Handles.DrawSolidDisc(startControl, new Vector3(0, 0, 1), 0.05f);
                Handles.DrawSolidDisc(endControl, new Vector3(0, 0, 1), 0.05f);

                Handles.DrawLine(startAncor, startControl);
                Handles.DrawLine(endAnchor, endControl);
                */
            }

        }

        public CCBezierSpline Clone()
        {
            CCBezierSpline myResult = new CCBezierSpline();
            myResult.BeginEditSpline();
            _myPoints.ForEach(p => myResult._myPoints.Add(p));
            myResult.EndEditSpline();
            return myResult;
        }
    }

}