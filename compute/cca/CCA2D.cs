using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEditor.Rendering;

public class CCA2D : MonoBehaviour
{
    private const int MAX_RANGE = 10;
    private const int MAX_THRESHOLD = 25;
    private const int MAX_STATES = 20;
    
    [Header("Simulation Parameters")] 
    [Range(1, MAX_RANGE)]public int range = 1;
    [Range(0, MAX_THRESHOLD)]public int threshold = 1;
    [Range(0, MAX_STATES)]public int nStates = 1;
    [Range(0, 1)]public float randomProb = 1;
    public bool moore;

    [Header("Color Parameters")] 
    [Range(0, 1)] public float fade;
    [Range(0, 15)] public float stateAmount; 
    [Range(0, 15)] public float countAmount;
    
    [Range(0, 1)] public float hue0;
    [Range(0, 1)] public float hue1;

    [Range(0, 1)] public float sat0;
    [Range(0, 1)] public float sat1;

    [Range(0, 1)] public float bri0;
    [Range(0, 1)] public float bri1;
    
    [Header("Setup")] 
    [Range(8, 2048)] public int resolution = 8;

    [Range(0, 50)] public int stepsPerFrame = 0;
    
    [Range(1, 50)] public int stepMod = 1;

    public ComputeShader cs;

    public Material outMat;
    private RenderTexture _outTex;
    
    
    private RenderTexture _readTex;
    private RenderTexture _writeTex;

    private int _stepKernel;
    private static readonly int UnlitColorMap = Shader.PropertyToID("_UnlitColorMap");

    private RenderTexture CreateTexture(RenderTextureFormat theFormat)
    {
        var myResult = new RenderTexture(resolution, resolution, 1, theFormat)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat,
            useMipMap = false,

        };
        myResult.Create();
        return myResult;
    }

    private void SwapTex()
    {
        var tmp = _readTex;
        _readTex = _writeTex;
        _writeTex = tmp;
    }

    private void CreateBuffers()
    {
        var myLookUps = new List<float>();
        for(var x = -range; x <= range; x++){
            for(var y = -range; y <= range; y++){
                if((x == 0 && y == 0))continue;
                if (!moore && x != 0 && y != 0) continue;
                
                myLookUps.Add(x);
                myLookUps.Add(y);
            }
        }
        var myLookupsBuffer = new ComputeBuffer(myLookUps.Count,sizeof(float), ComputeBufferType.Constant);
        myLookupsBuffer.SetData(myLookUps);
        Debug.Log(myLookUps.Count);
        cs.SetBuffer(cs.FindKernel("StepKernel"),"lookups", myLookupsBuffer);
    }
    
    private void SetSimulationParameters()
    {
        
        cs.SetInt("range", range);
        cs.SetInt("threshold", threshold);
        cs.SetInt("nStates", nStates);
        cs.SetBool("moore", moore);

        CreateBuffers();
    }
    
    private void SetColorParameters()
    {
        
        cs.SetFloat("fade", fade);
        cs.SetFloat("stateAmount", stateAmount);
        cs.SetFloat("countAmount", countAmount);
        
        cs.SetFloat("hue0", hue0);
        cs.SetFloat("hue1", hue1);

        cs.SetFloat("sat0", sat0);
        cs.SetFloat("sat1", sat1);

        cs.SetFloat("bri0", bri0);
        cs.SetFloat("bri1", bri1);
        cs.SetFloat("offset", Time.time);
        cs.SetFloat("randomProb", randomProb);
    }

    private void GPUResetKernel()
    {
        var k = cs.FindKernel("ResetKernel");
        cs.SetTexture(k, "writeTex",_writeTex);
        cs.SetInt("resolution", resolution);
        
        SetSimulationParameters();
        
        cs.Dispatch(k, resolution, resolution, 1);
        SwapTex();
    }

    [Button]
    private void Reset()
    {
        _readTex = CreateTexture(RenderTextureFormat.RFloat);
        _writeTex = CreateTexture(RenderTextureFormat.RFloat);
        _outTex = CreateTexture(RenderTextureFormat.ARGBFloat);

        _stepKernel = cs.FindKernel("StepKernel");
        GPUResetKernel();
    }

    private void Start()
    {
        Reset();
    }


    [Button]
    public void RandomizeParameters()
    {
        range = (int)Random.Range(1, MAX_RANGE);
        threshold  = (int)Random.Range(1, MAX_THRESHOLD);
        nStates =  (int)Random.Range(2, MAX_STATES);
        moore = Random.value > 0.5f;
        
        SetSimulationParameters();
    }

    [Button]
    public void ResetAndRandomize()
    {
        RandomizeParameters();
        Reset();
    }

    [Button]
    private void Step()
    {
        
        cs.SetTexture(_stepKernel, "readTex",_readTex);
        cs.SetTexture(_stepKernel, "writeTex",_writeTex);
        cs.SetTexture(_stepKernel, "outTex",_outTex);
        cs.SetTexture(_stepKernel, "outTex2",_outTex);
        
        cs.Dispatch(_stepKernel, resolution, resolution, 1);
        
        SwapTex();
        
        outMat.SetTexture(UnlitColorMap, _outTex);
    }

    private void Update()
    {
        if (Time.frameCount % stepMod != 0) return;

        SetColorParameters();
        
        for (var i = 0; i < stepsPerFrame;i++)
        {
            Step();
        }
    }
}
