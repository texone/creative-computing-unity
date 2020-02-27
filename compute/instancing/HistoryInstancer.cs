using System.Collections;
using System.Collections.Generic;
using System.Resources;
using cc.creativecomputing.render;
using cc.creativecomputing.ui;
using UnityEngine;
using UnityEngine.Serialization;

public class HistoryInstancer : MonoBehaviour
{
    // Start is called before the first frame update

    public ComputeShader compute;
    public Mesh mesh;
    public Material material;
    
    public int index = 0;
    public int width;
    public int height;
    public CCTextureProvider input;

    private ComputeBuffer argBuffer;
    private ComputeBuffer voxelBuffer;
    private uint[] args = new uint[5] { 0,0,0,0,0};
    private readonly Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 30);
    
    
    [Range(0,5)]
    public float size = 0.5f;
   [Range(0,5)]
    public float spacing = 0.5f;
    [Range(8,1024)]
    public int depth = 64;

    public bool append = true;

    [CCUIButton]
    private void Reset()
    {
        index = 0;
        append = true;
        var texture = input.Texture();
        if (!texture) return;

        width = texture.width;
        height = texture.width;
        
        voxelBuffer?.Release();
        voxelBuffer = new ComputeBuffer(width * depth * height,sizeof(float) * 3, ComputeBufferType.Append);
        voxelBuffer.SetCounterValue(0);

        argBuffer?.Release();
        argBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    private void BuildHistory()
    {
        if(!append)return;
        var k = compute.FindKernel("WriteHistory");
        compute.SetTexture(k, "inTex", input.Texture());
        compute.SetBuffer(k, "voxelBuffer", voxelBuffer);
        compute.SetInt("index", index);
        compute.Dispatch(k, width , height , 1);
        index++;
    }

    private void RenderHistory()
    {
        args[0] = mesh.GetIndexCount(0);
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
       
        ComputeBuffer.CopyCount(voxelBuffer, argBuffer, sizeof(int));
        
        /*
        var count = new int[1];
        var countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        ComputeBuffer.CopyCount(voxelBuffer, countBuffer, 0);
        countBuffer.GetData(count);
        Debug.Log("Count:" + count[0]);
        countBuffer.Release();
        */
        
        material.SetBuffer("voxelBuffer", voxelBuffer);
        material.SetInt("width", width);
        material.SetInt("height", height);
        material.SetFloat("size", size);
        material.SetFloat("spacing", spacing);
        material.SetMatrix("mat", transform.localToWorldMatrix);
        material.SetVector("position", transform.position);
        
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argBuffer);
    }
    // Update is called once per frame
    private void Update()
    {
        
        
        var texture = input.Texture();
        if (!texture) return;
        if (voxelBuffer == null || index >= depth)
        {
            append = false;
           // Debug.Log("RESET");
            //Reset();
        }
        
        width = texture.width;
        height = texture.height;

        BuildHistory();
        RenderHistory();
    }
}
