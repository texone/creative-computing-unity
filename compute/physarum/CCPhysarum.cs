using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cc.creativecomputing.render;
using UnityEngine;
using EasyButtons;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

namespace cc.creativecomputing.compute
{
    [ExecuteAlways][RequireComponent(typeof(CCDiffuseKernel))]
    public class CCPhysarum : MonoBehaviour
    {
        [Header("Trail Agent Params")] [Range(64, 1000000)]
        public int agentsCount = 1;

        private ComputeBuffer agentsBuffer;

        

        [Header("Mouse Input")] [Range(0, 100)]
        public int brushSize = 10;

        public GameObject interactivePlane;
        private Vector2 _hitXy;


        [Header("Setup")] [Range(8, 2048)] public int rez = 8;

        [Range(0, 50)] public int stepsPerFrame = 0;

        [Range(1, 50)] public int stepMod = 1;

        public Material outMat;
        public ComputeShader cs;

        public CCDiffuseKernel diffuse;
        
        private RenderTexture trailReadTex;
        private RenderTexture trailWriteTex;
        private RenderTexture outTex;
        private RenderTexture debugTex;

        private int _agentsDebugKernel;
        private int _moveAgentsKernel;
        private int _writeTrailsKernel;
        private int _renderKernel;
        private int _diffuseTextureKernel;

        private int _resetTextureKernel;
        private int _resetAgentsKernel;


        private List<ComputeBuffer> _buffers;
        private List<RenderTexture> _textures;

        private int _stepn = -1;

        private void Start()
        {
            Reset();
        }

        [Button]
        public void Reset()
        {
            _agentsDebugKernel = cs.FindKernel("AgentsDebugKernel");
            _moveAgentsKernel = cs.FindKernel("MoveAgentsKernel");
            _renderKernel = cs.FindKernel("RenderKernel");
            _writeTrailsKernel = cs.FindKernel("WriteTrailsKernel");
            _diffuseTextureKernel = cs.FindKernel("DiffuseTextureKernel");

            _resetTextureKernel = cs.FindKernel("ResetTextureKernel");
            _resetAgentsKernel = cs.FindKernel("ResetAgentsKernel");

            trailReadTex = CreateTexture(rez, FilterMode.Point);
            trailWriteTex = CreateTexture(rez, FilterMode.Point);
            outTex = CreateTexture(rez, FilterMode.Point);
            debugTex = CreateTexture(rez, FilterMode.Point);

            agentsBuffer = new ComputeBuffer(agentsCount, sizeof(float) * 4);
            _buffers.Add(agentsBuffer);

            ComputeReset();
            Render();
        }

        private void ComputeReset()
        {
            cs.SetInt("rez", rez);
            cs.SetInt("time", Time.frameCount);
            
            cs.SetTexture(_resetTextureKernel, "resetTex", trailWriteTex);
            cs.Dispatch(_resetTextureKernel, rez, rez, 1);

            cs.SetBuffer(_resetAgentsKernel, "agentsBuffer", agentsBuffer);
            cs.Dispatch(_resetAgentsKernel, agentsCount / 64, 1, 1);
        }
        
        private void Update()
        {
            if (Time.frameCount % stepMod != 0) return;
            for (var i = 0; i < stepsPerFrame; i++)
            {
                Step();
            }

        }

        [Button]
        public void Step()
        {

            HandleInput();


            _stepn += 1;
            cs.SetInt("time", Time.frameCount);
            cs.SetInt("stepn", _stepn);
            cs.SetInt("brushSize", brushSize);
            cs.SetVector("hitXY", _hitXy);

            GPUMoveAgentsKernel();

            if (_stepn % 2 == 1)
            {
                
                ComputeDiffuse();
                GPUWriteTrailsKernel();
                SwapTex();
            }

            Render();
        }

        private void HandleInput()
        {
            if (!Input.GetMouseButton(0))
            {
                _hitXy.x = _hitXy.y = 0;
                return;
            }

            RaycastHit hit;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                return;
            }


            if (hit.transform != interactivePlane.transform)
            {
                return;
            }

            _hitXy = hit.textureCoord * rez;
        }
        
        [Range(0, 1)] public float diffuseDecayFactor = .9f;
        [Range(0, 10)] public float diffuseSize = 1f;

        private void ComputeDiffuse()
        {
            cs.SetTexture(_diffuseTextureKernel, "diffuseReadTex", trailReadTex);
            cs.SetTexture(_diffuseTextureKernel, "diffuseWriteTex", trailWriteTex);
            cs.SetFloat("diffuseDecayFactor", diffuseDecayFactor);
            cs.SetFloat("diffuseSize", diffuseSize);

            cs.Dispatch(_diffuseTextureKernel, rez, rez, 1);
        }


        private void GPUMoveAgentsKernel()
        {
            cs.SetBuffer(_moveAgentsKernel, "agentsBuffer", agentsBuffer);
            cs.SetTexture(_moveAgentsKernel, "trailReadTex", trailReadTex);
            cs.SetTexture(_moveAgentsKernel, "debugTex", debugTex);

            cs.Dispatch(_moveAgentsKernel, agentsCount / 64, 1, 1);
        }

        private void GPUWriteTrailsKernel()
        {
            cs.SetBuffer(_writeTrailsKernel, "agentsBuffer", agentsBuffer);
            cs.SetTexture(_writeTrailsKernel, "trailWriteTex", trailWriteTex);
            cs.Dispatch(_writeTrailsKernel, agentsCount / 64, 1, 1);
        }

        private void SwapTex()
        {
            var tmp = trailReadTex;
            trailReadTex = trailWriteTex;
            trailWriteTex = tmp;
        }
        
        private void Render()
        {
            GPURenderKernel();
            GPUAgentsDebugKernel();

            outMat.SetTexture("_UnlitColorMap", outTex);
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView.RepaintAll();
            }
        }


        private void GPURenderKernel()
        {
            cs.SetTexture(_renderKernel, "trailReadTex", trailReadTex);
            cs.SetTexture(_renderKernel, "outTex", outTex);
            cs.SetTexture(_renderKernel, "debugTex", debugTex);

            cs.Dispatch(_renderKernel, rez, rez, 1);
        }


        private void GPUAgentsDebugKernel()
        {
            cs.SetBuffer(_agentsDebugKernel, "agentsBuffer", agentsBuffer);
            cs.SetTexture(_agentsDebugKernel, "outTex", outTex);

            cs.Dispatch(_agentsDebugKernel, agentsCount / 64, 1, 1);
        }

        public void Release()
        {

            _buffers?.ForEach(buffer => buffer?.Release());
            _buffers = new List<ComputeBuffer>();

            if (_textures != null)
            {
                foreach (var tex in _textures.Where(tex => tex != null))
                {
                    tex.Release();
                }
            }

            _textures = new List<RenderTexture>();

        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnEnable()
        {
            Release();
        }

        private void OnDisable()
        {
            Release();
        }

        protected RenderTexture CreateTexture(int r, FilterMode filterMode)
        {
            RenderTexture texture = new RenderTexture(r, r, 1, RenderTextureFormat.ARGBFloat);

            texture.name = "out";
            texture.enableRandomWrite = true;
            texture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            texture.volumeDepth = 1;
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.autoGenerateMips = false;
            texture.useMipMap = false;
            texture.Create();
            _textures.Add(texture);

            return texture;
        }
    }
}