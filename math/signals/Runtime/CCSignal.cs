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

using System;
using UnityEngine;

namespace cc.creativecomputing.math.signal
{

	/// <summary>
	/// This is the base signal class that handles the basic setup, like scale and offset.
	/// Here you can also set how fractal values should be calculated. Values are always in the 
	/// range 0 to 1.
	/// @author christianriekoff
	/// 
	/// </summary>
	[Serializable]
	public abstract class CCSignal : MonoBehaviour
	{
        [Range(0, 100)]
        public float scale = 1;
        [Range(1, 10)]
        public float octaves = 1;
        [Range(0, 1)]
        public float gain = 0.5f;
        [Range(0, 10)]
        public float lacunarity = 2;

        public bool normed = true;

        protected internal float _myOffsetX = 0;
		protected internal float _myOffsetY = 0;
		protected internal float _myOffsetZ = 0;

		

		/// <summary>
		/// Override this method to define how the 3d is calculated </summary>
		/// <param name="theX"> </param>
		/// <param name="theY"> </param>
		/// <param name="theZ"> </param>
		/// <returns> the calculated value </returns>
		public abstract float[] SignalImpl(float theX, float theY, float theZ);
        
		public virtual float[] SignalImpl(float theX, float theY)
		{
			return SignalImpl(theX,theY,0);
		}
        
		public virtual float[] SignalImpl(float theX)
		{
			return SignalImpl(theX,0);
		}

		/// <summary>
		/// Returns multiple values for 3d coordinates, this is useful to get derivatives or 
		/// multiple output values like in case of the worley noise </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <returns> multiple values </returns>
		public float[] Values(float theX)
		{
			var myScale = scale;
			float myFallOff = gain;

			float myOctaves = Mathf.Floor(octaves);
			float[] myResult = null;
			float myAmp = 0;

			for (int i = 0; i < myOctaves;i++)
			{
				float[] myValues = SignalImpl(theX * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff;
				}
				myAmp += myFallOff;
				myFallOff *= gain;
				myScale *= lacunarity;
			}
			var myBlend = octaves - myOctaves;
			if (myBlend > 0)
			{
				var myValues = SignalImpl(theX * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff * myBlend;
				}
				myAmp += myFallOff * myBlend;
			}

			if (!(myAmp > 0)) return myResult;
			
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] /= myAmp;
				}
			
			return myResult;
		}

		/// <summary>
		/// Returns the value for the given coordinates. By default only one band is calculated. </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <returns> the value for the given coordinates. </returns>
		public virtual float Value(float theX)
		{
			return Values(theX)[0];
		}

		/// <summary>
		/// Returns multiple values for 3d coordinates, this is useful to get derivatives or 
		/// multiple output values like in case of the worley noise </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <param name="theY"> y coord for the noise </param>
		/// <returns> multiple values </returns>
		public float[] Values(float theX, float theY)
		{
			float myScale = scale;
			float myFallOff = gain;

			int myOctaves = (int)Mathf.Floor(octaves);
			float[] myResult = null;
			float myAmp = 0;

			for (int i = 0; i < myOctaves;i++)
			{
				float[] myValues = SignalImpl(theX * myScale, theY * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff;
				}
				myAmp += myFallOff;
				myFallOff *= gain;
				myScale *= lacunarity;
			}
			float myBlend = octaves - myOctaves;
			if (myBlend > 0)
			{
				float[] myValues = SignalImpl(theX * myScale, theY * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff * myBlend;
				}
				myAmp += myFallOff * myBlend;
			}
			if (myAmp > 0)
			{
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] /= myAmp;
				}
			}
			return myResult;
		}

		/// <summary>
		/// Returns multiple values for 3d coordinates, this is useful to get derivatives or 
		/// multiple output values like in case of the worley noise </summary>
		/// <param name="theVector"> coordinates for the noise </param>
		/// <returns> multiple values </returns>
		public virtual float[] Values(Vector2 theVector)
		{
			return Values(theVector.x, theVector.y);
		}

		/// <summary>
		/// Returns the value for the given coordinates. By default only one band is calculated. </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <param name="theY"> y coord for the noise </param>
		/// <returns> the value for the given coordinates. </returns>
		public virtual float Value(float theX, float theY)
		{
			return Values(theX, theY)[0];
		}

		/// <summary>
		/// Returns the value for the given coordinates. By default only one band is calculated. </summary>
		/// <param name="theVector"> coordinates for the noise </param>
		/// <returns> the value for the given coordinates. </returns>
		public virtual float Value(Vector2 theVector)
		{
			return Values(theVector)[0];
		}

		/// <summary>
		/// Returns multiple values for 3d coordinates, this is useful to get derivatives or 
		/// multiple output values like in case of the worley noise </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <param name="theY"> y coord for the noise </param>
		/// <param name="theZ"> z coord for the noise </param>
		/// <returns> multiple values </returns>
		public float[] Values(float theX, float theY, float theZ)
		{
			float myScale = scale;
			float myFallOff = gain;

			int myOctaves = (int)Mathf.Floor(octaves);
			float[] myResult = null;
			float myAmp = 0;

			for (int i = 0; i < myOctaves;i++)
			{
				float[] myValues = SignalImpl(theX * myScale, theY * myScale, theZ * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff;
				}
				myAmp += myFallOff;
				myFallOff *= gain;
				myScale *= lacunarity;
			}
			float myBlend = octaves - myOctaves;
			if (myBlend > 0)
			{
				float[] myValues = SignalImpl(theX * myScale, theY * myScale, theZ * myScale);
				if (myResult == null)
				{
					myResult = new float[myValues.Length];
				}
				for (int j = 0; j < myResult.Length;j++)
				{
					myResult[j] += myValues[j] * myFallOff * myBlend;
				}
				myAmp += myFallOff * myBlend;
			}

			if (myAmp <= 0) return myResult;
			{
				if (myResult != null)
					for (int j = 0; j < myResult.Length; j++)
					{
						myResult[j] /= myAmp;
					}
			}
			return myResult;
		}

		/// <summary>
		/// Returns multiple values for 3d coordinates, this is useful to get derivatives or 
		/// multiple output values like in case of the worley noise </summary>
		/// <param name="theVector"> coordinates for the noise </param>
		/// <returns> multiple values </returns>
		public virtual float[] Values(Vector3 theVector)
		{
			return Values(theVector.x, theVector.y, theVector.z);
		}

		/// <summary>
		/// Returns the value for the given coordinates. By default only one band is calculated. </summary>
		/// <param name="theX"> x coord for the noise </param>
		/// <param name="theY"> y coord for the noise </param>
		/// <param name="theZ"> z coord for the noise </param>
		/// <returns> the value for the given coordinates. </returns>
		public virtual float Value(float theX, float theY, float theZ)
		{
			return Values(theX, theY, theZ)[0];
		}

		/// <summary>
		/// Returns the value for the given coordinates. By default only one band is calculated. </summary>
		/// <param name="theVector"> coordinates for the noise </param>
		/// <returns> the value for the given coordinates. </returns>
		public virtual float Value(Vector3 theVector)
		{
			return Values(theVector)[0];
		}
        
		public virtual float[] NoiseImpl(float theX)
		{
			return SignalImpl(theX,0);
		}

		public virtual void Offset(float theX, float theY, float theZ)
		{
			_myOffsetX = theX;
			_myOffsetY = theY;
			_myOffsetZ = theZ;
		}
	}

}