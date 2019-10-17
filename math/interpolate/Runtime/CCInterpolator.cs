using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cc.creativecomputing.math.util;

namespace cc.creativecomputing.math.interpolate
{

    public enum CCInterpolationMode
    {
        IN,
        OUT,
        IN_OUT
    }
    public abstract class CCInterpolator : ScriptableObject
    {
        public CCInterpolationMode mode = CCInterpolationMode.IN_OUT;

        public abstract float Interpolate(float theBlend);
    }

    [CreateAssetMenu(fileName = "Linear Interpolator", menuName = "Interpolation/Linear")]
    public class CCLinearInterpolator : CCInterpolator
    {
        public override float Interpolate(float theBlend)
        {
            return theBlend;
        }
    }

    [CreateAssetMenu(fileName = "Sine Interpolator", menuName = "Interpolation/Sine")]
    public class CCSineInterpolator : CCInterpolator
    {
        public override float Interpolate(float theBlend)
        {
            switch (mode)
            {
                case CCInterpolationMode.IN:
                    return 1 - CCMath.Cos(theBlend * CCMath.HALF_PI);
                case CCInterpolationMode.OUT:
                    return CCMath.Sin(theBlend * CCMath.HALF_PI);

            }
            return (CCMath.Cos(CCMath.PI + CCMath.PI * theBlend) + 1) / 2;
        }
    }

    [CreateAssetMenu(fileName = "Circular Interpolator", menuName = "Interpolation/Circular")]
    public class CCCircularInterpolator : CCInterpolator
    {
        public override float Interpolate(float theBlend)
        {
            switch (mode)
            {
                case CCInterpolationMode.IN:
                    return 1 - CCMath.Sqrt(1 - theBlend * theBlend);
                case CCInterpolationMode.OUT:
                    theBlend = 1 - theBlend;
                    return CCMath.Sqrt(1 - theBlend * theBlend);

            }
            theBlend *= 2;
            if (theBlend < 1) return -0.5f * (CCMath.Sqrt(1 - theBlend * theBlend) - 1);
            return 0.5f * (CCMath.Sqrt(1 - CCMath.Sq(2 - theBlend)) + 1);
        }
    }

    [CreateAssetMenu(fileName = "Exponential Interpolator", menuName = "Interpolation/Exponential")]
    public class CCExponentialInterpolator : CCInterpolator
    {
        public override float Interpolate(float theBlend)
        {
            switch (mode)
            {
                case CCInterpolationMode.IN:
                    return (theBlend == 0) ? 0 : theBlend * CCMath.Pow(2, 10 * (theBlend - 1));
                case CCInterpolationMode.OUT:
                    return (theBlend == 1) ? 1 : (-CCMath.Pow(2, -10 * theBlend) + 1);

            }
            if ((theBlend) < 0.5) return 0.5f * CCMath.Pow(2, 10 * (theBlend - 1));
            return 0.5f * (-CCMath.Pow(2, -10 * (theBlend - 1)) + 2);
        }
    }

    [CreateAssetMenu(fileName = "Pow Interpolator", menuName = "Interpolation/Pow")]
    public class CCPowerInterpolator : CCInterpolator
    {

        public float pow = 1;

        public override float Interpolate(float theBlend)
        {
            switch (mode)
            {
                case CCInterpolationMode.IN:
                    return CCMath.Pow(theBlend, pow);
                case CCInterpolationMode.OUT:
                    return 1 - CCMath.Pow(1 - theBlend, pow);

            }
            if (theBlend < 0.5f) return CCMath.Pow(theBlend * 2, pow) / 2;
            return 1 - CCMath.Pow((1 - theBlend) * 2, pow) / 2;
        }
    }

    [CreateAssetMenu(fileName = "Quadratic Interpolator", menuName = "Interpolation/Quadratic")]
    public class CCQuadraticInterpolator : CCInterpolator
    {

        public float pow = 1;

        public override float Interpolate(float theBlend)
        {
            switch (mode)
            {
                case CCInterpolationMode.IN:
                    return CCMath.Sq(theBlend);
                case CCInterpolationMode.OUT:
                    return -(theBlend) * (theBlend - 2);

            }
            if (theBlend < 0.5) return 2 * theBlend * theBlend;
            return 1 - 2 * CCMath.Sq(1 - theBlend);
        }
    }

}
