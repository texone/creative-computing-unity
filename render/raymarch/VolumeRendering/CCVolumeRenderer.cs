﻿
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using compute.util;
using UnityEngine;

namespace cc.creativecomputing.render.raymarch
{

    public class CCVolumeRenderer : MonoBehaviour {

        [SerializeField] protected Shader shader;
        protected Material material;

        [SerializeField] Color color = Color.white;
        [Range(0f, 1f)] public float threshold = 0.5f;
        [Range(0.5f, 5f)] public float intensity = 1.5f;
        [Range(0f, 1f)] public float sliceXMin = 0.0f, sliceXMax = 1.0f;
        [Range(0f, 1f)] public float sliceYMin = 0.0f, sliceYMax = 1.0f;
        [Range(0f, 1f)] public float sliceZMin = 0.0f, sliceZMax = 1.0f;
        public Quaternion axis = Quaternion.identity;

        public CCTextureProvider volume;

        protected virtual void Start () {
            material = new Material(shader);
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Build();
        }
        
        protected void Update () {
            material.SetTexture("_Volume", volume.Texture());
            material.SetColor("_Color", color);
            material.SetFloat("_Threshold", threshold);
            material.SetFloat("_Intensity", intensity);
            material.SetVector("_SliceMin", new Vector3(sliceXMin, sliceYMin, sliceZMin));
            material.SetVector("_SliceMax", new Vector3(sliceXMax, sliceYMax, sliceZMax));

            material.SetMatrix("_AxisRotationMatrix", Matrix4x4.Rotate(axis));
        }

        private Mesh Build() {

            var mesh = new Mesh
            {
                vertices = new[] {
                    new Vector3 (-0.5f, -0.5f, -0.5f),
                    new Vector3 ( 0.5f, -0.5f, -0.5f),
                    new Vector3 ( 0.5f,  0.5f, -0.5f),
                    new Vector3 (-0.5f,  0.5f, -0.5f),
                    new Vector3 (-0.5f,  0.5f,  0.5f),
                    new Vector3 ( 0.5f,  0.5f,  0.5f),
                    new Vector3 ( 0.5f, -0.5f,  0.5f),
                    new Vector3 (-0.5f, -0.5f,  0.5f),
                }, 
                triangles = new[] {
                    0, 2, 1,
                    0, 3, 2,
                    2, 3, 4,
                    2, 4, 5,
                    1, 2, 5,
                    1, 5, 6,
                    0, 7, 4,
                    0, 4, 3,
                    5, 4, 7,
                    5, 7, 6,
                    0, 6, 7,
                    0, 1, 6
                }
            };
            mesh.RecalculateNormals();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            return mesh;
        }

        private void OnValidate()
        {
            Constrain(ref sliceXMin, ref sliceXMax);
            Constrain(ref sliceYMin, ref sliceYMax);
            Constrain(ref sliceZMin, ref sliceZMax);
        }

        private static void Constrain (ref float min, ref float max)
        {
            const float threshold = 0.025f;
            if(min > max - threshold)
            {
                min = max - threshold;
            } else if(max < min + threshold)
            {
                max = min + threshold;
            }
        }

        private void OnDestroy()
        {
            Destroy(material);
        }

    }

}


