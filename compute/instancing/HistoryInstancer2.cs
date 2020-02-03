using System.Collections;
using System.Collections.Generic;
using System.Resources;
using cc.creativecomputing.render;
using EasyButtons;
using UnityEngine;
using UnityEngine.Serialization;


public class HistoryBuffer
{
    
    public ComputeBuffer argBuffer;
    public ComputeBuffer voxelBuffer;

    private int _width;
    private int _height;
    private int _depth;

    private int _index;

    
    public HistoryBuffer(int theWidth, int theHeight, int theDepth)
    {
        
        _width = theWidth;
        _height = theHeight;
        _depth = theDepth;

        Reset();
    }

    public void Reset()
    {
        
        voxelBuffer?.Release();
        voxelBuffer = new ComputeBuffer(_width * _height * _depth,sizeof(float) * 3, ComputeBufferType.Append);
        voxelBuffer.SetCounterValue(0);

        argBuffer?.Release();
        argBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        
    }

    
}

public class HistoryInstancer2 : MonoBehaviour
{
    // Start is called before the first frame update

    public ComputeShader compute;
    public Mesh mesh;
    public Material material;
    
    public int index = 0;
    public int width;
    public int height;
    public CCTextureProvider input;
    
    private readonly Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 30);
    
    private List<HistoryBuffer> _myBuffers = new List<HistoryBuffer>();
    private uint[] args = new uint[5] { 0,0,0,0,0};
    
    [Range(0,5)]
    public float size = 0.5f;
    [Range(0,5)]
    public float spacing = 0.5f;
    [Range(8,1024)]
    public int depth = 64;
    
    [Range(10,20)]
    public int subBuffers = 10;
    public int subBufferDepth;

    private HistoryBuffer _currentBuffer;

    private void OnValidate()
    {
        Reset();
        Debug.Log("YO");
    }
    

    [Button]
    private void Reset()
    {
        var texture = input.Texture();
        if (!texture) return;
        
        index = 0;

        width = texture.width;
        height = texture.width;
        
        subBufferDepth = depth / (subBuffers - 1) + 1;
        _myBuffers.Clear();

        for (var i = 0; i < subBuffers;i++)
        {
            _myBuffers.Add(new HistoryBuffer(width, height,subBufferDepth));
        }

        _currentBuffer = _myBuffers[0];
        
        Debug.Log("RESET");
    }
    
    
    // Update is called once per frame
    private void Update()
    {
        
        
        var texture = input.Texture();
        if (!texture) return;

        if (_myBuffers.Count == 0) Reset();

        width = texture.width;
        height = texture.height;
        
        // 1. Build History

        
        if (index >= subBufferDepth)
        {
            _currentBuffer = _myBuffers[_myBuffers.Count - 1];
            _myBuffers.RemoveAt(_myBuffers.Count - 1);
            _myBuffers.Insert(0,_currentBuffer);
            index = 0;
        }
        
        
        var k = compute.FindKernel("WriteHistory");
        compute.SetTexture(k, "inTex", texture);
        compute.SetBuffer(k, "voxelBuffer", _currentBuffer.voxelBuffer);
        compute.SetInt("index", index);
        compute.Dispatch(k, width , height , 1);
        index++;
        /*
        // 2. Render
*/
        args[0] = mesh.GetIndexCount(0);
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
       
        /*
        ComputeBuffer.CopyCount(_currentBuffer.voxelBuffer, _currentBuffer.argBuffer, sizeof(int));
        */
        /*
        var count = new int[1];
        var countBuffer = new ComputeBuffer(1, sizeof(uint), ComputeBufferType.IndirectArguments);
        ComputeBuffer.CopyCount(_currentBuffer.voxelBuffer, countBuffer, 0);
        countBuffer.GetData(count);
        Debug.Log("Count:" + count[0]);
        countBuffer.Release();
        */
        Debug.Log(_currentBuffer);
        
        material.SetInt("width", width);
        material.SetInt("height", height);
        material.SetFloat("size", size);
        material.SetFloat("spacing", spacing);
        material.SetMatrix("mat", transform.localToWorldMatrix);
        material.SetVector("position", transform.position);
        material.SetFloat("offset", index + 0 * subBufferDepth);
        material.SetBuffer("voxelBuffer", _currentBuffer.voxelBuffer);
        
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, _currentBuffer.argBuffer);
        /*
        for(var i = 0; i < _myBuffers.Count;i++){
        }
        */
    }
}
