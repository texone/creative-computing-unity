using UnityEngine.Serialization;

namespace cc.creativecomputing.math.signal
{
	using System;
	using UnityEngine;

	[Serializable]
	public class CCMixSignal : CCSignal
	{

		[Range(min : 0, max : 1)]
		public float saw = 0;
		[Range(min : 0, max : 1)]
		public float simplex = 0;
		[Range( min : 0, max : 1)]
		public float sine = 0;
		[Range(min : 0, max : 1)]
		public float square = 0;
		[Range(min : 0, max : 1)]
		public float tri = 0;
		[Range(min : 0, max : 1)]
		public float slopedTri = 0;
		[Range(min : 0, max : 1)]
		public float amp = 0;


		private CCSawSignal _mySaw;
		private CCSimplexNoise _mySimplex;
		private CCSinSignal _mySine;
		
		public CCSquareSignal squareSig;
		
		public CCTriSignal triSig;
		private CCSlopedTriSignal _mySlopedtriSig;

		public CCMixSignal()
		{
			_mySaw = new CCSawSignal();
			_mySimplex = new CCSimplexNoise();
			_mySine = new CCSinSignal();
			squareSig = new CCSquareSignal();
			triSig = new CCTriSignal();
			_mySlopedtriSig = new CCSlopedTriSignal();
		}

		private float mixSignal(float[] theSaw, float[] theSimplex, float[] theSine, float[] thesquareSig, float[] thetriSig, float[] theSlopedtriSig)
		{
			float myMaxAmount = saw + simplex + sine + square + tri + slopedTri;
			if (myMaxAmount == 0)
			{
				return 0;
			}
			return (
				theSaw[0] * saw + 
				theSimplex[0] * simplex + 
				theSine[0] * sine + 
				thesquareSig[0] * square + 
				thetriSig[0] * tri + 
				theSlopedtriSig[0] * slopedTri
			) / myMaxAmount * amp;
		}

		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			return new float[]{mixSignal(
				saw == 0 ? new float[]{0} : _mySaw.SignalImpl(theX, theY, theZ), 
				simplex == 0 ? new float[]{0} : _mySimplex.SignalImpl(theX, theY, theZ), 
				sine == 0 ? new float[]{0} : _mySine.SignalImpl(theX, theY, theZ), 
				square == 0 ? new float[]{0} : squareSig.SignalImpl(theX, theY, theZ), 
				tri == 0 ? new float[]{0} : triSig.SignalImpl(theX, theY, theZ), 
				slopedTri == 0 ? new float[]{0} : _mySlopedtriSig.SignalImpl(theX, theY, theZ))
			};
		}

		public override float[] SignalImpl(float theX, float theY)
		{
			return new float[]{mixSignal(
				saw == 0 ? new float[]{0} : _mySaw.SignalImpl(theX, theY), 
				simplex == 0 ? new float[]{0} : _mySimplex.SignalImpl(theX, theY), 
				sine == 0 ? new float[]{0} : _mySine.SignalImpl(theX, theY), 
				square == 0 ? new float[]{0} : squareSig.SignalImpl(theX, theY), 
				tri == 0 ? new float[]{0} : triSig.SignalImpl(theX, theY), 
				slopedTri == 0 ? new float[]{0} : _mySlopedtriSig.SignalImpl(theX, theY))
			};
		}

		public override float[] SignalImpl(float theX)
		{
			return new float[]{mixSignal(
				saw == 0 ? new float[]{0} : _mySaw.SignalImpl(theX), 
				simplex == 0 ? new float[]{0} : _mySimplex.SignalImpl(theX), 
				sine == 0 ? new float[]{0} : _mySine.SignalImpl(theX), 
				square == 0 ? new float[]{0} : squareSig.SignalImpl(theX), 
				tri == 0 ? new float[]{0} : triSig.SignalImpl(theX), 
				slopedTri == 0 ? new float[]{0} : _mySlopedtriSig.SignalImpl(theX))
			};
		}

	}

}