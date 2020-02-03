using cc.creativecomputing.render;
using UnityEngine;

namespace cc.creativecomputing.compute.noise{
    
    public class CCComputeNoise : CCTextureProvider
    {
        public ComputeShader compute;
        private int _kernel;
        private RenderTexture _texture;

        

        // marching cube feature
        public int GridRes = 128;
        public float NoiseInterval = 0.05f;

        private void Start()
        {
            _texture = new RenderTexture(GridRes, GridRes, 0, RenderTextureFormat.RFloat)
            {
                enableRandomWrite = true,
                dimension = UnityEngine.Rendering.TextureDimension.Tex3D,
                volumeDepth = GridRes,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat,
                useMipMap = false
            };

            _texture.Create();
            
            _kernel = compute.FindKernel("Noise");
        }

        private void Update()
        {
            compute.SetInt("GridRes", GridRes);
            compute.SetFloat("Time", Time.time);
            compute.SetFloat("NoiseInterval", NoiseInterval);
            compute.SetTexture(_kernel, "NoiseTexture", _texture);
            compute.Dispatch(_kernel, GridRes, GridRes, GridRes);
        }


        public override RenderTexture Texture()
        {
            return _texture;
        }
    }
}