using System.Collections;
using System.Collections.Generic;
using cc.creativecomputing.render;
using compute.util;
using UnityEngine;





public class TextureInstancer : MonoBehaviour
{

    public Mesh mesh;
    public Material material;
    public CCTextureProvider input;
    readonly Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);

    [Range(0,5)]
    public float size = 0.5f;
    [Range(0,5)]
    public float sizeY = 0.5f;
    [Range(0,5)]
    public float spacing = 0.5f;
    private ComputeBuffer argBuffer;

    private uint[] args = new uint[5] { 0,0,0,0,0};
    
    

    // Update is called once per frame
    private void Update()
    {
        argBuffer?.Release();

        var texture = input.Texture();
        if (!texture) return;

        int width = texture.width;
        int height = texture.height;
        argBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = mesh.GetIndexCount(0);
        args[1] =  (uint)(width * height);
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        argBuffer.SetData(args);
            
        material.SetTexture("tex", texture);
        material.SetMatrix("mat", transform.localToWorldMatrix);
        material.SetVector("position", transform.position);
        material.SetInt("width", width);
        material.SetInt("height", height);
        material.SetFloat("size", size);
        material.SetFloat("sizeY", sizeY);
        material.SetFloat("spacing", spacing);
        Graphics.DrawMeshInstancedIndirect(mesh,0,material,bounds,argBuffer);
    }
}
