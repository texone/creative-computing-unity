using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cc.creativecomputing.math.util;
using cc.creativecomputing.ui;

namespace cc.creativecomputing.render.util
{
    public abstract class CCDataProvider : ScriptableObject
    {
        public abstract void GetData(RenderTexture theTargetMap, List<Vector4> theDataList);
    }

    [CreateAssetMenu(fileName = "Curve data", menuName = "gpu data/Curve to texture")]
    public class CCCurveDataProvider : CCDataProvider
    {

        public List<AnimationCurve> curves = new List<AnimationCurve>();

        public override void GetData(RenderTexture theTargetMap, List<Vector4> theDataList)
        {
            if (curves.Count <= 0) return;
            for (var y = 0; y < theTargetMap.height; y++)
            {
                var myCurve = curves[y % curves.Count];
              
                for (var x = 0; x < theTargetMap.width; x++)
                {
                    var value = myCurve.Evaluate(CCMath.Norm(x,0, theTargetMap.width - 1));
                    theDataList.Add(new Vector4(value,0,0,0));
                }

            }
        }
    }
    
    [CreateAssetMenu(fileName = "transform position data", menuName = "gpu data/transform position to texture")]
    public class CCTransformPositionProvider : CCDataProvider
    {

        public List<GameObject> gameObjects = new List<GameObject>();

        public override void GetData(RenderTexture theTargetMap, List<Vector4> theDataList)
        {
            if (gameObjects.Count <= 0) return;
            foreach (var t in gameObjects)
            {
                var position = t.transform.position;
                theDataList.Add(new Vector4(position.x,position.y,position.z,0));
            }
        }
    }

    public class CCDataToRenderTexture : MonoBehaviour
    {
        public RenderTexture targetMap = null;
        public ComputeShader computeShader;
        public CCDataProvider dataProvider;

        public bool update = true;

        private readonly List<Vector4> dataList = new List<Vector4>();
        private ComputeBuffer dataBuffer;
        private RenderTexture tempDataMap;

        private static RenderTexture CreateRenderTexture(int theWidth, int theHeight, RenderTextureFormat theFormat)
        {
            var myResult = new RenderTexture(theWidth, theHeight, 0, theFormat) {enableRandomWrite = true};
            myResult.Create();
            return myResult;
        }
        private void TransferData()
        {
            var mapWidth = targetMap.width;
            var mapHeight = targetMap.height;
            var format = targetMap.format;

            var vcount = dataList.Count;
            var vcount_x4 = vcount * 4;

            // Release the temporary objects when the size of them don't match
            // the input.

            if (dataBuffer != null && dataBuffer.count != vcount_x4)
            {
                dataBuffer.Dispose();
                dataBuffer = null;
            }

            if (tempDataMap &&
               (tempDataMap.width != mapWidth ||
                tempDataMap.height != mapHeight ||
                tempDataMap.format != format))
            {
                //Destroy(tempDataMap);

                tempDataMap = null;
            }

            // Lazy initialization of temporary objects

            if (dataBuffer == null)
            {
                dataBuffer = new ComputeBuffer(vcount_x4, sizeof(float));
            }

            if (!tempDataMap)
            {
                tempDataMap = CreateRenderTexture(mapWidth, mapHeight, format);
            }

            // Set data and execute the transfer task.
            computeShader.SetInt("VertexCount", mapWidth);

            dataBuffer.SetData(dataList);

            computeShader.SetBuffer(0, "DataBuffer", dataBuffer);
            computeShader.SetTexture(0, "DataMap", tempDataMap);

            computeShader.Dispatch(0, mapWidth, mapHeight, 1);

            Graphics.CopyTexture(tempDataMap, targetMap);
        }


        private void CheckConsistency()
        {

            if (
               targetMap.format != RenderTextureFormat.RFloat &&
               targetMap.format != RenderTextureFormat.RGFloat &&
               targetMap.format != RenderTextureFormat.ARGBFloat &&
               targetMap.format != RenderTextureFormat.RHalf &&
               targetMap.format != RenderTextureFormat.RGHalf &&
               targetMap.format != RenderTextureFormat.ARGBHalf)
            {
                Debug.LogError("map format should be Half or Float.");
            }
        }


        private void Init()
        {
            /*
            ComputeShader[] myShaders = Resources.FindObjectsOfTypeAll<ComputeShader>();
            foreach (ComputeShader myShader in myShaders)
            {
                if (myShader.name.Equals("CCDataToRenderTextureShader"))
                {
                    computeShader = myShader;
                }
            }*/
        }

        // Start is called before the first frame update
        private void Start()
        {
            Init();
        }

        private void OnValidate()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (!update) return;
            if (!dataProvider) return;

            dataList.Clear();

            dataProvider.GetData(targetMap, dataList);

            CheckConsistency();

            TransferData();
        }

        [CCUIButton]
        public void UpdateData()
        {
            Update();
        }
    }
}
