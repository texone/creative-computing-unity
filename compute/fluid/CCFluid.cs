// StableFluids - A GPU implementation of Jos Stam's Stable Fluids on Unity
// https://github.com/keijiro/StableFluids

using System.Collections.Generic;
using compute.util;
using UnityEngine;

namespace cc.creativecomputing.compute.fluid
{
    public class CCFluidInput
    {
        public Vector2 position;
        public Vector2 motion;

        public CCFluidInput(Vector2 thePosition, Vector2 theMotion)
        {
            position = thePosition;
            motion = theMotion;
        }
    }
    public class CCFluid : MonoBehaviour 
    {
        #region Editable attributes

        [SerializeField] private int resolution = 512;
        [SerializeField] private float viscosity = 1e-6f;
        [SerializeField] private float force = 300;
        [SerializeField] private float exponent = 200;
        [SerializeField] private Texture2D initial;

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] private ComputeShader compute;
        [SerializeField, HideInInspector] private Shader shader;

        #endregion

        #region Private members

        private Material _shaderSheet;

      
        private int _advect;
        private int _force;
        private int _pSetup;
        private int _pFinish;
        private int _jacobi1;
        private int _jacobi2;
        private int _add2D;

        private int ThreadCountX => (resolution                                + 7) / 8;
        private int ThreadCountY => (resolution * height / width + 7) / 8;

        private int ResolutionX => ThreadCountX * 8;
        private int ResolutionY => ThreadCountY * 8;

        // Vector field buffers
        private RenderTexture _velocityKeep;
        private CCDoubleBufferedTexture _velocityBuffer;
        private CCDoubleBufferedTexture _pressureBuffer;
        private CCDoubleBufferedTexture _externalBuffer;

        // Color buffers (for double buffering)
        private RenderTexture _colorRT1;
        private RenderTexture _colorRT2;

        public RenderTexture outputColor;
        public RenderTexture outputForce;
        public RenderTexture outputInput;

        public int width = 7560;
        public int height = 1920;
        private RenderTexture AllocateBuffer(int componentCount, int width = 0, int height = 0)
        {
            var format = RenderTextureFormat.ARGBHalf;
            switch (componentCount)
            {
                case 1:
                    format = RenderTextureFormat.RHalf;
                    break;
                case 2:
                    format = RenderTextureFormat.RGHalf;
                    break;
            }

            if (width  == 0) width  = ResolutionX;
            if (height == 0) height = ResolutionY;

            var rt = new RenderTexture(width, height, 0, format) {enableRandomWrite = true};
            rt.Create();
            return rt;
        }

        #endregion

        #region MonoBehaviour implementation

        private void OnValidate()
        {
            resolution = Mathf.Max(resolution, 8);
        }

        private void Start()
        {
            _advect = compute.FindKernel("Advect2D");
            _force = compute.FindKernel("Force");
            _pSetup = compute.FindKernel("PSetup");
            _pFinish = compute.FindKernel("PFinish");
            _jacobi1 = compute.FindKernel("Jacobi1D");
            _jacobi2 = compute.FindKernel("Jacobi2D");
            
            _add2D = compute.FindKernel("Add2D");
            
            _shaderSheet = new Material(shader);

            _velocityKeep = AllocateBuffer(2);

            var rSetup = new CCTextureSetup(ResolutionX, ResolutionY)
            {
                format = RenderTextureFormat.RHalf
            };
            var rgSetup = new CCTextureSetup(ResolutionX, ResolutionY)
            {
                format = RenderTextureFormat.RGHalf
            };
            var rgbSetup = new CCTextureSetup(ResolutionX, ResolutionY)
            {
                format = RenderTextureFormat.ARGBHalf
            };
            
            _velocityBuffer = new CCDoubleBufferedTexture(rgSetup);
            _pressureBuffer = new CCDoubleBufferedTexture(rSetup);
            _externalBuffer = new CCDoubleBufferedTexture(rgbSetup);

            _colorRT1 = AllocateBuffer(4, width, height);
            _colorRT2 = AllocateBuffer(4, width, height);

            Graphics.Blit(initial, _colorRT1);
            
        }

        private void OnDestroy()
        {
            Destroy(_shaderSheet);
            Destroy(_velocityKeep);
            
            _velocityBuffer.Destroy();
            _pressureBuffer.Destroy();
            _externalBuffer.Destroy();

            Destroy(_colorRT1);
            Destroy(_colorRT2);
        }

        private void Advect2D(int threadGroupsX, int threadGroupsY, int threadGroupsZ, Texture advectIn, Texture advectOut, float deltaTime)
        {
            compute.SetFloat("advectDeltaTime", deltaTime);
            compute.SetTexture(_advect, "Advect2DIn", advectIn);
            compute.SetTexture(_advect, "Advect2DOut", advectOut);
            compute.Dispatch(_advect, threadGroupsX, threadGroupsY, threadGroupsZ);
        }

        private void Diffuse1D(int threadGroupsX, int threadGroupsY, int threadGroupsZ, Texture diffuseInB, CCDoubleBufferedTexture diffuseInOut, float diffuseAlpha, float diffuseBeta, int iterations)
        {
            compute.SetFloat("JacobiAlpha", diffuseAlpha);
            compute.SetFloat("JacobiBeta", diffuseBeta);
            compute.SetTexture(_jacobi1, "Jacobi1DInB", diffuseInB);

            // Jacobi iteration
            for (var i = 0; i < iterations; i++)
            {
                compute.SetTexture(_jacobi1, "Jacobi1DInX", diffuseInOut.ReadTex);
                compute.SetTexture(_jacobi1, "Jacobi1DOut", diffuseInOut.WriteTex);
                compute.Dispatch(_jacobi1, ThreadCountX, ThreadCountY, 1);
                diffuseInOut.Swap();
            }
        }
        
        private void Diffuse2D(int threadGroupsX, int threadGroupsY, int threadGroupsZ, Texture diffuseInB, CCDoubleBufferedTexture diffuseInOut, float diffuseAlpha, float diffuseBeta, int iterations)
        {
            compute.SetFloat("JacobiAlpha", diffuseAlpha);
            compute.SetFloat("JacobiBeta", diffuseBeta);
            compute.SetTexture(_jacobi2, "Jacobi2DInB", diffuseInB);

            // Jacobi iteration
            for (var i = 0; i < iterations; i++)
            {
                compute.SetTexture(_jacobi2, "Jacobi2DInX", diffuseInOut.ReadTex);
                compute.SetTexture(_jacobi2, "Jacobi2DOut", diffuseInOut.WriteTex);
                compute.Dispatch(_jacobi2, ThreadCountX, ThreadCountY, 1);
                diffuseInOut.Swap();
            }
        }

        public List<CCFluidInput> Inputs { get; set; } = new List<CCFluidInput>();

        [Range(0,1)]
        [SerializeField] public float _randomNess = 0;

        private void AddForce()
        {
            _externalBuffer.Clear();
            
            Inputs.ForEach(input =>
            {
                // Input point
            
                compute.SetVector("ForceOrigin", input.position / height);
                compute.SetVector("ForceVector", Vector2.Lerp(input.motion / height * force,Random.insideUnitCircle * (force * 0.25f),_randomNess));
                compute.SetFloat("ForceExponent", exponent);
                compute.SetTexture(_force, "ForceIn", _velocityBuffer.ReadTex);
                compute.SetTexture(_force, "ForceOut", _velocityBuffer.WriteTex);

                compute.Dispatch(_force, ThreadCountX, ThreadCountY, 1);
            });
            
            _velocityBuffer.Swap();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            var dx = 1.0f / ResolutionY;
            
            // Common variables
            compute.SetFloat("Time", Time.time);

            Advect2D(ThreadCountX, ThreadCountY, 1,_velocityKeep, _velocityBuffer.WriteTex, dt);
            _velocityBuffer.Swap();
            
            var difAlpha = dx * dx / (viscosity * dt);
            Graphics.CopyTexture(_velocityBuffer.ReadTex, _velocityKeep);
            Diffuse2D(ThreadCountX, ThreadCountY, 1, _velocityKeep, _velocityBuffer, difAlpha, 4 + difAlpha, 40);
           
            AddForce();

            // Projection setup
            compute.SetTexture(_pSetup, "W_in", _velocityBuffer.ReadTex);
            compute.SetTexture(_pSetup, "DivW_out", _velocityBuffer.WriteTex);
            compute.SetTexture(_pSetup, "P_out", _pressureBuffer.WriteTex);
            compute.Dispatch(_pSetup, ThreadCountX, ThreadCountY, 1);
            _pressureBuffer.Swap();
            _velocityBuffer.Swap();

            // Jacobi iteration
            Diffuse1D(ThreadCountX, ThreadCountY, 1, _velocityBuffer.ReadTex, _pressureBuffer, -dx * dx, 4, 40);

            // Projection finish
            compute.SetTexture(_pFinish, "W_in", _velocityBuffer.WriteTex);
            compute.SetTexture(_pFinish, "P_in", _pressureBuffer.ReadTex);
            compute.SetTexture(_pFinish, "U_out", _velocityKeep);
            compute.Dispatch(_pFinish, ThreadCountX, ThreadCountY, 1);

            // Apply the velocity field to the color buffer.
            var offs = Vector2.one * (Input.GetMouseButton(1) ? 0 : 1e+7f);
            //_shaderSheet.SetVector("_ForceOrigin", input + offs);
            _shaderSheet.SetFloat("_ForceExponent", exponent);
            _shaderSheet.SetTexture("_VelocityField", _velocityKeep);
            Graphics.Blit(_colorRT1, _colorRT2, _shaderSheet, 0);

            // Swap the color buffers.
            var temp = _colorRT1;
            _colorRT1 = _colorRT2;
            _colorRT2 = temp;

            
            if(outputColor)Graphics.Blit(_colorRT1, outputColor, _shaderSheet, 1);
            if(outputForce)Graphics.Blit(_velocityKeep, outputForce, _shaderSheet, 2);
            if(outputInput)Graphics.Blit(_externalBuffer.ReadTex, outputInput, _shaderSheet, 2);
            
        }

   

        #endregion
    }
}
