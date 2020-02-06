using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "diffuse kernel", menuName = "Compute/Diffuse")]
[Serializable]
public class CCDiffuseKernel : ScriptableObject
{
    public ComputeShader cs;

    [Range(0, 1)] public float diffuseDecayFactor = .9f;
    [Range(0, 10)] public float diffuseSize = 1f;

    private int _kernel;
    
    public void Start()
    {
        _kernel = cs.FindKernel("Kernel");
    }

    public void Execute(Texture _readTexture, Texture _writeTexture)
    {
        cs.SetTexture(_kernel, "diffuseReadTex", _readTexture);
        cs.SetTexture(_kernel, "diffuseWriteTex", _writeTexture);
        cs.SetFloat("diffuseDecayFactor", diffuseDecayFactor);
        cs.SetFloat("diffuseSize", diffuseSize);

        cs.Dispatch(_kernel, _readTexture.width, _readTexture.height, 1);
    }
}
