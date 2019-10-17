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
namespace cc.creativecomputing.math.signal
{

	[Serializable]
	public class CCSimplexNoise : CCSignal
	{

		private static readonly float SQRT3 = Mathf.Sqrt(3.0f);
		private static readonly float SQRT5 = Mathf.Sqrt(5.0f);

		/// <summary>
		/// Skewing and unskewing factors for 2D, 3D and 4D, some of them
		/// pre-multiplied.
		/// </summary>
		private static readonly float G2 = (3.0f - SQRT3) / 6.0f;
		private static readonly float G4 = (5.0f - SQRT5) / 20.0f;

		/// <summary>
		/// Permutation table
		/// </summary>
		private static readonly int[] p = new int[] {151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180};

		/// <summary>
		/// To remove the need for index wrapping, float the permutation table
		/// length
		/// </summary>
		private static int[] perm = new int[0x200];
		static CCSimplexNoise()
		{
				for (int i = 0; i < 0x200; i++)
				{
						perm[i] = p[i & 0xff];
				}
		}



		// A lookup table to traverse the simplex around a given point in 4D.
				// Details can be found where this table is used, in the 4D noise method.

		private static int[][] simplex = new int[][]
		{
			new int[] {0, 1, 2, 3},
			new int[] {0, 1, 3, 2},
			new int[] {0, 0, 0, 0},
			new int[] {0, 2, 3, 1},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {1, 2, 3, 0},
			new int[] {0, 2, 1, 3},
			new int[] {0, 0, 0, 0},
			new int[] {0, 3, 1, 2},
			new int[] {0, 3, 2, 1},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {1, 3, 2, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {1, 2, 0, 3},
			new int[] {0, 0, 0, 0},
			new int[] {1, 3, 0, 2},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {2, 3, 0, 1},
			new int[] {2, 3, 1, 0},
			new int[] {1, 0, 2, 3},
			new int[] {1, 0, 3, 2},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {2, 0, 3, 1},
			new int[] {0, 0, 0, 0},
			new int[] {2, 1, 3, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {2, 0, 1, 3},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {3, 0, 1, 2},
			new int[] {3, 0, 2, 1},
			new int[] {0, 0, 0, 0},
			new int[] {3, 1, 2, 0},
			new int[] {2, 1, 0, 3},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {0, 0, 0, 0},
			new int[] {3, 1, 0, 2},
			new int[] {0, 0, 0, 0},
			new int[] {3, 2, 0, 1},
			new int[] {3, 2, 1, 0}
		};

		// Helper function to compute gradients.
		private static float grad1(int theHash)
		{
			int h = theHash & 15;
			float gx = 1 + (h & 7); // Gradient value is one of 1.0, 2.0, ..., 8.0
			if ((h & 8) == 1)
			{
				gx = -gx; // Make half of the gradients negative
			}
			return gx;
		}

		// 1D simplex noise with derivative.
		// If the last argument is not null, the analytic derivative is also calculated.
		public override float[] NoiseImpl(float x)
		{
			int i0 = (int)Mathf.Floor(x); // Standard Floor flushes the processor pipeline.
			int i1 = i0 + 1;
			float x0 = x - i0;

			if (x0 == 0)
			{
				x0 += Mathf.Epsilon; // Otherwise it's always zero at integer x!
			}
			float x1 = x0 - 1;
			float x20 = x0 * x0;
			float t0 = 1 - x20;
			// if(t0<0) t0=0; // Never happens for 1D: x0<=1 always
			float t20 = t0 * t0;
			float t40 = t20 * t20;
			float gx0 = grad1(p[i0 & 0xFF]);
			float Noise = t40 * gx0 * x0;
			float x21 = x1 * x1;
			float t1 = 1 - x21;
			// if(t1<0) t1=0; // Never happens for 1D: |x1|<=1 always
			float t21 = t1 * t1;
			float t41 = t21 * t21;
			float gx1 = grad1(perm[i1 & 0xFF]);
			Noise += t41 * gx1 * x1;

			float dx = t20 * t0 * gx0 * x20; // *dx =-8 * t20 * t0 * x0 * (gx0 * x0) + t40 * gx0;
			dx += t21 * t1 * gx1 * x21; // *dx+=-8 * t21 * t1 * x1 * (gx1 * x1) + t41 * gx1;
			dx *= -8;
			dx += t40 * gx0 + t41 * gx1;
			dx *= 0.407461f; // Scale derivative to match the noise scaling:

			float myNoise = Mathf.Clamp(Noise * 0.407461f + 0.030914f, -1.0f, 1.0f);

			return new float[] {normed ? (myNoise + 1) / 2 : myNoise, normed ? (dx + 1) / 2 : dx};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.random.CCNoise#noiseImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			float[] myValues = NoiseImpl(theX);
			float[] myResult = new float[myValues.Length];
			for (int i = 0; i < myResult.Length;i++)
			{
				myResult[i] = (float)myValues[i];
			}
			return myResult;
		}

		/// <summary>
		/// Gradient tables. These could be programmed the Ken Perlin way 
		/// with some clever bit-twiddling, but this is clearer.
		/// </summary>
		private static float[][] grad2lut = new float[][]
		{
			new float[] {-1, -1},
			new float[] {1, 0},
			new float[] {-1, 0},
			new float[] {1, 1},
			new float[] {-1, 1},
			new float[] {0, -1},
			new float[] {0, 1},
			new float[] {1, -1}
		};

		/// <summary>
		/// Helper function to compute gradients and gradients-dot-residualvectors. </summary>
		/// <param name="theHash">
		/// @return </param>
		private float[] grad2(int theHash)
		{
			int h = theHash & 7;
			return new float[] {grad2lut[h][0], grad2lut[h][1]};
		}

		public override float[] SignalImpl(float x, float y)
		/// <summary>
		/// 2D simplex noise with derivatives.the analytic derivative (the 2D gradient of the scalar noise field) is
		/// also calculated. </summary>
		/// <param name="x"> </param>
		/// <param name="y">
		/// @return </param>
		{
			// Skew the input space to determine the current simplex cell:
			float s = (x + y) * 0.366025403f; // (sqrt(3)-1)/2
			float xs = x + s;
			float ys = y + s;
			int i = (int)Mathf.Floor(xs); // Standard Floor flushes the processor pipeline.
			int j = (int)Mathf.Floor(ys);

			float t = (float)(i + j) * G2;
			float X0 = i - t; // Unskew the cell origin back to (x,y) space
			float Y0 = j - t;
			float x0 = x - X0; // The x,y distances from the cell origin
			float y0 = y - Y0;
			// For the 2D case, the simplex shape is an equilateral triangle. Determine which simplex we are in.
			int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
			if (x0 > y0)
			{
				i1 = 1;
				j1 = 0;
			} // lower triangle, XY order: (0,0)->(1,0)->(1,1)
			else
			{
				i1 = 0;
				j1 = 1;
			} // upper triangle, YX order: (0,0)->(0,1)->(1,1)
			// A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
			// a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where c=(3-sqrt(3))/6
			float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
			float y1 = y0 - j1 + G2;
			float x2 = x0 - 1 + 2 * G2; // Offsets for last corner in (x,y) unskewed coords
			float y2 = y0 - 1 + 2 * G2;
			// Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
			int ii = i & 0xFF;
			int jj = j & 0xFF;
			// Calculate the contribution from the three corners
			float n0, n1, n2; // Noise contributions from the three simplex corners
			float gx0 = 0, gy0 = 0, gx1 = 0, gy1 = 0, gx2 = 0, gy2 = 0; // Gradients at simplex corners
			float t0 = 0.5f - x0 * x0 - y0 * y0;
			float t20, t40;
			if (t0 < 0)
			{
				t40 = t20 = t0 = n0 = gx0 = gy0 = 0; // No influence
			}
			else
			{
				float[] grads = grad2(perm[ii + perm[jj]]);
				gx0 = grads[0];
				gy0 = grads[1];
				t20 = t0 * t0;
				t40 = t20 * t20;
				n0 = t40 * (gx0 * x0 + gy0 * y0);
			}
			float t1 = 0.5f - x1 * x1 - y1 * y1;
			float t21, t41;
			if (t1 < 0)
			{
				t21 = t41 = t1 = n1 = gx1 = gy1 = 0; // No influence
			}
			else
			{
				float[] grads = grad2(perm[ii + i1 + perm[jj + j1]]);
				gx1 = grads[0];
				gy1 = grads[1];
				t21 = t1 * t1;
				t41 = t21 * t21;
				n1 = t41 * (gx1 * x1 + gy1 * y1);
			}
			float t2 = 0.5f - x2 * x2 - y2 * y2;
			float t22, t42;
			if (t2 < 0)
			{
				t42 = t22 = t2 = n2 = gx2 = gy2 = 0; // No influence
			}
			else
			{
				float[] grads = grad2(perm[ii + 1 + perm[jj + 1]]);
				gx2 = grads[0];
				gy2 = grads[1];
				t22 = t2 * t2;
				t42 = t22 * t22;
				n2 = t42 * (gx2 * x2 + gy2 * y2);
			}
			float Noise = n0 + n1 + n2; // Add contributions from each corner to get the final noise value.

			/*
			 * Compute derivative: dx =-8 * t20 * t0 * x0 * (gx0 * x0 + gy0 * y0) + t40 * gx0; dy =-8 * t20 * t0 * y0 * (gx0
			 * * x0 + gy0 * y0) + t40 * gy0; dx+=-8 * t21 * t1 * x1 * (gx1 * x1 + gy1 * y1) + t41 * gx1; dy+=-8 * t21 * t1 *
			 * y1 * (gx1 * x1 + gy1 * y1) + t41 * gy1; dx+=-8 * t22 * t2 * x2 * (gx2 * x2 + gy2 * y2) + t42 * gx2; dy+=-8 *
			 * t22 * t2 * y2 * (gx2 * x2 + gy2 * y2) + t42 * gy2; Optimises to:
			 */
			float temp0 = t20 * t0 * (gx0 * x0 + gy0 * y0);
			float dx = temp0 * x0;
			float dy = temp0 * y0;
			float temp1 = t21 * t1 * (gx1 * x1 + gy1 * y1);
			dx += temp1 * x1;
			dy += temp1 * y1;
			float temp2 = t22 * t2 * (gx2 * x2 + gy2 * y2);
			dx += temp2 * x2;
			dy += temp2 * y2;
			dx *= -8;
			dy *= -8;
			dx += t40 * gx0 + t41 * gx1 + t42 * gx2;
			dy += t40 * gy0 + t41 * gy1 + t42 * gy2;
			dx *= 70.1605f; // Scale derivative to match the noise scaling
			dy *= 70.1605f;

            float myNoise = Mathf.Clamp(Noise * 70.1605f - 0.000142584f, -1.0f, 1.0f);

            if (normed)
            {
                return new float[] {
                    (myNoise + 1) / 2,
                    (dx + 1) / 2,
                    (dy + 1) / 2
                };
            }
			return new float[] { myNoise, dx, dy };
		}

		/// <summary>
		/// Gradient directions for 3D.
		/// These vectors are based on the midpoints of the 12 edges of a cube.
		/// A larger array of random unit length vectors would also do the job, but these 12
		/// (including 4 repeats to make the array length a power of two) work better.
		/// They are not random, they are carefully chosen to represent a small, isotropic set of directions.
		/// </summary>
		internal static float[][] grad3lut = new float[][]
		{
			new float[] {1, 0, 1},
			new float[] {0, 1, 1},
			new float[] {-1, 0, 1},
			new float[] {0, -1, 1},
			new float[] {1, 0, -1},
			new float[] {0, 1, -1},
			new float[] {-1, 0, -1},
			new float[] {0, -1, -1},
			new float[] {1, -1, 0},
			new float[] {1, 1, 0},
			new float[] {-1, 1, 0},
			new float[] {-1, -1, 0},
			new float[] {1, 0, 1},
			new float[] {-1, 0, 1},
			new float[] {0, 1, -1},
			new float[] {0, -1, -1}
		};

		/// <summary>
		/// Helper function to compute gradients. </summary>
		/// <param name="theHash">
		/// @return </param>
		private float[] grad3(int theHash)
		{
			int h = theHash & 15;
			return new float[] {grad3lut[h][0], grad3lut[h][1], grad3lut[h][2]};
		}

		public override float[] SignalImpl(float x, float y, float z)
		/// <summary>
		/// 3D simplex noise with derivatives.the analytic derivative (the 3D gradient of the scalar noise field) is
		/// also calculated. </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="z">
		/// @return </param>
		{
			x = Mathf.Abs(x);
			y = Mathf.Abs(y);
			z = Mathf.Abs(z);
			// Skew the input space to determine which simplex cell we're in:
			float s = (x + y + z) / 3; // Simple 1/3 skew factor for 3D
			float xs = x + s;
			float ys = y + s;
			float zs = z + s;

			int i = (int)Mathf.Floor(xs); // Standard Floor flushes the processor pipeline.
			int j = (int)Mathf.Floor(ys);
			int k = (int)Mathf.Floor(zs);

			float t = (i + j + k) / 6.0f; // Simple 1/6 skew factor for 3D
			float X0 = i - t; // Unskew the cell origin back to (x,y,z) space:
			float Y0 = j - t;
			float Z0 = k - t;
			float x0 = x - X0; // The x,y,z distances from the cell origin
			float y0 = y - Y0;
			float z0 = z - Z0;
			// For the 3D case, the simplex shape is a slightly irregular tetrahedron. Determine which simplex we are in.
			int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
			int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
			if (x0 >= y0)
			{ // TODO: This code would benefit from a backport from the GLSL version!
				if (y0 >= z0)
				{
					i1 = 1;
					j1 = 0;
					k1 = 0;
					i2 = 1;
					j2 = 1;
					k2 = 0;
				} // X Y Z order
				else if (x0 >= z0)
				{
					i1 = 1;
					j1 = 0;
					k1 = 0;
					i2 = 1;
					j2 = 0;
					k2 = 1;
				} // X Z Y order
				else
				{
					i1 = 0;
					j1 = 0;
					k1 = 1;
					i2 = 1;
					j2 = 0;
					k2 = 1;
				} // Z X Y order
			}
			else
			{ // x0<y0
				if (y0 < z0)
				{
					i1 = 0;
					j1 = 0;
					k1 = 1;
					i2 = 0;
					j2 = 1;
					k2 = 1;
				} // Z Y X order
				else if (x0 < z0)
				{
					i1 = 0;
					j1 = 1;
					k1 = 0;
					i2 = 0;
					j2 = 1;
					k2 = 1;
				} // Y Z X order
				else
				{
					i1 = 0;
					j1 = 1;
					k1 = 0;
					i2 = 1;
					j2 = 1;
					k2 = 0;
				} // Y X Z order
			}
			/*
			 * A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z), a step of (0,1,0) in (i,j,k) means a
			 * step of (-c,1-c,-c) in (x,y,z), and a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z),
			 * where c=1/6.
			 */
			float x1 = x0 - i1 + 1 / 6.0f; // Offsets for second corner in (x,y,z) coords
			float y1 = y0 - j1 + 1 / 6.0f;
			float z1 = z0 - k1 + 1 / 6.0f;
			float x2 = x0 - i2 + 2 / 6.0f; // Offsets for third corner in (x,y,z) coords
			float y2 = y0 - j2 + 2 / 6.0f;
			float z2 = z0 - k2 + 2 / 6.0f;
			float x3 = x0 - 1 + 3 / 6.0f; // Offsets for last corner in (x,y,z) coords
			float y3 = y0 - 1 + 3 / 6.0f;
			float z3 = z0 - 1 + 3 / 6.0f;
			// Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
			int ii = i % 256;
			int jj = j % 256;
			int kk = k % 256;
			// Calculate the contribution from the four corners
			float gx0 = 0, gy0 = 0, gz0 = 0; // Gradients at simplex corners
			float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
			float t20, t40;
			float n0, n1, n2, n3; // Noise contributions from the four simplex corners
			if (t0 < 0)
			{
				n0 = t0 = t20 = t40 = gx0 = gy0 = gz0 = 0;
			}
			else
			{
				float[] grads = grad3(perm[ii + perm[jj + perm[kk]]]);
				gx0 = grads[0];
				gy0 = grads[1];
				gz0 = grads[2];
				t20 = t0 * t0;
				t40 = t20 * t20;
				n0 = t40 * (gx0 * x0 + gy0 * y0 + gz0 * z0);
			}
			float gx1 = 0, gy1 = 0, gz1 = 0; // Gradients at simplex corners
			float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
			float t21, t41;
			if (t1 < 0)
			{
				n1 = t1 = t21 = t41 = gx1 = gy1 = gz1 = 0;
			}
			else
			{
				float[] grads = grad3(perm[ii + i1 + perm[jj + j1 + perm[kk + k1]]]);
				gx1 = grads[0];
				gy1 = grads[1];
				gz1 = grads[2];
				t21 = t1 * t1;
				t41 = t21 * t21;
				n1 = t41 * (gx1 * x1 + gy1 * y1 + gz1 * z1);
			}
			float gx2 = 0, gy2 = 0, gz2 = 0; // Gradients at simplex corners
			float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
			float t22, t42;
			if (t2 < 0)
			{
				n2 = t2 = t22 = t42 = gx2 = gy2 = gz2 = 0;
			}
			else
			{
				float[] grads = grad3(perm[ii + i2 + perm[jj + j2 + perm[kk + k2]]]);
				gx2 = grads[0];
				gy2 = grads[1];
				gz2 = grads[2];
				t22 = t2 * t2;
				t42 = t22 * t22;
				n2 = t42 * (gx2 * x2 + gy2 * y2 + gz2 * z2);
			}
			float gx3 = 0, gy3 = 0, gz3 = 0; // Gradients at simplex corners
			float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
			float t23, t43;
			if (t3 < 0)
			{
				n3 = t3 = t23 = t43 = gx3 = gy3 = gz3 = 0;
			}
			else
			{
				float[] grads = grad3(perm[ii + 1 + perm[jj + 1 + perm[kk + 1]]]);
				gx3 = grads[0];
				gy3 = grads[1];
				gz3 = grads[2];
				t23 = t3 * t3;
				t43 = t23 * t23;
				n3 = t43 * (gx3 * x3 + gy3 * y3 + gz3 * z3);
			}
			float Noise = n0 + n1 + n2 + n3;
			float temp0 = t20 * t0 * (gx0 * x0 + gy0 * y0 + gz0 * z0);

			float dx = temp0 * x0;
			float dy = temp0 * y0;
			float dz = temp0 * z0;
			float temp1 = t21 * t1 * (gx1 * x1 + gy1 * y1 + gz1 * z1);
			dx += temp1 * x1;
			dy += temp1 * y1;
			dz += temp1 * z1;
			float temp2 = t22 * t2 * (gx2 * x2 + gy2 * y2 + gz2 * z2);
			dx += temp2 * x2;
			dy += temp2 * y2;
			dz += temp2 * z2;
			float temp3 = t23 * t3 * (gx3 * x3 + gy3 * y3 + gz3 * z3);
			dx += temp3 * x3;
			dy += temp3 * y3;
			dz += temp3 * z3;
			dx *= -8;
			dy *= -8;
			dz *= -8;
			dx += t40 * gx0 + t41 * gx1 + t42 * gx2 + t43 * gx3;
			dy += t40 * gy0 + t41 * gy1 + t42 * gy2 + t43 * gy3;
			dz += t40 * gz0 + t41 * gz1 + t42 * gz2 + t43 * gz3;
			dx *= 16.9446f; // Scale derivative to match the noise scaling:
			dy *= 16.9446f;
			dz *= 16.9446f;

            float myNoise = Mathf.Clamp(Noise * 32.741f + 0.00104006f, -1.0f, 1.0f);

            if (normed)
            {
                return new float[] {
                    (myNoise + 1) / 2,
                    (dx + 1) / 2,
                    (dy + 1) / 2,
                    (dz + 1) / 2
                };
            }

			return new float[] { myNoise, dx, dy, dz};
		}

		private static float[][] grad4lut = new float[][]
		{
			new float[] {0, 1, 1, 1},
			new float[] {0, 1, 1, -1},
			new float[] {0, 1, -1, 1},
			new float[] {0, 1, -1, -1},
			new float[] {0, -1, 1, 1},
			new float[] {0, -1, 1, -1},
			new float[] {0, -1, -1, 1},
			new float[] {0, -1, -1, -1},
			new float[] {1, 0, 1, 1},
			new float[] {1, 0, 1, -1},
			new float[] {1, 0, -1, 1},
			new float[] {1, 0, -1, -1},
			new float[] {-1, 0, 1, 1},
			new float[] {-1, 0, 1, -1},
			new float[] {-1, 0, -1, 1},
			new float[] {-1, 0, -1, -1},
			new float[] {1, 1, 0, 1},
			new float[] {1, 1, 0, -1},
			new float[] {1, -1, 0, 1},
			new float[] {1, -1, 0, -1},
			new float[] {-1, 1, 0, 1},
			new float[] {-1, 1, 0, -1},
			new float[] {-1, -1, 0, 1},
			new float[] {-1, -1, 0, -1},
			new float[] {1, 1, 1, 0},
			new float[] {1, 1, -1, 0},
			new float[] {1, -1, 1, 0},
			new float[] {1, -1, -1, 0},
			new float[] {-1, 1, 1, 0},
			new float[] {-1, 1, -1, 0},
			new float[] {-1, -1, 1, 0},
			new float[] {-1, -1, -1, 0}
		};

		/// <summary>
		/// Helper function to compute gradients and gradients-dot-residualvectors. </summary>
		/// <param name="theHash">
		/// @return </param>
		private float[] grad4(int theHash)
		{
			int h = theHash & 31;
			return new float[] {grad4lut[h][0], grad4lut[h][1], grad4lut[h][2], grad4lut[h][3]};
		}

		/// <summary>
		/// 4D simplex noise with derivatives. the analytic derivative (the 4D gradient of the scalar noise field) is
		/// also calculated. </summary>
		/// <param name="x"> </param>
		/// <param name="y"> </param>
		/// <param name="z"> </param>
		/// <param name="w">
		/// @return </param>
		public virtual float[] noiseImpl(float x, float y, float z, float w)
		{
			// Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
			float s = (x + y + z + w) * 0.30901699437494742410229341718282f; // (sqrt(5)-1)/4 Factor for 4D skewing
			float xs = x + s;
			float ys = y + s;
			float zs = z + s;
			float ws = w + s;
			int i = (int)Mathf.Floor(xs); // Standard Floor flushes the processor pipeline.
			int j = (int)Mathf.Floor(ys);
			int k = (int)Mathf.Floor(zs);
			int l = (int)Mathf.Floor(ws);

			float t = (i + j + k + l) * G4; // Factor for 4D unskewing
			float X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
			float Y0 = j - t;
			float Z0 = k - t;
			float W0 = l - t;
			float x0 = x - X0; // The x,y,z,w distances from the cell origin
			float y0 = y - Y0;
			float z0 = z - Z0;
			float w0 = w - W0;
			/*
			 * For the 4D case, the simplex is a tesseract (see 'Dimension' on Wikipedia
			 * http://en.wikipedia.org/wiki/Dimension). To find out which of the 24 possible simplices is current, determine
			 * the magnitude ordering of x0, y0, z0 and w0. The method below is a reasonable way of finding the ordering of
			 * x,y,z,w and then finding the correct traversal order for the simplex we're in. First, six pair-wise
			 * comparisons are performed between each possible pair of the four coordinates, and then the results are used
			 * to add up binary bits for an integer index into a precomputed lookup table, simplex[].
			 */
			int c1 = (x0 > y0) ? 32 : 0;
			int c2 = (x0 > z0) ? 16 : 0;
			int c3 = (y0 > z0) ? 8 : 0;
			int c4 = (x0 > w0) ? 4 : 0;
			int c5 = (y0 > w0) ? 2 : 0;
			int c6 = (z0 > w0) ? 1 : 0;
			int c = c1 & c2 & c3 & c4 & c5 & c6; /*
												 * '&' is mostly faster than '+' simplex[c] is a 4-vector with the numbers
												 * 0, 1, 2 and 3 in some order. Many values of c will never occur, since
												 * e.g. x>y>z>w makes x<z, y<w and x<w impossible. Only the 24 indices which
												 * have non-zero entries make any sense.
												 */

			/*
			 * We use a thresholding to set the coordinates in turn from the largest magnitude. The number 3 in the
			 * "simplex" array is at the position of the largest coordinate.
			 */
			int i1 = simplex[c][0] >= 3 ? 1 : 0; // The integer offsets for the second simplex corner
			int j1 = simplex[c][1] >= 3 ? 1 : 0;
			int k1 = simplex[c][2] >= 3 ? 1 : 0;
			int l1 = simplex[c][3] >= 3 ? 1 : 0;
			// The number 2 in the "simplex" array is at the second largest coordinate.
			int i2 = simplex[c][0] >= 2 ? 1 : 0; // The integer offsets for the third simplex corner
			int j2 = simplex[c][1] >= 2 ? 1 : 0;
			int k2 = simplex[c][2] >= 2 ? 1 : 0;
			int l2 = simplex[c][3] >= 2 ? 1 : 0;
			// The number 1 in the "simplex" array is at the second smallest coordinate.
			int i3 = simplex[c][0] >= 1 ? 1 : 0; // The integer offsets for the fourth simplex corner
			int j3 = simplex[c][1] >= 1 ? 1 : 0;
			int k3 = simplex[c][2] >= 1 ? 1 : 0;
			int l3 = simplex[c][3] >= 1 ? 1 : 0;
			// The fifth corner has all coordinate offsets=1, so no need to look that up.
			float x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
			float y1 = y0 - j1 + G4;
			float z1 = z0 - k1 + G4;
			float w1 = w0 - l1 + G4;
			float x2 = x0 - i2 + 2 * G4; // Offsets for third corner in (x,y,z,w) coords
			float y2 = y0 - j2 + 2 * G4;
			float z2 = z0 - k2 + 2 * G4;
			float w2 = w0 - l2 + 2 * G4;
			float x3 = x0 - i3 + 3 * G4; // Offsets for fourth corner in (x,y,z,w) coords
			float y3 = y0 - j3 + 3 * G4;
			float z3 = z0 - k3 + 3 * G4;
			float w3 = w0 - l3 + 3 * G4;
			float x4 = x0 - 1 + 4 * G4; // Offsets for last corner in (x,y,z,w) coords
			float y4 = y0 - 1 + 4 * G4;
			float z4 = z0 - 1 + 4 * G4;
			float w4 = w0 - 1 + 4 * G4;
			// Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
			int ii = i & 0xFF;
			int jj = j & 0xFF;
			int kk = k & 0xFF;
			int ll = l & 0xFF;
			// Calculate the contribution from the five corners
			float n0, t20, t40, gx0, gy0, gz0, gw0; // Noise contribution, Gradients at simplex corners
			float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
			if (t0 < 0)
			{
				n0 = t0 = t20 = t40 = gx0 = gy0 = gz0 = gw0 = 0;
			}
			else
			{
				t20 = t0 * t0;
				t40 = t20 * t20;
				float[] grads = grad4(perm[ii + perm[jj + perm[kk + perm[ll]]]]);
				gx0 = grads[0];
				gy0 = grads[1];
				gz0 = grads[2];
				gw0 = grads[3];
				n0 = t40 * (gx0 * x0 + gy0 * y0 + gz0 * z0 + gw0 * w0);
			}
			float n1, t21, t41, gx1, gy1, gz1, gw1; // Noise contribution, Gradients at simplex corners
			float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
			if (t1 < 0)
			{
				n1 = t1 = t21 = t41 = gx1 = gy1 = gz1 = gw1 = 0;
			}
			else
			{
				t21 = t1 * t1;
				t41 = t21 * t21;
				float[] grads = grad4(perm[ii + i1 + perm[jj + j1 + perm[kk + k1 + perm[ll + l1]]]]);
				gx1 = grads[0];
				gy1 = grads[1];
				gz1 = grads[2];
				gw1 = grads[3];
				n1 = t41 * (gx1 * x1 + gy1 * y1 + gz1 * z1 + gw1 * w1);
			}
			float n2, t22, t42, gx2, gy2, gz2, gw2; // Noise contribution, Gradients at simplex corners
			float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
			if (t2 < 0)
			{
				n2 = t2 = t22 = t42 = gx2 = gy2 = gz2 = gw2 = 0;
			}
			else
			{
				t22 = t2 * t2;
				t42 = t22 * t22;
				float[] grads = grad4(perm[ii + i2 + perm[jj + j2 + perm[kk + k2 + perm[ll + l2]]]]);
				gx2 = grads[0];
				gy2 = grads[1];
				gz2 = grads[2];
				gw2 = grads[3];
				n2 = t42 * (gx2 * x2 + gy2 * y2 + gz2 * z2 + gw2 * w2);
			}
			float n3, t23, t43, gx3, gy3, gz3, gw3; // Noise contribution, Gradients at simplex corners
			float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
			if (t3 < 0)
			{
				n3 = t3 = t23 = t43 = gx3 = gy3 = gz3 = gw3 = 0;
			}
			else
			{
				t23 = t3 * t3;
				t43 = t23 * t23;
				float[] grads = grad4(perm[ii + i3 + perm[jj + j3 + perm[kk + k3 + perm[ll + l3]]]]);
				gx3 = grads[0];
				gy3 = grads[1];
				gz3 = grads[2];
				gw3 = grads[3];
				n3 = t43 * (gx3 * x3 + gy3 * y3 + gz3 * z3 + gw3 * w3);
			}
			// Noise contribution, Gradients at simplex corners
			float n4, t24, t44, gx4, gy4, gz4, gw4;
			float t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
			if (t4 < 0)
			{
				n4 = t4 = t24 = t44 = gx4 = gy4 = gz4 = gw4 = 0;
			}
			else
			{
				t24 = t4 * t4;
				t44 = t24 * t24;
				float[] grads = grad4(perm[ii + 1 + perm[jj + 1 + perm[kk + 1 + perm[ll + 1]]]]);
				gx4 = grads[0];
				gy4 = grads[1];
				gz4 = grads[2];
				gw4 = grads[3];
				n4 = t44 * (gx4 * x4 + gy4 * y4 + gz4 * z4 + gw4 * w4);
			}
			float Noise = n0 + n1 + n2 + n3 + n4;
			float temp0 = t20 * t0 * (gx0 * x0 + gy0 * y0 + gz0 * z0 + gw0 * w0);
			float dx = temp0 * x0;
			float dy = temp0 * y0;
			float dz = temp0 * z0;
			float dw = temp0 * w0;
			float temp1 = t21 * t1 * (gx1 * x1 + gy1 * y1 + gz1 * z1 + gw1 * w1);
			dx += temp1 * x1;
			dy += temp1 * y1;
			dz += temp1 * z1;
			dw += temp1 * w1;
			float temp2 = t22 * t2 * (gx2 * x2 + gy2 * y2 + gz2 * z2 + gw2 * w2);
			dx += temp2 * x2;
			dy += temp2 * y2;
			dz += temp2 * z2;
			dw += temp2 * w2;
			float temp3 = t23 * t3 * (gx3 * x3 + gy3 * y3 + gz3 * z3 + gw3 * w3);
			dx += temp3 * x3;
			dy += temp3 * y3;
			dz += temp3 * z3;
			dw += temp3 * w3;
			float temp4 = t24 * t4 * (gx4 * x4 + gy4 * y4 + gz4 * z4 + gw4 * w4);
			dx += temp4 * x4;
			dy += temp4 * y4;
			dz += temp4 * z4;
			dw += temp4 * w4;
			dx *= -8;
			dy *= -8;
			dz *= -8;
			dw *= -8;
			dx += t40 * gx0 + t41 * gx1 + t42 * gx2 + t43 * gx3 + t44 * gx4;
			dy += t40 * gy0 + t41 * gy1 + t42 * gy2 + t43 * gy3 + t44 * gy4;
			dz += t40 * gz0 + t41 * gz1 + t42 * gz2 + t43 * gz3 + t44 * gz4;
			dw += t40 * gw0 + t41 * gw1 + t42 * gw2 + t43 * gw3 + t44 * gw4;
			dx *= 27.2568f; // Scale derivative to match the noise scaling:
			dy *= 27.2568f;
			dz *= 27.2568f;
			dw *= 27.2568f;

			float myNoise = Mathf.Clamp(Noise * 27.2568f + 0.000252695f, -1.0f, 1.0f);

            if (normed)
            {
                return new float[] {
                    (myNoise + 1) / 2,
                    (dx + 1) / 2,
                    (dy + 1) / 2,
                    (dz + 1) / 2,
                    (dw + 1) / 2
                };
            }

            return new float[] { myNoise, dx, dy, dz, dw };

        }
	}

}