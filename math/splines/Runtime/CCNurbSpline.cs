using System;
using System.Collections.Generic;
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


	/// <summary>
	/// <para>
	/// Non-uniform rational basis spline (NURBS) is a mathematical model commonly 
	/// used in computer graphics for generating and representing curves and surfaces 
	/// which offers great flexibility and precision for handling both analytic and 
	/// freeform shapes.
	/// </para>
	/// <para>
	/// They allow representation of geometrical shapes in a compact form. They can be 
	/// efficiently handled by the computer programs and yet allow for easy human interaction. 
	/// NURBS surfaces are functions of two parameters mapping to a surface in three-dimensional 
	/// space. The shape of the surface is determined by control points.
	/// </para>
	/// <para>
	/// A NURBS curve is defined by its order, a set of weighted control points, and a knot 
	/// vector. NURBS curves and surfaces are generalizations of both B-splines and Bezier 
	/// curves and surfaces, the primary difference being the weighting of the control points 
	/// which makes NURBS curves rational (non-rational B-splines are a special case of 
	/// rational B-splines). Whereas Bezier curves evolve into only one parametric direction, 
	/// usually called s or u, NURBS surfaces evolve into two parametric directions, called s and 
	/// t or u and v.
	/// </para>
	/// <a href="http://en.wikipedia.org/wiki/Non-uniform_rational_B-spline">nurbs at wikipedia</a>
	/// @author christianriekoff
	/// 
	/// </summary>
	public class CCNurbSpline : CCSpline
	{

		private const float KNOTS_MINIMUM_DELTA = 0.0001f;

		private IList<float> _myKnots; // knots of NURBS spline
		private float[] _myWeights; // weights of NURBS spline
		private int _myBasisFunctionDegree; // degree of NURBS spline basis function
											// (computed automatically)

		/// <summary>
		/// Create a NURBS spline. A spline type is automatically set to
		/// SplineType.Nurb. The cycle is set to <b>false</b> by default.
		/// </summary>
		/// <param name="controlPoints">  a list of vector to use as control points of the spline </param>
		/// <param name="nurbKnots"> the nurb's spline knots </param>
		public CCNurbSpline(IList<Vector4> controlPoints, IList<float> nurbKnots) : base(CCSplineType.NURB, false)
		{
			// input data control
			for (int i = 0; i < nurbKnots.Count - 1; ++i)
			{
				if (nurbKnots[i] > nurbKnots[i + 1])
				{
					throw new System.ArgumentException("The knots values cannot decrease!");
				}
			}

			// storing the data
			_myWeights = new float[controlPoints.Count];
			_myKnots = nurbKnots;
			_myBasisFunctionDegree = nurbKnots.Count - _myWeights.Length;

			for (int i = 0; i < controlPoints.Count; ++i)
			{
				Vector4 controlPoint = controlPoints[i];
				_myPoints.Add(new Vector3(controlPoint.x,controlPoint.y, controlPoint.z));
				_myWeights[i] = controlPoint.w;
			}
			prepareNurbsKnots(_myKnots, _myBasisFunctionDegree);
			ComputeTotalLentgh();
		}

		/// <summary>
		/// This method prepares the knots to be used. If the knots represent
		/// non-uniform B-splines (first and last knot values are being repeated) it
		/// leads to NaN results during calculations. This method adds a small number
		/// to each of such knots to avoid NaN's.
		/// </summary>
		/// <param name="knots">
		///            the knots to be prepared to use </param>
		/// <param name="basisFunctionDegree">
		///            the degree of basis function </param>
		// TODO: improve this; constant delta may lead to errors if the difference
		// between tha last repeated
		// point and the following one is lower than it
		private void prepareNurbsKnots(IList<float> knots, int basisFunctionDegree)
		{
			float delta = KNOTS_MINIMUM_DELTA;
			float prevValue = knots[0];
			for (int i = 1; i < knots.Count; ++i)
			{
				float value = knots[i];
				if (value <= prevValue)
				{
					value += delta;
					knots[i] =value;
					delta += KNOTS_MINIMUM_DELTA;
				}
				else
				{
					delta = KNOTS_MINIMUM_DELTA; // reset the delta's value
				}

				prevValue = value;
			}
		}

		public override void ComputeTotalLengthImpl()
		{
			// TODO implement this

		}

		/// <summary>
		/// This method computes the base function value for the NURB curve.
		/// </summary>
		/// <param name="i">
		///            the knot index </param>
		/// <param name="k">
		///            the base function degree </param>
		/// <param name="t">
		///            the knot value </param>
		/// <param name="knots">
		///            the knots' values </param>
		/// <returns> the base function value </returns>
		private float ComputeBaseFunctionValue(int i, int k, float t, IList<float> knots)
		{
			if (k == 1)
			{
				return knots[i] <= t && t < knots[i + 1] ? 1.0f : 0.0f;
			}
			else
			{
				return (t - knots[i]) / (knots[i + k - 1] - knots[i]) * ComputeBaseFunctionValue(i, k - 1, t, knots) + (knots[i + k] - t) / (knots[i + k] - knots[i + 1]) * ComputeBaseFunctionValue(i + 1, k - 1, t, knots);
			}
		}

		public override Vector3 Interpolate(float value, int currentControlPoint)
		{
			int controlPointAmount = _myPoints.Count;

			Vector3 store = new Vector3();
			float delimeter = 0;

			for (int i = 0; i < controlPointAmount; ++i)
			{
				float val = _myWeights[i] * ComputeBaseFunctionValue(i, _myBasisFunctionDegree, value, _myKnots);
				store += _myPoints[i] * val;
				delimeter += val;
			}
			return store *= 1f / delimeter;
		}

		// ////////// NURBS getters /////////////////////

		/// <summary>
		/// This method returns the minimum nurb curve knot value. Check the nurb
		/// type before calling this method. It the curve is not of a Nurb type - NPE
		/// will be thrown.
		/// </summary>
		/// <returns> the minimum nurb curve knot value </returns>
		public virtual float MinNurbKnot()
		{
			return _myKnots[_myBasisFunctionDegree - 1];
		}

		/// <summary>
		/// This method returns the maximum nurb curve knot value. Check the nurb
		/// type before calling this method. It the curve is not of a Nurb type - NPE
		/// will be thrown.
		/// </summary>
		/// <returns> the maximum nurb curve knot value </returns>
		public virtual float MaxNurbKnot()
		{
			return _myKnots[_myWeights.Length];
		}

		/// <summary>
		/// This method returns NURBS' spline knots.
		/// </summary>
		/// <returns> NURBS' spline knots </returns>
		public virtual IList<float> knots()
		{
			return _myKnots;
		}

		/// <summary>
		/// This method returns NURBS' spline weights.
		/// </summary>
		/// <returns> NURBS' spline weights </returns>
		public virtual float[] Weights()
		{
			return _myWeights;
		}

		/// <summary>
		/// This method returns NURBS' spline basis function degree.
		/// </summary>
		/// <returns> NURBS' spline basis function degree </returns>
		public virtual int BasisFunctionDegree()
		{
			return _myBasisFunctionDegree;
		}
    }

}