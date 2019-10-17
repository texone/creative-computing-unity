using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
namespace cc.creativecomputing.render
{
    [VolumeComponentMenu("Sky/Texture Sky")]
    [SkyUniqueID(991)]
    public class CCTextureSky : SkySettings
    {

        public ClampedFloatParameter dither = new ClampedFloatParameter(5, 0, 20);

        public TextureParameter texture = new TextureParameter(null);

        public override SkyRenderer CreateRenderer()
        {
            return new CCTextureSkyRenderer(this);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();

            unchecked
            {
                hash = hash * 23 + texture.GetHashCode();
            }

            return hash;
        }
    }
}
