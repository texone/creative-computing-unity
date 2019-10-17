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
using Random = UnityEngine.Random;

namespace cc.creativecomputing.math.signal
{

	/// <summary>
	/// @author max goettner
	/// 
	/// </summary>
	[Serializable]
	public class CCRandom2DSignal : CCSignal
	{


		[Range(1, 1000)]
		public int _cNSteps = 0;

		[Range(0, 1)]
		internal float R = 0.1f;

		[Range(0, 1)]
		internal float dMin = 0.1f;

		[Range(0, 1)]
		internal float dMax = 0.5f;


		internal int currentStep = 0;
		internal float phi = 0;

		internal Vector3 step = new Vector3();
		internal Vector3 nextPosition = new Vector3();
		internal Vector3 currentPosition = new Vector3();


		private void getNewRandomPos()
		{
			//nextPosition = new Vector3(currentPosition);
			phi += 2 * Mathf.PI * Random.Range(dMin, dMax);

			step.x = R * Mathf.Cos(phi) / (float)_cNSteps; //(nextPosition.x - currentPosition.x) / (float)_cNSteps;
			step.y = R * Mathf.Sin(phi) / (float)_cNSteps; //(nextPosition.y - currentPosition.y) / (float)_cNSteps;
			step.z = 0; // (nextPosition.z - currentPosition.z) / (float)_cNSteps;
		}

		private void nextStep()
		{

			if (currentPosition.x + step.x > 1 || currentPosition.x + step.x < -1)
			{
				step.x = 0; //-step.x;
			}
			if (currentPosition.y + step.y > 1 || currentPosition.y + step.y < -1)
			{
				step.y = 0; // -step.y;
			}

            currentPosition += step;
			currentStep += 1;
			if (currentStep == _cNSteps)
			{
				currentStep = 0;
                getNewRandomPos();
			}
		}
		public override float[] SignalImpl(float theX, float theY, float theZ)
		{
			nextStep();
			return new float[]{currentPosition.x, currentPosition.y, currentPosition.z};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float, float)
		 */
		public override float[] SignalImpl(float theX, float theY)
		{
			nextStep();
			return new float[]{currentPosition.x, currentPosition.y};
		}

		/* (non-Javadoc)
		 * @see cc.creativecomputing.math.signal.CCSignal#signalImpl(float)
		 */
		public override float[] SignalImpl(float theX)
		{
			nextStep();
			return new float[]{currentPosition.x};
		}

	}

}