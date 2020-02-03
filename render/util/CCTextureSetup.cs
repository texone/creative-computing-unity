using UnityEngine;
using UnityEngine.Rendering;

namespace cc.creativecomputing.render
{
    public class CCTextureSetup
    {

        public int width;
        public int height;
        public int depth = 1;
        public RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
        public FilterMode filterMode = FilterMode.Point;
        public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        public bool useMipMap = false;
        public TextureDimension dimension = TextureDimension.Tex2D;
        public int volumeDepth = 1;

        public CCTextureSetup(int theWidth, int theHeight)
        {
            width = theWidth;
            height = theHeight;
        }
    }
}