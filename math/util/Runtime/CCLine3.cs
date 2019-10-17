using System;
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
namespace cc.creativecomputing.math.util
{
	public class CCLine3
	{

		protected internal Vector3 _myStart;
		protected internal Vector3 _myEnd;


		public CCLine3(Vector3 theStart, Vector3 theEnd)
		{
			_myStart = theStart;
			_myEnd = theEnd;
		}

		public CCLine3(float theStartX, float theStartY, float theStartZ, float theEndX, float theEndY, float theEndZ) : this(new Vector3(theStartX, theStartY, theStartZ), new Vector3(theEndX, theEndY, theEndZ))
		{
		}

		public CCLine3(CCLine3 theSegment) : this(theSegment._myStart, theSegment._myEnd)
		{
		}

		public CCLine3() : this(0, 0, 0, 0, 0, 0)
		{
		}

		/// <returns> the start </returns>
		public virtual Vector3 Start()
		{
			return _myStart;
		}

		/// <returns> the end </returns>
		public virtual Vector3 End()
		{
			return _myEnd;
		}

		public virtual float Length()
		{
			return Vector3.Distance(_myStart, _myEnd);
		}

		public override bool Equals(object theSegment)
		{
			if (!(theSegment is CCLine3))
			{
				return false;
			}

			CCLine3 mySegment = (CCLine3)theSegment;
			return mySegment.Start().Equals(Start()) && mySegment.End().Equals(End()) || mySegment.Start().Equals(End()) && mySegment.End().Equals(Start());
		}

		public virtual float ClosestPointBlend(Vector3 thePoint)
		{
			return ClosestPointBlend(thePoint.x, thePoint.y, thePoint.z);
		}

		public virtual float ClosestPointBlend(float theX, float theY, float theZ)
		{
			return CCMath.Saturate(((theX - _myStart.x) * (_myEnd.x - _myStart.x) + (theY - _myStart.y) * (_myEnd.y - _myStart.y) + (theZ - _myStart.z) * (_myEnd.z - _myStart.z)) / Vector3.SqrMagnitude(_myStart -_myEnd));
		}

		/// <summary>
		/// Returns the point on the line that is closest to the given point </summary>
		/// <param name="theX"> x coord of the point </param>
		/// <param name="theY"> y coord of the point </param>
		/// <param name="theZ"> z coord of the point </param>
		/// <returns> the closest point </returns>
		public virtual Vector3 ClosestPoint(float theX, float theY, float theZ)
		{

			float myBlend = ClosestPointBlend(theX, theY, theZ);

			return Vector3.Lerp(_myStart, _myEnd, myBlend);

	//	    if( _myU < 0.0f) {
	//	    	return _myStart.clone();
	//	    }
	//	    
	//	    if(_myU > 1.0f ) {
	//	    	return _myEnd.clone();
	//	    }
	//	 
	//	    return new Vector3(
	//	    	_myStart.x + _myU * (_myEnd.x - _myStart.x),
	//	    	_myStart.y + _myU * (_myEnd.y - _myStart.y),
	//	    	_myStart.z + _myU * (_myEnd.z - _myStart.z)
	//	    );
		}

		/// <summary>
		/// Returns the point on the line that is closest to the given point </summary>
		/// <param name="thePoint"> the point to use for searching </param>
		/// <returns> the closest point on the line to the given point </returns>
		public virtual Vector3 ClosestPoint(Vector3 thePoint)
		{
			return ClosestPoint(thePoint.x, thePoint.y, thePoint.z);
		}

		/// <summary>
		/// Returns the distance of a point to a segment defined by the two end points </summary>
		/// <param name="theX"> </param>
		/// <param name="theY"> </param>
		/// <param name="theZ">
		/// @return </param>
		public virtual float Distance(float theX, float theY, float theZ)
		{
			Vector3 myClosestPoint = ClosestPoint(theX, theY, theZ);
			return Vector3.Distance(myClosestPoint, new Vector3(theX, theY, theZ));
		}

		/// <summary>
		/// Returns the distance of a point to a segment defined by the two end points </summary>
		/// <param name="thePoint"> </param>
		/// <param name="theEndPoint1"> </param>
		/// <param name="theEndPoint2">
		/// @return </param>
		public virtual float Distance(Vector3 theVector)
		{
			return Distance(theVector.x, theVector.y, theVector.z);
		}



		public virtual CCLine3 ClosestLineBetween(CCLine3 theOtherLine)
		{

			Vector3 p13 = _myStart - theOtherLine._myStart;
			Vector3 p43 = theOtherLine._myEnd - theOtherLine._myStart;

			if (Math.Abs(p43.x) <= Double.Epsilon && Math.Abs(p43.y) <= Double.Epsilon && Math.Abs(p43.z) <= Double.Epsilon)
			{
				return null;
			}

			Vector3 p21 = _myEnd - _myStart;

			if (Math.Abs(p21.x) <= Double.Epsilon && Math.Abs(p21.y) <= Double.Epsilon && Math.Abs(p21.z) <= Double.Epsilon)
			{
				return null;
			}

			float d4321 = Vector3.Dot(p43,p21);
			float d4343 = Vector3.Dot(p43,p43);
			float d2121 = Vector3.Dot(p21,p21);

			float denom = d2121 * d4343 - d4321 * d4321;
			if (Math.Abs(denom) < Double.Epsilon)
			{
				return (null);
			}

			float d1343 = Vector3.Dot(p13, p43);
			float d1321 = Vector3.Dot(p13, p21);

			float numer = d1343 * d4321 - d1321 * d4343;

			float mua = numer / denom;
			float mub = (d1343 + d4321 * (mua)) / d4343;

			return new CCLine3(
                _myStart.x + mua * p21.x, 
                _myStart.y + mua * p21.y, 
                _myStart.z + mua * p21.z, 
                theOtherLine._myStart.x + mub * p43.x, 
                theOtherLine._myStart.y + mub * p43.y, 
                theOtherLine._myStart.z + mub * p43.z
               );
		}

		/// <summary>
		/// Returns a string representation of the vector
		/// </summary>
		public override string ToString()
		{
			return "CCLine3f _myStart:[ " + _myStart + " ] end:[ " + _myEnd + " ]";
		}
	}

}