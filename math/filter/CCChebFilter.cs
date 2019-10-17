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

// This Chebyshev Filter implementation has been ported from the BASIC 
// implementation outlined in Chapter 20 of The Scientist and Engineer's
// Guide to Signal Processing, which can be found at:
//
//     http://www.dspguide.com/ch20.htm

namespace cc.creativecomputing.math.filter
{

    /// <summary>
    /// A Chebyshev filter is an IIR filter that uses a particular method to
    /// calculate the coefficients of the filter. It is defined by whether it is a
    /// low pass filter or a high pass filter and the number of poles it has. You
    /// needn't worry about what a pole is, exactly, just know that more poles
    /// usually makes for a better filter. An additional limitation is that the
    /// number of poles must be even. See <seealso cref="#poles(int)"/> for more information
    /// about poles. Another characteristic of Chebyshev filters is how much "ripple"
    /// they allow in the pass band. The pass band is the range of frequencies that
    /// the filter lets through. The "ripple" in the pass band can be seen as wavy
    /// line in the frequency response of the filter. Lots of ripple is bad, but more
    /// ripple gives a faster rolloff from the pass band to the stop band (the range
    /// of frequencies blocked by the filter). Faster rolloff is good because it
    /// means the cutoff is sharper. Ripple is expressed as a percentage, such as
    /// 0.5% ripple.
    /// 
    /// @author Damien Di Fede
    /// @author christian riekoff </summary>
    /// <seealso cref= <a href="http://www.dspguide.com/ch20.htm">Chebyshev Filters</a>
    ///  </seealso>
    [AddComponentMenu("Filter/cheb")]
    public class CCChebFilter : CCIIRFilter
    {

        public enum ChebFilterType
        {
            /// <summary>
            /// A constant used to indicate a low pass filter. </summary>
            LP,
            /// <summary>
            /// A constant used to indicate a high pass filter. </summary>
            HP
        }

        public ChebFilterType type = ChebFilterType.LP;
        [Range(2, 20)]
        public int poles = 2;
        [Range(0, 1)]
        public float ripple = 0.5f;


        /// <summary>
        /// Sets the number of poles used in the filter. The number of poles must be
        /// even and between 2 and 20. This function will report an error if either
        /// of those conditions are not met. However, it should also be mentioned
        /// that depending on the current cutoff frequency of the filter, the number
        /// of poles that will result in a <i>stable</i> filter, can be a few as 4.
        /// The function does not report an error in the case of the number of
        /// requested poles resulting in an unstable filter. For reference, here is a
        /// table of the maximum number of poles possible according to cutoff
        /// frequency:
        /// <para>
        /// <table border="1" cellpadding="5">
        /// <tr>
        /// <td>Cutoff Frequency<br />
        /// (expressed as a fraction of the sampling rate)</td>
        /// <td>0.02</td>
        /// <td>0.05</td>
        /// <td>0.10</td>
        /// <td>0.25</td>
        /// <td>0.40</td>
        /// <td>0.45</td>
        /// <td>0.48</td>
        /// </tr>
        /// <tr>
        /// <td>Maximum poles</td>
        /// <td>4</td>
        /// <td>6</td>
        /// <td>10</td>
        /// <td>20</td>
        /// <td>10</td>
        /// <td>6</td>
        /// <td>4</td>
        /// </tr>
        /// </table>
        /// 
        /// </para>
        /// </summary>
        /// <param name="thePoles"> - the number of poles </param>

        public void calcpoles()
        {
            poles = poles / 2 * 2;
            if (poles < 2)
            {
                poles = 2;
                return;
            }
            if (poles % 2 != 0)
            {
                return;
            }
            if (poles > 20)
            {
                poles = 20;
            }
        }



        // where the poles will wind up
        internal float[] ca = new float[23];
        internal float[] cb = new float[23];

        // temporary arrays for working with ca and cb
        internal float[] ta = new float[23];
        internal float[] tb = new float[23];

        // arrays to hold the two-pole coefficients
        // used during the aggregation process
        internal float[] pa = new float[3];
        internal float[] pb = new float[2];

        protected internal override void CalcCoeff()
        {
            calcpoles();
            

            // System.out.println("ChebFilter is calculating coefficients...");

            // initialize our arrays
            for (int i = 0; i < 23; ++i)
            {
                ca[i] = cb[i] = ta[i] = tb[i] = 0.0f;
            }

            // I don't know why this must be done
            ca[2] = 1.0f;
            cb[2] = 1.0f;

            // calculate two poles at a time
            for (int p = 1; p <= poles / 2; p++)
            {
                // calc pair p, put the results in pa and pb
                calcTwoPole(p, pa, pb);

                // copy ca and cb into ta and tb
                Array.Copy(ca, 0, ta, 0, ta.Length);
                Array.Copy(cb, 0, tb, 0, tb.Length);

                // add coefficients to the cascade
                for (int i = 2; i < 23; i++)
                {
                    ca[i] = pa[0] * ta[i] + pa[1] * ta[i - 1] + pa[2] * ta[i - 2];
                    cb[i] = tb[i] - pb[0] * tb[i - 1] - pb[1] * tb[i - 2];
                }
            }

            // final stage of combining coefficients
            cb[2] = 0;
            for (int i = 0; i < 21; i++)
            {
                ca[i] = ca[i + 2];
                cb[i] = -cb[i + 2];
            }

            // normalize the gain
            float sa = 0;
            float sb = 0;
            for (int i = 0; i < 21; i++)
            {
                if (type == ChebFilterType.LP)
                {
                    sa += ca[i];
                    sb += cb[i];
                }
                else
                {
                    sa += ca[i] * CCMath.Pow(-1, i);
                    sb += cb[i] * CCMath.Pow(-1, i);
                }
            }

            float gain = sa / (1 - sb);

            for (int i = 0; i < 21; i++)
            {
                ca[i] /= gain;
            }

            // initialize the coefficient arrays used by process()
            // but only if the number of poles has changed
            if (a == null || a.Length != poles + 1)
            {
                a = new float[poles + 1];
            }
            if (b == null || b.Length != poles)
            {
                b = new float[poles];
            }
            // copy the values from ca and cb into a and b
            // in this implementation cb[0] = 0 and cb[1] is where
            // the b coefficients begin, so they are numbered the way
            // one normally numbers coefficients when talking about IIR filters
            // however, process() expects b[0] to be the coefficient B1
            // so we copy cb over to b starting at index 1
            Array.Copy(ca, 0, a, 0, a.Length);
            Array.Copy(cb, 1, b, 0, b.Length);

        }

        private void calcTwoPole(int p, float[] pa, float[] pb)
        {
            float np = poles;

            // precalc
            float angle = CCMath.PI / (np * 2) + (p - 1) * CCMath.PI / np;

            float rp = -(float)Math.Cos(angle);
            float ip = (float)Math.Sin(angle);

            // warp from a circle to an ellipse
            if (ripple > 0)
            {
                // precalc
                float ratio = 100.0f / (100.0f - ripple);
                float ratioSquared = ratio * ratio;

                float es = 1.0f / (float)Math.Sqrt(ratioSquared - 1.0f);

                float oneOverNP = 1.0f / np;
                float esSquared = es * es;

                float vx = oneOverNP * (float)Math.Log(es + Math.Sqrt(esSquared + 1.0f));
                float kx = oneOverNP * (float)Math.Log(es + Math.Sqrt(esSquared - 1.0f));

                float expKX = (float)Math.Exp(kx);
                float expNKX = (float)Math.Exp(-kx);

                kx = (expKX + expNKX) * 0.5f;

                float expVX = (float)Math.Exp(vx);
                float expNVX = (float)Math.Exp(-vx);
                float oneOverKX = 1.0f / kx;

                rp *= ((expVX - expNVX) * 0.5f) * oneOverKX;
                ip *= ((expVX + expNVX) * 0.5f) * oneOverKX;
            }

            // s-domain to z-domain conversion
            float t = 2.0f * (float)Math.Tan(0.5f);
            float w = CCMath.TWO_PI * (frequency / sampleRate);
            float m = rp * rp + ip * ip;

            // precalc
            float fourTimesRPTimesT = 4.0f * rp * t;
            float tSquared = t * t;
            float mTimesTsquared = m * tSquared;
            float tSquaredTimes2 = 2.0f * tSquared;

            float d = 4.0f - fourTimesRPTimesT + mTimesTsquared;

            // precalc
            float oneOverD = 1.0f / d;

            float x0 = tSquared * oneOverD;
            float x1 = tSquaredTimes2 * oneOverD;
            float x2 = x0;

            float y1 = (8.0f - (tSquaredTimes2 * m)) * oneOverD;
            float y2 = (-4.0f - fourTimesRPTimesT - mTimesTsquared) * oneOverD;

            // LP to LP, or LP to HP transform
            float k;
            float halfW = w * 0.5f;

            if (type == ChebFilterType.HP)
            {
                k = -(float)Math.Cos(halfW + 0.5f) / (float)Math.Cos(halfW - 0.5f);
            }
            else
            {
                k = (float)Math.Sin(0.5f - halfW) / (float)Math.Sin(0.5f + halfW);
            }

            // precalc
            float kSquared = k * k;
            float x1timesK = x1 * k;
            float kDoubled = 2.0f * k;
            float y1timesK = y1 * k;

            d = 1.0f + y1timesK - y2 * kSquared;

            // precalc
            oneOverD = 1.0f / d;

            pa[0] = (x0 - x1timesK + (x2 * kSquared)) * oneOverD;
            pa[1] = ((-kDoubled * x0) + x1 + (x1 * kSquared) - (kDoubled * x2)) * oneOverD;
            pa[2] = ((x0 * kSquared) - x1timesK + x2) * oneOverD;

            pb[0] = (kDoubled + y1 + (y1 * kSquared) - (y2 * kDoubled)) * oneOverD;
            pb[1] = (-kSquared - y1timesK + y2) * oneOverD;

            if (type == ChebFilterType.HP)
            {
                pa[1] = -pa[1];
                pb[0] = -pb[0];
            }
        }
    }

}