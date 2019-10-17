using System;
using UnityEngine;
using cc.creativecomputing.math.util;

/*
 *  Copyright (c) 2007 - 2008 by Damien Di Fede <ddf@compartmental.net>
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU Library General Public License as published
 *   by the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU Library General Public License for more details.
 *
 *   You should have received a copy of the GNU Library General Public
 *   License along with this program; if not, write to the Free Software
 *   Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

namespace cc.creativecomputing.math.filter
{

    /// <summary>
    /// An Infinite Impulse Response, or IIR, filter is a filter that uses a set of
    /// coefficients and previous filtered values to filter a stream of audio. It is
    /// an efficient way to do digital filtering. IIRFilter is a general IIRFilter
    /// that simply applies the filter designated by the filter coefficients so that
    /// sub-classes only have to dictate what the values of those coefficients are by
    /// defining the <code><seealso cref="#calcCoeff()"/></code> function. When filling the coefficient
    /// arrays, be aware that <code>b[0]</code> corresponds to
    /// <code>b<sub>1</sub></code>.
    /// 
    /// @author Damien Di Fede
    /// 
    /// </summary>
    public abstract class CCIIRFilter : CCFilter
    {
        [Range(0.01f, 1)]
        public float frequency = 0.01f;

        /// <summary>
        /// The a coefficients. </summary>
        protected internal float[] a;
        /// <summary>
        /// The b coefficients. </summary>
        protected internal float[] b;

        /// <summary>
        /// The input values to the left of the output value currently being
        /// calculated.
        /// </summary>
        private float[][] input;

        /// <summary>
        /// The previous output values. </summary>
        private float[][] output;

        private float _myPreviousFrequency = -1f;



        /// <summary>
        /// Initializes the in and out arrays based on the number of coefficients
        /// being used.
        /// 
        /// </summary>
        private void InitArrays()
        {
            int memSize = (a.Length >= b.Length) ? a.Length : b.Length;
            input = new float[_myChannels][];//, memSize);
            output = new float[_myChannels][];//, memSize);
            for (int i = 0; i < _myChannels; i++)
            {
                input[i] = new float[memSize];
                output[i] = new float[memSize];
            }
        }

        private void OnValidate()
        {
            CalcCoeff();
        }

        private void Start()
        {
            CalcCoeff();
        }

        public override float Process(int theChannel, float signal, float theTime)
        {
            _myChannels = CCMath.Max(_myChannels, theChannel + 1);

           // Debug.Log(_myChannels + " " + input.Length);
            // make sure we have enough filter buffers
            if (input == null || input.Length != _myChannels || (input[theChannel].Length < a.Length && input[theChannel].Length < b.Length))
            {
                InitArrays();
            }

            // apply the filter to the sample value in each channel
            Array.Copy(input[theChannel], 0, input[theChannel], 1, input[theChannel].Length - 1);
            input[theChannel][0] = signal;
            float y = 0;
            for (int ci = 0; ci < a.Length; ci++)
            {
                y += a[ci] * input[theChannel][ci];
            }
            for (int ci = 0; ci < b.Length; ci++)
            {
                y += b[ci] * output[theChannel][ci];
            }
            Array.Copy(output[theChannel], 0, output[theChannel], 1, output[theChannel].Length - 1);

            if (double.IsNaN(y))
            {
                y = 0;
            }
            output[theChannel][0] = y;

            if (bypass)
            {
                return signal;
            }
            return y;

        }



        /// <summary>
        /// Calculates the coefficients of the filter using the current cutoff
        /// frequency. To make your own IIRFilters, you must extend IIRFilter and
        /// implement this function. The frequency is expressed as a fraction of the
        /// sample rate. When filling the coefficient arrays, be aware that
        /// <code>b[0]</code> corresponds to the coefficient
        /// <code>b<sub>1</sub></code>.
        /// 
        /// </summary>
        protected internal abstract void CalcCoeff();

        /// <summary>
        /// Prints the current values of the coefficients to the console.
        /// 
        /// </summary>
        public void PrintCoeff()
        {
            Console.WriteLine("Filter coefficients: ");
            if (a != null)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    Console.Write("  A" + i + ": " + a[i]);
                }
            }
            Console.WriteLine();
            if (b != null)
            {
                for (int i = 0; i < b.Length; i++)
                {
                    Console.Write("  B" + (i + 1) + ": " + b[i]);
                }
                Console.WriteLine();
            }
        }
    }

}