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

	/// <summary>
	/// <para>
	/// This class contains methods for performing basic numeric operations such as the elementary exponential, 
	/// logarithm, Square root, and trigonometric functions.
	/// </para> 
	/// @author christianriekoff
	/// 
	/// </summary>
	public class CCMath
	{
		/// <summary>
		/// Square root of 2
		/// </summary>
		public static readonly float SQRT2 = (float) Math.Sqrt(2);

		/// <summary>
		/// Square root of 3
		/// </summary>
		public static readonly float SQRT3 = (float) Math.Sqrt(3);

		public const float ALLOWED_DEVIANCE = 0.000001f;
        

		/// <summary>
		/// A "close to zero" float epsilon value for use </summary>
		public const float FLT_EPSILON = 1.1920928955078125E-6f;

		/// <summary>
		/// A "close to zero" float epsilon value for use </summary>
		public const float ZERO_TOLERANCE = 0.0001f;

		public static readonly float PI = (float) Math.PI;
		public static readonly float HALF_PI = PI / 2.0f;
		public static readonly float THIRD_PI = PI / 3.0f;
		public static readonly float QUARTER_PI = PI / 4.0f;
		public static readonly float TWO_PI = PI * 2.0f;

		public static readonly float ONE_THIRD = 1f / 3;

		public static readonly float DEG_TO_RAD = PI / 180.0f;
		public static readonly float RAD_TO_DEG = 180.0f / PI;

		public static readonly CCRandom RANDOM = new CCRandom();
		public static readonly CCFastRandom FAST_RANDOM = new CCFastRandom();

		 /// <summary>
		 /// Returns true if the number is a Power of 2 (2,4,8,16...)
		 /// 
		 /// A good implementation found on the Java boards. note: a number is a Power of two if and only if it is the
		 /// smallest number with that number of significant bits. Therefore, if you subtract 1, you know that the new number
		 /// will have fewer bits, so ANDing the original number with anything less than it will give 0.
		 /// </summary>
		 /// <param name="number">
		 ///            The number to test. </param>
		 /// <returns> True if it is a Power of two. </returns>
		public static bool IsPowerOfTwo(int number)
		{
			return number > 0 && (number & number - 1) == 0;
		}

		/// <param name="number"> </param>
		/// <returns> the closest Power of two to the given number. </returns>
		public static int NearestPowerOfTwo(int number)
		{
			return (int) Math.Pow(2, Math.Ceiling(Math.Log(number) / Math.Log(2)));
		}

		public static float Random()
		{
			return FAST_RANDOM.Random();
		}
        
		public static float Random(float theMax)
		{
			return FAST_RANDOM.Random(theMax);
		}
        
		public static float Random(float theMin, float theMax)
		{
			return FAST_RANDOM.Random(theMin, theMax);
		}

		public static float GaussianRandom()
		{
			return FAST_RANDOM.GaussianRandom();
		}
        
		public static float gaussianRandom(float theMax)
		{
			return FAST_RANDOM.GaussianRandom(theMax);
		}
        
		public static float GaussianRandom(float theMin, float theMax)
		{
			return FAST_RANDOM.GaussianRandom(theMin, theMax);
		}
        
		public static void RandomSeed(int theSeed)
		{
			FAST_RANDOM.RandomSeed((int)theSeed);
			RANDOM.RandomSeed(theSeed);
		}

		/// <summary>
		/// returns the bitwise representation of a floating point number
		/// </summary>
		public static int FloatToBits(float theValue)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(theValue), 0);
		}

		/// <summary>
		/// returns the absolute value of a floating point number in bitwise form
		/// </summary>
		public static int FloatToAbsluteBits(float theValue)
		{
			return (FloatToBits(theValue) & 0x7FFFFFFF);
		}

		/// <summary>
		/// returns the signal bit of a floating point number
		/// </summary>
		public static int SignalBit(float theValue)
		{
			return FloatToBits(theValue) & unchecked((int)0x80000000);
		}

		/// <summary>
		/// returns the value of 1.0f in bitwise form
		/// </summary>
		public static int OneInBits()
		{
			return 0x3F800000;
		}

		/// <summary>
		///  Convert's an angle in radians to one in degrees.
		/// </summary>
		///  <param name="rad">  The angle in radians to be converted. </param>
		///  <returns> The angle in degrees.
		///  </returns>
		public static float RadiansToDegrees(float rad)
		{
			return rad * 180 / PI;
		}

		/// <summary>
		///  Convert's an angle in degrees to one in radians.
		/// </summary>
		///  <param name="deg">  The angle in degrees to be converted. </param>
		///  <returns> The angle in radians.
		///  </returns>
		public static float DegreesToRadians(float deg)
		{
			return deg * PI / 180;
		}

		public static float Mag(float[] abc)
		{
			return (float) Math.Sqrt(abc[0] * abc[0] + abc[1] * abc[1] + abc[2] * abc[2]);
		}

		//////////////////////////////////////////////////////////////

		// MATH

		// lots of convenience methods for math with floats.

		public static int Abs(int theValue)
		{
			int y = theValue >> 31;
			return (theValue ^ y) - y;
		}

		public static float Abs(float theValue)
		{
			return theValue < 0 ? -theValue : theValue;
		}

		public static float Sq(float a)
		{
			return a * a;
		}

		public static float Sqrt(float a)
		{
			return Mathf.Sqrt(a);
		}

		/// <summary>
		/// Returns 1/Sqrt(fValue)
		/// </summary>
		/// <param name="theValue"> The value to process. </param>
		/// <returns> 1/Sqrt(fValue) </returns>
		/// <seealso cref= java.lang.Math#Sqrt(float) </seealso>
		public static float invSqrt(float theValue)
		{
			return 1.0f / Sqrt(theValue);
		}

		public static float Log(float a)
		{
			return (float) Math.Log(a);
		}

		public static float Log2(float a)
		{
			return (float)(Math.Log(a) / Log(2));
		}

		public static float Log10(float a)
		{
			return (float) Math.Log10(a);
		}

		public static float exp(float a)
		{
			return (float) Math.Exp(a);
		}

		public static float Pow(float a, float b)
		{
			return (float) Math.Pow(a, b);
		}

		public static int Pow(int a, int b)
		{
			return (int)Math.Pow(a, b);
		}

		public static float Max(float a, float b)
		{
			return Math.Max(a, b);
		}

		public static float Max(float a, float b, float c)
		{
			//return Math.max(a, Math.max(b, c));
			return (a > b) ? ((a > c) ? a : c) : ((b > c) ? b : c);
		}

		public static float Max(params float[] theValues)
		{
			float result = float.Epsilon;
			foreach (float myValue in theValues)
			{
				result = Max(result,myValue);
			}
			return result;
		}

		public static float Min(float a, float b)
		{
	//		return Math.min(a, b);
			return (a < b) ? a : b;
		}

		public static float Min(float a, float b, float c)
		{
			//return Math.min(a, Math.min(b, c));
			return (a < b) ? ((a < c) ? a : c) : ((b < c) ? b : c);
		}
		/// <summary>
		/// constrains a value to a range between two floats. </summary>
		/// <param name="value"> </param>
		/// <param name="theMin"> minimum output value </param>
		/// <param name="theMax"> maximum output value
		/// @return </param>
		public static float Clamp(float value, float theMin, float theMax)
		{
			return  Max(theMin,Min(value,theMax));
		}

		
        /*
		public static int Clamp(int value, int theMin, int theMax)
		{
			return Max(theMin,Min(value,theMax));
		}*/

		public static float min(params float[] theValues)
		{
			float result = float.MaxValue;
			foreach (float myValue in theValues)
			{
				result = min(result,myValue);
			}
			return result;
		}

		/// <summary>
		/// Blends between a start and an end value according to the given blend.
		/// The blend parameter is the amount to interpolate between the two values 
		/// where 0.0 equal to the first point, 0.1 is very near the first point, 
		/// 0.5 is half-way in between, etc. The blend function is convenient for 
		/// creating motion along a straight path and for drawing dotted lines.
		/// </summary>
		/// <param name="theStart"> first value </param>
		/// <param name="theStop"> second value </param>
		/// <param name="theBlend"> between 0.0 and 1.0
		/// @return </param>
		public static float Blend(float theStart, float theStop, float theBlend)
		{
			return theStart + (theStop - theStart) * theBlend;
		}
        
		public static long Blend(long theStart, long theStop, float theBlend)
		{
			return (long)(theStart + (theStop - theStart) * theBlend);
		}
        
		public static float[] Blend(float[] theStart, float[] theStop, float theBlend)
		{
			float[] result = new float[Min(theStart.Length, theStop.Length)];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Blend(theStart[i], theStop[i], theBlend);
			}
			return result;
		}

		public static float blend(float theBlendU, float theBlendV, float theA, float theB, float theC)
		{
			// Compute vectors        
			float v0 = theC - theA;
			float v1 = theB - theA;

			float myResult = theA;
			myResult += v0 * theBlendU;
			myResult += v1 * theBlendV;
			return myResult;
		}

		/// <summary>
		/// <para>Normalizes a number from another range into a value between 0 and 1.</para>
		/// <para>Identical to map(value, low, high, 0, 1);</para>
		/// <para>Numbers outside the range are not clamped to 0 and 1, because out-of-range 
		/// values are often intentional and useful.</para>
		/// </summary>
		/// <param name="theValue"> The incoming value to be converted </param>
		/// <param name="theMin"> Lower bound of the value's current range </param>
		/// <param name="theMax"> Upper bound of the value's current range
		/// @return </param>
		public static float Norm(float theValue, float theMin, float theMax)
		{
			return (theValue - theMin) / (theMax - theMin);
		}

		/// <summary>
		/// <para>
		/// Re-maps a number from one range to another. In the example above, the number '25' 
		/// is converted from a value in the range 0..100 into a value that ranges from the 
		/// left edge (0) to the right edge (width) of the screen.</para>
		/// <para>
		/// Numbers outside the range are not clamped to 0 and 1, because out-of-range values 
		/// are often intentional and useful.</para> </summary>
		/// <param name="theValue"> The incoming value to be converted </param>
		/// <param name="theMinSrc"> Lower bound of the value's current range </param>
		/// <param name="theMaxSrc"> Upper bound of the value's current range </param>
		/// <param name="theMinDst"> Lower bound of the value's target range </param>
		/// <param name="theMaxDst"> Upper bound of the value's target range
		/// @return </param>
		public static float Map(float theValue, float theMinSrc, float theMaxSrc, float theMinDst, float theMaxDst)
		{
			return Blend(theMinDst, theMaxDst, Norm(theValue, theMinSrc, theMaxSrc));
		}

		/// <summary>
		/// For values of x between min and max, returns a smoothly varying value 
		/// that ranges from 0 at x = min to 1 at x = max. x is clamped to the 
		/// range [min, max] and then the interpolation formula is evaluated:
		/// -2*((x-min)/(max-min))3 + 3*((x-min)/(max-min))2 </summary>
		/// <param name="theA"> </param>
		/// <param name="theB"> </param>
		/// <param name="theValue">
		/// @return </param>
		public static float SmoothStep(float theMin, float theMax, float theValue)
		{
			if (theValue <= theMin)
			{
				return 0;
			}
			if (theValue >= theMax)
			{
				return 1;
			}

			return 3 * Pow((theValue - theMin) / (theMax - theMin), 2) - 2 * Pow((theValue - theMin) / (theMax - theMin), 3);
		}

		/// <summary>
		/// Constrains a value to not exceed a maximum and minimum value. </summary>
		/// <param name="theValue"> the value to constrain </param>
		/// <param name="theMin"> minimum limit </param>
		/// <param name="theMax"> maximum limit </param>
		/// <returns> the constrained value </returns>
		public static float Constrain(float theValue, float theMin, float theMax)
		{
			return (theValue < theMin) ? theMin : ((theValue > theMax) ? theMax : theValue);
		}

		/// <summary>
		/// Constrains the given value to be between 0 and 1 </summary>
		/// <param name="theValue"> the value to constrain </param>
		/// <returns> the saturated value </returns>
		public static float Saturate(float theValue)
		{
			return Constrain(theValue, 0, 1);
		}

		public static int Max(int a, int b)
		{
			return (a > b) ? a : b;
		}

		public static int Max(params int[] theValues)
		{
			int result = int.MinValue;
			foreach (int myValue in theValues)
			{
				result = Max(result,myValue);
			}
			return result;
		}

		private static int UnsignedByteToInt(sbyte b)
		{
			return (int) b & 0xFF;
		}

		public static sbyte Max(sbyte a, sbyte b)
		{
			return (UnsignedByteToInt(a) > UnsignedByteToInt(b)) ? a : b;
		}

		public static int Max(int a, int b, int c)
		{
			return (a > b) ? ((a > c) ? a : c) : ((b > c) ? b : c);
		}

		public static int Min(int a, int b)
		{
			return (a < b) ? a : b;
		}

		public static sbyte Min(sbyte a, sbyte b)
		{
			return (UnsignedByteToInt(a) < UnsignedByteToInt(b)) ? a : b;
		}

		public static int Min(params int[] theValues)
		{
			int result = int.MaxValue;
			foreach (int myValue in theValues)
			{
				result = Min(result,myValue);
			}
			return result;
		}

		public static int Min(int a, int b, int c)
		{
			return (a < b) ? ((a < c) ? a : c) : ((b < c) ? b : c);
		}

		public static float Sin(float angle)
		{
			return Mathf.Sin((float)angle);
		}

		public static float Cos(float angle)
		{
			return Mathf.Cos((float)angle);
		}

		public static float Tan(float angle)
		{
			return Mathf.Tan(angle);
		}

		public static float Asin(float value)
		{
			return Mathf.Asin(value);
		}

		public static float Acos(float value)
		{
			return Mathf.Acos(value);
		}

		public static float Atan(float value)
		{
			return Mathf.Atan(value);
		}

		public static float Atan2(float a, float b)
		{
			return (float) Math.Atan2(a, b);
		}

		public static float Degrees(float radians)
		{
			return radians * (float)RAD_TO_DEG;
		}

		public static float Radians(float degrees)
		{
			return degrees * (float)DEG_TO_RAD;
		}

		public static int Ceil(float theValue)
		{
			return (int)Math.Ceiling(theValue);
		}

		public static int Floor(float theValue)
		{
			return (int)Math.Floor(theValue);
		}

		public static int Round(float theValue)
		{
			return (int) Math.Round(theValue);
		}

		public static float FloorMod(float theA, float theB)
		{
			return (theA % theB + theB) % theB;
		}

		public static float Frac(float theValue)
		{
			return (theValue - Floor(theValue));
		}

		/// <summary>
		/// Round a float value to a specified number of decimal places. </summary>
		/// <param name="val"> the value to be rounded. </param>
		/// <param name="places"> the number of decimal places to round to. </param>
		/// <returns> val rounded to places decimal places. </returns>
		public static float Round(float val, int places)
		{
			long factor = (long) Math.Pow(10, places);

			// Shift the decimal the correct number of places
			// to the right.
			val = val * factor;

			// Round to the nearest integer.
			long tmp = (long)Math.Round(val);

			// Shift the decimal the correct number of places
			// back to the left.
			return (float) tmp / factor;
		}

		public static float Mag(float a, float b)
		{
			return (float) Math.Sqrt(a * a + b * b);
		}

		public static float Mag(float a, float b, float c)
		{
			return (float) Math.Sqrt(a * a + b * b + c * c);
		}

		public static float Dist(float x1, float y1, float x2, float y2)
		{
			return Sqrt(Sq(x2 - x1) + Sq(y2 - y1));
		}

		public static float Dist(float x1, float y1, float z1, float x2, float y2, float z2)
		{
			return Sqrt(Sq(x2 - x1) + Sq(y2 - y1) + Sq(z2 - z1));
		}

		/// <summary>
		/// Returns the sign of the given number so 1 if the value
		/// is bigger or equal than zero otherwise -1; </summary>
		/// <param name="theValue"> value to check for the sign </param>
		/// <returns> sign of the given value </returns>
		public static int Sign(float theValue)
		{
			if (theValue < 0)
			{
				return -1;
			}
			return 1;
		}
        
		public static bool SameSign(float theValue1, float theValue2)
		{
			return Sign(theValue1) == Sign(theValue2);
		}

		public static int LeastCommonMultiple(int theA, int theB)
		{
			int ggtZahl1 = theA;
			int ggtZahl2 = theB;

			while (ggtZahl2 != 0)
			{ // Berechnung des ggT
				int temp = ggtZahl1;
				ggtZahl1 = ggtZahl2;
				ggtZahl2 = temp % ggtZahl1;
			}
			// Berechnung und Rueckgabe des kgVs
			return Math.Abs(theA / ggtZahl1 * theB);
		}

		public static int LeastCommonMultiple(params int[] theValues)
		{
			int result = 1;
			foreach (int myValue in theValues)
			{
				result = LeastCommonMultiple(result,myValue);
			}
			return result;
		}

		/// <param name="theLastX"> </param>
		/// <param name="theF"> </param>
		/// <param name="theF2"> </param>
		/// <param name="theF3"> </param>
		/// <param name="theT">
		/// @return </param>
		public static float BezierBlend(float a, float b, float c, float d, float t)
		{
			float t1 = 1.0f - t;
			return a * t1 * t1 * t1 + 3 * b * t * t1 * t1 + 3 * c * t * t * t1 + d * t * t * t;
		}

		/// <summary>
		/// Interpolate a spline between at least 4 control points following the Catmull-Rom equation.
		/// here is the interpolation matrix
		/// m = [ 0.0  1.0  0.0   0.0 ]
		///     [-T    0.0  T     0.0 ]
		///     [ 2T   T-3  3-2T  -T  ]
		///     [-T    2-T  T-2   T   ]
		/// where T is the curve tension
		/// the result is a value between p1 and p2, t=0 for p1, t=1 for p2 </summary>
		/// <param name="theU"> value from 0 to 1 </param>
		/// <param name="theT"> The tension of the curve </param>
		/// <param name="theP0"> control point 0 </param>
		/// <param name="theP1"> control point 1 </param>
		/// <param name="theP2"> control point 2 </param>
		/// <param name="theP3"> control point 3 </param>
		/// <returns> catmull-Rom interpolation </returns>
		public static float CatmullRomBlend(float theP0, float theP1, float theP2, float theP3, float theU, float theT)
		{
			float c1, c2, c3, c4;
			c1 = theP1;
			c2 = -1.0f * theT * theP0 + theT * theP2;
			c3 = 2 * theT * theP0 + (theT - 3) * theP1 + (3 - 2 * theT) * theP2 + -theT * theP3;
			c4 = -theT * theP0 + (2 - theT) * theP1 + (theT - 2) * theP2 + theT * theP3;

			return (float)(((c4 * theU + c3) * theU + c2) * theU + c1);
		}

		public static float CubicBlend(float theV0, float theV1, float theV2, float theV3, float theBlend)
		{
			float mu2 = theBlend * theBlend;
			float a0 = theV3 - theV2 - theV0 + theV1;
			float a1 = theV0 - theV1 - a0;
			float a2 = theV2 - theV0;
			float a3 = theV1;

			return (a0 * theBlend * mu2 + a1 * mu2 + a2 * theBlend + a3);
		}

		public static float SmoothCubicBlend(float theV0, float theV1, float theV2, float theV3, float theBlend)
		{
			float mu2 = theBlend * theBlend;
			float a0 = -0.5f * theV0 + 1.5f * theV1 - 1.5f * theV2 + 0.5f * theV3;
			float a1 = theV0 - 2.5f * theV1 + 2 * theV2 - 0.5f * theV3;
			float a2 = -0.5f * theV0 + 0.5f * theV2;
			float a3 = theV1;

			return (a0 * theBlend * mu2 + a1 * mu2 + a2 * theBlend + a3);
		}

		/// <summary>
		/// Hermite interpolation like cubic requires 4 points so that it can achieve a higher degree of continuity. 
		/// In addition it has nice tension and biasing controls. Tension can be used to tighten up the curvature at 
		/// the known points. The bias is used to twist the curve about the known points. </summary>
		/// <param name="theV0"> </param>
		/// <param name="theV1"> </param>
		/// <param name="theV2"> </param>
		/// <param name="theV3"> </param>
		/// <param name="theBlend"> </param>
		/// <param name="tension"> 1 is high, 0 normal, -1 is low </param>
		/// <param name="bias"> 0 is even, positive is towards first segment, negative towards the other
		/// @return </param>
		public static float HermiteBlend(float theV0, float theV1, float theV2, float theV3, float theBlend, float tension, float bias)
		{

			float mu2 = theBlend * theBlend;
			float mu3 = mu2 * theBlend;
			float m0 = (theV1 - theV0) * (1 + bias) * (1 - tension) / 2;
			m0 += (theV2 - theV1) * (1 - bias) * (1 - tension) / 2;
			float m1 = (theV2 - theV1) * (1 + bias) * (1 - tension) / 2;
			m1 += (theV3 - theV2) * (1 - bias) * (1 - tension) / 2;

			float a0 = 2 * mu3 - 3 * mu2 + 1;
			float a1 = mu3 - 2 * mu2 + theBlend;
			float a2 = mu3 - mu2;
			float a3 = -2 * mu3 + 3 * mu2;

			return (a0 * theV1 + a1 * m0 + a2 * m1 + a3 * theV2);
		}

		public static float HermiteBlend(float theV0, float theV1, float theV2, float theV3, float theBlend)
		{
			return HermiteBlend(theV0, theV1, theV2, theV3, theBlend, 0, 0);
		}

		//////////////////////////////////////////////////////////////////
		//
		// function to shape value ranges between 0 and 1 or do easing
		//
		//////////////////////////////////////////////////////////////////

		/// <summary>
		/// Use this function to shape a linear increase from 0 to 1 to a curved one.
		/// Higher exponents result in steeper curves. </summary>
		/// <param name="theValue"> the value to shape </param>
		/// <param name="theExponent"> the exponent for shaping the output </param>
		/// <returns> the shaped value </returns>
		public static float ShapeExponential(float theValue, float theExponent)
		{
			return ShapeExponential(theValue, 0.5f, theExponent);
		}

		/// <summary>
		/// Use this function to shape a linear increase from 0 to 1 to a curved one.
		/// Higher exponents result in steeper curves. </summary>
		/// <param name="theValue"> the value to shape </param>
		/// <param name="theExponent"> the exponent for shaping the output </param>
		/// <returns> the shaped value </returns>
		public static float ShapeExponential(float theValue, float theBreakPoint, float theExponent)
		{
			if (theValue < 0)
			{
				return 0;
			}
			if (theValue > 1)
			{
				return 1;
			}

			if (theValue < 0.5)
			{
				return theBreakPoint * CCMath.Pow(2 * theValue,theExponent);
			}

			return (1 - (1 - theBreakPoint) * CCMath.Pow(2 * (1 - theValue),theExponent));
		}

		/// <summary>
		/// Use this method to average a value. This is useful if you want to buffer
		/// value changes. The smaller the factor is the slower the value reacts to 
		/// changes. </summary>
		/// <param name="theOldValue"> value you had so far </param>
		/// <param name="theNewValue"> new value </param>
		/// <param name="theFactor"> influence of the new value to the average value </param>
		/// <returns> averaged value based on the two given values and the factor </returns>
		public static float BufferedAverage(float theOldValue, float theNewValue, float theFactor)
		{
			return theOldValue * (1f - theFactor) + theNewValue * theFactor;
		}

		/// <summary>
		/// Use this function to get the average of all given values. </summary>
		/// <param name="theValues"> values to average </param>
		/// <returns> average of the given values </returns>
		public static float average(params float[] theValues)
		{
			float theSum = 0;
			foreach (float myValue in theValues)
			{
				theSum += myValue;
			}
			return theSum / theValues.Length;
		}

		/// <summary>
		/// Use this function to get the average of all given values. </summary>
		/// <param name="theValues"> values to average </param>
		/// <returns> average of the given values </returns>
		public static float Average(params float[] theValues)
		{
			float theSum = 0;
			foreach (float myValue in theValues)
			{
				theSum += myValue;
			}
			return theSum / theValues.Length;
		}

		/// <summary>
		/// compute the maximum number of digits
		/// </summary>
		public static int CountDigits(float theValue)
		{
			int myDigits = 1;
			int myTemp = 10;

			while (true)
			{
				if (theValue >= myTemp)
				{
					myDigits++;
					myTemp *= 10;
				}
				else
				{
					break;
				}
			}
			return myDigits;
		}

		/// <summary>
		/// Checks if the given value is between the two given borders </summary>
		/// <param name="theValue"> the value to check </param>
		/// <param name="theBorder1"> the first border </param>
		/// <param name="theBorder2"> the second border </param>
		/// <returns> <code>true</code> if the value is between the two given border values other wise <code>false</code> </returns>
		public static bool IsInBetween(float theValue, float theBorder1, float theBorder2)
		{
			return theValue > theBorder1 && theValue < theBorder2 || theValue < theBorder1 && theValue > theBorder2;
		}

		public static bool IsNaN(params float[] theValues)
		{
			foreach (float myValue in theValues)
			{
				if (float.IsNaN(myValue))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Sqrt(a^2 + b^2) without under/overflow. * </summary>

	   public static float Hypot(float a, float b)
	   {
		  float r;
		  if (Math.Abs(a) > Math.Abs(b))
		  {
			 r = b / a;
			 r = (float)Math.Abs(a) * (float)Math.Sqrt(1 + r * r);
		  }
		  else if (b != 0)
		  {
			 r = a / b;
			 r = (float)Math.Abs(b) * (float)Math.Sqrt(1 + r * r);
		  }
		  else
		  {
			 r = 0.0f;
		  }
		  return r;
	   }
	}

}