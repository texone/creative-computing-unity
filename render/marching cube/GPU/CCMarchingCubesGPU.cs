using compute.util;
using UnityEngine;

namespace cc.creativecomputing.render.marchingcube{
    public class CCMarchingCubesGPU : MonoBehaviour
    {
        public ComputeShader compute;
        private int _kernel;
        private int _maxTriNum;

        private struct Triangles
        {
            public Vector3 posA, posB, posC;
            public Vector3 normalA, normalB, normalC;
        }

        private ComputeBuffer _tribuffer;
        private ComputeBuffer _countBuffer; 

        Mesh mesh;
        // marching cube feature
        public bool EnableSmooth;
        private int GridRes = 5;
        public float IsoLevel = 0.5f;

        public Material mat;

        public CCTextureProvider textureInput;

        private void Start()
        {

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>().material = mat;
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        }

        private void UpdateMesh()
        {
            if (!textureInput.Texture()) return;

            if (_tribuffer == null)
            {
                
                GridRes = textureInput.Texture().width;
                _maxTriNum = GridRes * GridRes * GridRes; // the max triangle num
                var maxValue = _maxTriNum;
                _tribuffer = new ComputeBuffer(maxValue, sizeof(float) * 3 * 3 * 2, ComputeBufferType.Append);
            }

            _tribuffer.SetCounterValue(0);

            _kernel = compute.FindKernel("CalculateTriangle");
            
            // common
            compute.SetInt("GridRes", GridRes);
            compute.SetFloat("IsoLevel", IsoLevel);
            compute.SetBool("EnableSmooth", EnableSmooth);
            compute.SetTexture(_kernel, "GridValTexture", textureInput.Texture());
            compute.SetBuffer(_kernel, "Tribuffer", _tribuffer);
            compute.Dispatch(_kernel, GridRes, GridRes, GridRes);


            ComputeBuffer.CopyCount(_tribuffer, _countBuffer, 0);
            int[] countArr = { 0 };
            _countBuffer.GetData(countArr);
            var count = countArr[0]; // the actual num
            var appendData = new Triangles[count];
            _tribuffer.GetData(appendData, 0, 0, count);

            var posArr = new Vector3[count * 3];
            var normalArr = new Vector3[count * 3];
            var indexArr = new int[count * 3];
            
            for (var i = 0; i < count; i++)
            {
                posArr[i * 3] = appendData[i].posA;
                posArr[i * 3 + 1] = appendData[i].posB;
                posArr[i * 3 + 2] = appendData[i].posC;

                normalArr[i * 3] = appendData[i].normalA;
                normalArr[i * 3 + 1] = appendData[i].normalB;
                normalArr[i * 3 + 2] = appendData[i].normalC;

                indexArr[i * 3] = i * 3;
                indexArr[i * 3 + 1] = i * 3 + 1;
                indexArr[i * 3 + 2] = i * 3 + 2;
            }
            mesh.Clear();
            mesh.vertices = posArr;
            mesh.normals = normalArr;
            mesh.triangles = indexArr;
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void OnDisable()
        {
            _tribuffer.Release();
            _tribuffer.Dispose();
            _countBuffer.Release();
            _countBuffer.Dispose();
        }
    }
}