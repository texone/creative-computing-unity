using cc.creativecomputing.render;
using EasyButtons;
using UnityEngine;

public class ExpansiveReactionDiffusion : CCTextureProvider
{
    
    public ComputeShader cs;
    
    private CCDoubleBufferedTexture _bufferA;
    private CCDoubleBufferedTexture _bufferB;
    private CCDoubleBufferedTexture _bufferC;

    private RenderTexture _render;
    private RenderTexture _normals;

    public Material outMat;
    
    private int _myBufferAKernel;
    private int _myBufferBKernel;
    private int _myBufferCKernel;
    private int _myResetKernel;
    private int _myRenderKernel;
    private int _myNormalKernel;

    public int width = 1200;
    public int height = 600;

    public string textureProperty = "_UnlitColorMap";

    private void Start()
    {
        _myBufferAKernel = cs.FindKernel("BufferA");
        _myBufferBKernel = cs.FindKernel("BufferB");
        _myBufferCKernel = cs.FindKernel("BufferC");
        _myResetKernel = cs.FindKernel("Reset");
        _myRenderKernel = cs.FindKernel("Render");
        _myNormalKernel = cs.FindKernel("Normals");

        var mySetup = new CCTextureSetup(width, height)
        {
            format = RenderTextureFormat.RFloat,
            filterMode = FilterMode.Bilinear
        };
        _bufferA = new CCDoubleBufferedTexture(mySetup);
        _bufferB = new CCDoubleBufferedTexture(mySetup);
        _bufferC = new CCDoubleBufferedTexture(mySetup);
        _render = CCDoubleBufferedTexture.CreateTexture(mySetup);

        var myNormalSetup = new CCTextureSetup(width, height)
        {
            format = RenderTextureFormat.ARGBFloat,
            filterMode = FilterMode.Bilinear
        };
        _normals = CCDoubleBufferedTexture.CreateTexture(myNormalSetup);
        Reset();
    }
    
    [Button]
    private void Reset()
    {
        cs.SetTexture(_myResetKernel, "bufferAWrite", _bufferA.WriteTex);
        cs.SetFloats("iResolution", width, height);
        cs.SetFloat("iTime", Time.time);
        cs.Dispatch(_myResetKernel, width, height, 1);
        _bufferA.Swap();
    }

    private void Simulate()
    {
        
        cs.SetTexture(_myBufferAKernel, "bufferAWrite", _bufferA.WriteTex);
        cs.SetTexture(_myBufferAKernel, "bufferARead", _bufferA.ReadTex);
        cs.SetTexture(_myBufferAKernel, "bufferCRead", _bufferC.ReadTex);
        
        cs.SetFloats("iResolution", width, height);
        cs.SetFloat("iTime", Time.time);
        
        cs.Dispatch(_myBufferAKernel, width, height, 1);
        _bufferA.Swap();
    }

    private void HorizontalBlur()
    {
        cs.SetTexture(_myBufferBKernel, "bufferARead", _bufferA.ReadTex);
        cs.SetTexture(_myBufferBKernel, "bufferBWrite", _bufferB.WriteTex);
        
        cs.SetFloats("iResolution", width, height);
        cs.SetFloat("iTime", Time.time);
        
        cs.Dispatch(_myBufferBKernel, width, height, 1);
        
        _bufferB.Swap();
    }
    
    private void VerticalBlur()
    {
        cs.SetTexture(_myBufferCKernel, "bufferBRead", _bufferB.ReadTex);
        cs.SetTexture(_myBufferCKernel, "bufferCWrite", _bufferC.WriteTex);
        
        cs.SetFloats("iResolution", width, height);
        cs.SetFloat("iTime", Time.time);
        
        cs.Dispatch(_myBufferCKernel, width, height, 1);
        
        _bufferC.Swap();
    }

    private void Render()
    {
        cs.SetTexture(_myRenderKernel, "bufferCRead", _bufferC.ReadTex);
        cs.SetTexture(_myRenderKernel, "renderWrite", _render);
        
        cs.Dispatch(_myRenderKernel, width, height, 1);
    }

    private void Normals()
    {
        cs.SetTexture(_myNormalKernel, "bufferCRead", _bufferC.ReadTex);
        cs.SetTexture(_myNormalKernel, "normalWrite", _normals);
        
        cs.SetFloats("iResolution", width, height);
        cs.Dispatch(_myNormalKernel, width, height, 1);
    }

    // Update is called once per frame
    private void Update()
    {
        Simulate();
        HorizontalBlur();
        VerticalBlur();
        Render();
        Normals();
        outMat.SetTexture(textureProperty, _render);
    }

    public override Texture Texture()
    {
        return _render;
    }
}
