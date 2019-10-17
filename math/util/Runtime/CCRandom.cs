using System;

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
	/// An instance of this class is used to generate a stream of 
	/// pseudorandom numbers. The class uses a 48-bit seed, which is 
	/// modified using a linear congruential formula.
	/// 
	/// If two instances of <code>CCRandom</code> are created with the same 
	/// seed, and the same sequence of method calls is made for each, they 
	/// will generate and return identical sequences of numbers. In order to 
	/// guarantee this property, particular algorithms are specified for the 
	/// class <tt>Random</tt>. Java implementations must use all the algorithms 
	/// shown here for the class <tt>Random</tt>, for the sake of absolute 
	/// portability of Java code. However, subclasses of class <tt>Random</tt> 
	/// are permitted to use other algorithms, so long as they adhere to the 
	/// general contracts for all the methods.
	/// <para>
	/// The algorithms implemented by class <tt>Random</tt> use a 
	/// <tt>protected</tt> utility method that on each invocation can supply 
	/// up to 32 pseudorandomly generated bits.
	/// </para>
	/// <para>
	/// Many applications will find the <code>random</code> method in 
	/// class <code>Math</code> simpler to use.
	/// 
	/// @author  Frank Yellin
	/// @version 1.43, 01/12/04
	/// </para>
	/// </summary>
	/// <seealso cref=     java.lang.Math#random()
	/// @since   JDK1.0
	/// @author texone
	///  </seealso>
	public class CCRandom
    {

        Random _myRandom;
        /// 
        private const long serialVersionUID = 4711320491779209820L;

		public CCRandom() 
		{
            _myRandom = new Random();

        }
        
		public CCRandom(int theSeed) 
		{
            _myRandom = new Random(theSeed);

        }

		/// <summary>
		/// @shortdesc Generates random numbers. 
		/// Generates random numbers. Each time the random() function is called, it returns an 
		/// unexpected value within the specified range. If one parameter is passed to the function 
		/// it will return a float between zero and the value of the high parameter. The function 
		/// call random(5) returns values between 0 and 5. If two parameters are passed, 
		/// it will return a float with a value between the the parameters. The function call 
		/// random(-5, 10.2) returns values between -5 and 10.2. </summary>
		/// <param name="howsmall"> </param>
		/// <param name="howbig"> </param>
		/// <returns> random value </returns>
		public virtual float Random()
		{
			return _myRandom.Next();
		}

		/// 
		/// <param name="theMax"> </param>
		public virtual float Random(float theMax)
		{
			if (theMax == 0)
			{
				return 0;
			}
			return _myRandom.Next() * theMax;
		}

		/// 
		/// <param name="theMin"> minimum value for the random to generate </param>
		/// <param name="theMax"> maximum value for the random to generate </param>
		/// <returns> random value </returns>
		public virtual float Random(float theMin, float theMax)
		{
			if (theMin >= theMax)
			{
				float tmp = theMin;
				theMin = theMax;
				theMax = tmp;
			}

			float diff = theMax - theMin;
			return Random(diff) + theMin;
        }

        
        private float nextNextGaussian;
        private bool haveNextNextGaussian = false;

        /// <summary>
        /// @shortdesc Returns the next pseudorandom, Gaussian ("normally") distributed float value
        /// Returns the next pseudorandom, Gaussian ("normally") distributed float value with mean 
        /// 0.0 and standard deviation 1.0 from this random number generator's sequence. The general 
        /// contract of nextGaussian is that one float value, chosen from (approximately) the usual 
        /// normal distribution with mean 0.0 and standard deviation 1.0, is pseudo randomly generated 
        /// and returned. 
        /// 
        /// This method ensures that the result is mapped between 0 and 1. </summary>
        /// <returns> gaussian random </returns>
        public virtual float GaussianRandom()
		{
            float myGaussian;
            // See Knuth, ACP, Section 3.4.1 Algorithm C.
            if (haveNextNextGaussian)
            {
                haveNextNextGaussian = false;
                myGaussian = nextNextGaussian;
            }
            else
            {
                float v1, v2, s;
                do
                {
                    v1 = 2 * _myRandom.Next() - 1; // between -1 and 1
                    v2 = 2 * _myRandom.Next() - 1; // between -1 and 1
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1 || s == 0);
                float multiplier = CCMath.Sqrt(-2 * CCMath.Log(s) / s);
                nextNextGaussian = v2 * multiplier;
                haveNextNextGaussian = true;
                myGaussian = v1 * multiplier;
            }
            return (CCMath.Constrain(myGaussian / 4,-1,1) + 1) / 2;
		}

		/// 
		/// <param name="theMax">
		/// @return </param>
		public virtual float GaussianRandom(float theMax)
		{
			  return GaussianRandom() * theMax;
		}

		/// 
		/// <param name="theMin"> minimum value for the random to generate </param>
		/// <param name="theMax"> maximum value for the random to generate
		/// @return </param>
		public virtual float GaussianRandom(float theMin, float theMax)
		{
			  return GaussianRandom() * (theMax - theMin) + theMin;
		}

		/// <summary>
		/// Sets the seed of this random number generator using a single long seed. By default, random() produces different results 
		/// each time the program is run. Set the value parameter to a constant to return the 
		/// same pseudo-random numbers each time the software is run.
		/// 
		///  The general contract of setSeed is that it alters the state of this random number 
		///  generator object so as to be in exactly the same state as if it had just been created 
		///  with the argument seed as a seed.The implementation of setSeed by class Random happens 
		///  to use only 48 bits of the given seed. In general, however, an overriding method may 
		///  use all 64 bits of the long argument as a seed value. 
		///  Note: Although the seed value is an AtomicLong, this method must still be 
		///  synchronized to ensure correct semantics of haveNextNextGaussian. </summary>
		/// <param name="what"> </param>
		public void RandomSeed(int what)
        {
            _myRandom = new Random(what);
        }
	}

}