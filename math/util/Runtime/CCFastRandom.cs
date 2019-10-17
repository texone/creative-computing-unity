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
	/// @author christianriekoff
	/// 
	/// </summary>
	public class CCFastRandom
	{
		// seed this if one wishes
		private int _mySeed;

		public CCFastRandom(int theSeed)
		{
			_mySeed = theSeed;
		}

		public CCFastRandom() : this(0)
		{
		}

		/// <summary>
		/// Returns a pseudo-random number.
		/// </summary>
		private int RandomImp()
		{
			// this makes a 'nod' to being potentially called from multiple threads
			int seed = _mySeed;

			seed *= 1103515245;
			seed += 12345;
			_mySeed = seed;

			// NOTE: hi bits have better properties
			return seed;
		}

		/// <summary>
		/// Returns a random number on [0, range)
        /// <param name="range"><param>
		/// </summary>
		public int Random(int range)
		{
			return (int)((uint)(((int)((uint)RandomImp() >> 15)) * range) >> 17);
		}

        /// <summary>
		/// Returns a random number on [theMin, theMax)
        /// <param name="theMin"><param>
        /// <param name="theMax"><param>
		/// </summary>
        public int Random(int theMin, int theMax)
        {
            return theMin + Random(theMax - theMin);
        }

        public bool nextBoolean()
		{
			// hi-bit is the most random
			return RandomImp() > 0;
		}

		public float NextFloat()
		{
			return ((int)((uint)RandomImp() >> 8)) * (1.0f / (1 << 24));
		}


		/// <summary>
		/// True if the next nextGaussian is available.  This is used by
		/// nextGaussian, which generates two gaussian numbers by one call,
		/// and returns the second on the second call.
		/// 
		/// @serial whether nextNextGaussian is available </summary>
		/// <seealso cref= #nextGaussian() </seealso>
		/// <seealso cref= #nextNextGaussian </seealso>
		private bool haveNextNextGaussian;

		/// <summary>
		/// The next nextGaussian, when available.  This is used by nextGaussian,
		/// which generates two gaussian numbers by one call, and returns the
		/// second on the second call.
		/// 
		/// @serial the second gaussian of a pair </summary>
		/// <seealso cref= #nextGaussian() </seealso>
		/// <seealso cref= #haveNextNextGaussian </seealso>
		private float nextNextGaussian;

		/// <summary>
		/// Generates the next pseudorandom, Gaussian (normally) distributed
		/// double value, with mean 0.0 and standard deviation 1.0.
		/// <para>This is described in section 3.4.1 of <em>The Art of Computer
		/// Programming, Volume 2</em> by Donald Knuth.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the next pseudorandom Gaussian distributed double </returns>
		public virtual float NextGaussian()
		{
			lock (this)
			{
				if (haveNextNextGaussian)
				{
					haveNextNextGaussian = false;
					return nextNextGaussian;
				}
				float v1, v2, s;
				do
				{
					v1 = 2 * NextFloat() - 1; // Between -1.0 and 1.0.
					v2 = 2 * NextFloat() - 1; // Between -1.0 and 1.0.
					s = v1 * v1 + v2 * v2;
				} while (s >= 1);
				float norm = (float)CCMath.Sqrt(-2 * CCMath.Log(s) / s);
				nextNextGaussian = v2 * norm;
				haveNextNextGaussian = true;
				return v1 * norm;
			}
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
			return NextFloat();
		}

		/// 
		/// <param name="theMax"> </param>
		public virtual float Random(float theMax)
		{
			if (theMax == 0)
			{
				return 0;
			}
			return NextFloat() * theMax;
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

		/// <summary>
		/// @shortdesc Returns the next pseudorandom, Gaussian ("normally") distributed double value
		/// Returns the next pseudorandom, Gaussian ("normally") distributed double value with mean 
		/// 0.0 and standard deviation 1.0 from this random number generator's sequence. The general 
		/// contract of nextGaussian is that one double value, chosen from (approximately) the usual 
		/// normal distribution with mean 0.0 and standard deviation 1.0, is pseudo randomly generated 
		/// and returned. 
		/// 
		/// This method ensures that the result is mapped between 0 and 1. </summary>
		/// <returns> gaussian random </returns>
		public virtual float GaussianRandom()
		{
			return (CCMath.Constrain(NextGaussian() / 4f,-1,1) + 1) / 2;
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
		public virtual void RandomSeed(int what)
		{
			_mySeed = what;
			Random();
		}
	}

}