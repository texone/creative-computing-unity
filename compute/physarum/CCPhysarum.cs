using System.Collections;
using System.Collections.Generic;
using cc.creativecomputing.render;
using UnityEngine;
using EasyButtons;
using UnityEngine.SocialPlatforms;

namespace cc.creativecomputing.compute
{
    [ExecuteAlways]
    public class CCPhysarum : MonoBehaviour
    {
        [Header("Trail Agent Params")] [Range(1, 50000)]
        public int agentsCount = 1;

        private ComputeBuffer agentsBuffer;

        [Header("Setup")] [Range(8, 2048)] 
        public int rez = 8;
        [Range(0, 50)] 
        public int stepsPerFrame = 8;
        [Range(1, 50)] 
        public int stepMod = 8;

        public Material outMat;
        public ComputeShader cs;

        private CCDoubleBufferedTexture tex;
        private RenderTexture outTex;

        private int agentDebugKernel;

        protected List<ComputeBuffer> buffers;
        protected List<ComputeBuffer> textures;

        protected int stepn = -1;

        private void Reset()
        {
            agentDebugKernel = cs.FindKernel("AgentsDebugKernel");
            
            var mySetup = new CCTextureSetup(rez, rez)
            {
                format = RenderTextureFormat.RFloat,
                filterMode = FilterMode.Point
            };
            tex = new CCDoubleBufferedTexture(mySetup);
            
            agentsBuffer = new ComputeBuffer(agentsCount, sizeof(float) * 4);
            buffers.Add(agentsBuffer);

            GPUResetKernel();
            //RenderTexture();
        }

        private void GPUResetKernel()
        {
            int kernel;
            
            cs.SetInt("rez", rez);
            cs.SetInt("time", Time.frameCount);

            kernel = cs.FindKernel("ResetTextureKernel");
            cs.SetTexture(kernel,"writeTex", tex.WriteTex);
            cs.Dispatch(kernel, rez, rez, 1);
            
            cs.SetTexture(kernel,"writeTex", tex.ReadTex);
            cs.Dispatch(kernel, rez, rez, 1);
            
            kernel = cs.FindKernel("ResetAgentsKernel");
            cs.SetBuffer(kernel,"agentsBuffer", agentsBuffer);
            cs.Dispatch(kernel, agentsCount, 1, 1);
        }

        private void Start()
        {
            Reset();
        }

        private void Update()
        {
            
        }
    }
}