using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace cc.creativecomputing.math.util {
public class CCVector3 
{
    /// <summary>
    /// Interpolate a spline between at least 4 control points following the
    /// Catmull-Rom equation. here is the interpolation matrix m = [ 0.0 1.0 0.0
    /// 0.0 ] [-T 0.0 T 0.0 ] [ 2T T-3 3-2T -T ] [-T 2-T T-2 T ] where T is the
    /// tension of the curve the result is a value between p1 and p2, t=0 for p1,
    /// t=1 for p2
    /// </summary>
    /// <param name="theU">
    ///            value from 0 to 1 </param>
    /// <param name="theT">
    ///            The tension of the curve </param>
    /// <param name="theP0">
    ///            control point 0 </param>
    /// <param name="theP1">
    ///            control point 1 </param>
    /// <param name="theP2">
    ///            control point 2 </param>
    /// <param name="theP3">
    ///            control point 3 </param>
    /// <param name="store">
    ///            a Vector3f to store the result </param>
    /// <returns> catmull-Rom interpolation </returns>
    public static Vector3 CatmulRomPoint(Vector3 theP0, Vector3 theP1, Vector3 theP2, Vector3 theP3, float theU, float theT)
    {
        return new Vector3(
            CCMath.CatmullRomBlend(theP0.x, theP1.x, theP2.x, theP3.x, theU, theT), 
            CCMath.CatmullRomBlend(theP0.y, theP1.y, theP2.y, theP3.y, theU, theT), 
            CCMath.CatmullRomBlend(theP0.z, theP1.z, theP2.z, theP3.z, theU, theT)
        );
    }

        /// <summary>
		/// Evaluates quadratic bezier at point t for points a, b, c, d.
		/// t varies between 0 and 1, and a and d are the on curve points,
		/// b and c are the control points. this can be done once with the
		/// x coordinates and a second time with the y coordinates to get
		/// the location of a bezier curve at t.
		/// <P>
		/// For instance, to convert the following example:<PRE>
		/// stroke(255, 102, 0);
		/// line(85, 20, 10, 10);
		/// line(90, 90, 15, 80);
		/// stroke(0, 0, 0);
		/// bezier(85, 20, 10, 10, 90, 90, 15, 80);
		/// 
		/// // draw it in gray, using 10 steps instead of the default 20
		/// // this is a slower way to do it, but useful if you need
		/// // to do things with the coordinates at each step
		/// stroke(128);
		/// beginShape(LINE_STRIP);
		/// for (int i = 0; i <= 10; i++) {
		///   double t = i / 10.0f;
		///   double x = bezierPoint(85, 10, 90, 15, t);
		///   double y = bezierPoint(20, 10, 90, 80, t);
		///   vertex(x, y);
		/// }
		/// endShape();</PRE>
		/// </summary>
        public static Vector3 BezierPoint(Vector3 theStartPoint, Vector3 theStartAnchor, Vector3 theEndAnchor, Vector3 theEndPoint, float t)
        {
            float t1 = 1.0f - t;
            return new Vector3(
                theStartPoint.x * t1 * t1 * t1 + 3 * theStartAnchor.x * t * t1 * t1 + 3 * theEndAnchor.x * t * t * t1 + theEndPoint.x * t * t * t, 
                theStartPoint.y * t1 * t1 * t1 + 3 * theStartAnchor.y * t * t1 * t1 + 3 * theEndAnchor.y * t * t * t1 + theEndPoint.y * t * t * t, 
                theStartPoint.z * t1 * t1 * t1 + 3 * theStartAnchor.z * t * t1 * t1 + 3 * theEndAnchor.z * t * t * t1 + theEndPoint.z * t * t * t
            );
        }
    }
}