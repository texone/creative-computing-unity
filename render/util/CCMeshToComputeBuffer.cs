using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace cc.creativecomputing.render
{

    [ExecuteAlways]
    public class CCMeshToComputeBuffer : MonoBehaviour
    {
        public GameObject sourceObject;

        public bool bakeVertices = true;
        public bool bakeNormals = false;
        public bool bakeColors = false;

        // Compute shaders
        public ComputeShader bakeFloat3;
        public ComputeShader bakeFloat4;
        
        private RenderTexture _myComputeTexture;

        public string renderTexturePath;
        public RenderTexture VFXPositionMap;
        public RenderTexture VFXColormap;

        private int _myVertexCount = 0;
        private int _myVertexTexturexSize = 0;

        public void Initialize()
        {
            if (sourceObject == null) return;
            MeshFilter[] myMeshFilters = sourceObject.GetComponentsInChildren<MeshFilter>();
            int myTriangles = 0;
            foreach (MeshFilter myFilter in myMeshFilters)
            {
                myTriangles += myFilter.sharedMesh.triangles.Length;
                _myVertexCount += myFilter.sharedMesh.vertexCount;
            }
            if (_myVertexCount == 0) return;
            _myVertexTexturexSize = Mathf.CeilToInt(Mathf.Sqrt(_myVertexCount));

            Debug.Log(" num points " + _myVertexCount + " texsize " + _myVertexTexturexSize);

           
            _myComputeTexture = new RenderTexture(_myVertexTexturexSize, _myVertexTexturexSize, 0, RenderTextureFormat.ARGBFloat);
            _myComputeTexture.enableRandomWrite = true;
            _myComputeTexture.Create();

        }



        public RenderTexture Bake<Type>(RenderTexture theOuput, string theAssetName, Func<Mesh, Type[]> theFunc, ComputeShader theShader, int theStride)
        {

            if (_myVertexCount == 0) return null;

            if (theOuput == null || theOuput.width != _myVertexTexturexSize || theOuput.height != _myVertexTexturexSize)
            {
                theOuput = new RenderTexture(_myVertexTexturexSize, _myVertexTexturexSize, 0, RenderTextureFormat.ARGBFloat);

#if UNITY_EDITOR

                Debug.Log("Creating texture");
                AssetDatabase.CreateAsset(theOuput, renderTexturePath + "/" + theAssetName + "_" + gameObject.name + ".asset");

#else
                
                Debug.LogError("VFX Render Texture should exist");
                
#endif
            }


            // Compute
            ComputeBuffer computeBuffer = new ComputeBuffer(_myVertexCount, theStride * sizeof(float));

            MeshFilter[] myMeshFilters = sourceObject.GetComponentsInChildren<MeshFilter>();
            int myIndex = 0;
            foreach (MeshFilter myFilter in myMeshFilters)
            {
                Type[] myData = theFunc(myFilter.sharedMesh);
                computeBuffer.SetData(myData, 0, myIndex, myData.Length);
                myIndex += myData.Length;
            }

            theShader.SetInt("size", _myVertexTexturexSize);
            theShader.SetInt("lastIndex", _myVertexCount);
            theShader.SetBuffer(0, "computeBuffer", computeBuffer);
            theShader.SetTexture(0, "computeTexture", _myComputeTexture);
            theShader.Dispatch(0, (_myVertexTexturexSize / 8) + 1, (_myVertexTexturexSize / 8) + 1, 1);

            Graphics.CopyTexture(_myComputeTexture, theOuput);

            computeBuffer.Dispose();

            return theOuput;
        }
        public void Bake()
        {

            Initialize();

            if (bakeVertices) VFXPositionMap = Bake(VFXPositionMap, "RT_VERTEX", mesh => mesh.vertices, bakeFloat3, 3);
        }

        private void OnValidate()
        {
            Bake();
        }

        // Start is called before the first frame update
        void Start()
        {
            Bake();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BakeVertices()
        {
           
        }

        public void BakeColors()
        {
            /*

            // COlor texture

            if (VFXColormap == null)
            {

                VFXColormap = new RenderTexture(texSize, texSize, 0, RenderTextureFormat.ARGBFloat);

#if UNITY_EDITOR

                Debug.Log("Creating texture");
                VFXColormap = new RenderTexture(texSize, texSize, 0, RenderTextureFormat.ARGBFloat); // Square texture
                AssetDatabase.CreateAsset(VFXColormap, renderTexturePath + "/RT_Col_" + gameObject.name + ".asset");
#else
            Debug.LogError("VFX Render Texture should exist");
#endif
            }
            else if (VFXColormap.width != texSize || VFXColormap.height != texSize)
            {

                //Create again

            }


            colorBuffer = new ComputeBuffer(numPoints, 4 * sizeof(float));

            colorBuffer.SetData(mesh.colors);

            colorBaker.SetInt("dim", texSize);

            colorBaker.SetTexture(0, "ColorTexture", inputPositionTexture);

            colorBaker.SetBuffer(0, "ColorBuffer", colorBuffer);

            colorBaker.Dispatch(0, (texSize / 8) + 1, (texSize / 8) + 1, 1);

            Graphics.CopyTexture(inputPositionTexture, VFXColormap);

            colorBuffer.Dispose();

            */

        }


       

        

       
    }
}
