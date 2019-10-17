using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

namespace cc.creativecomputing.util
{
    public class CCTextureToBytes : MonoBehaviour
    {
        public RenderTexture dataTexture;
        Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();

        private Texture2D texture;

        private NativeArray<Color32> bytes;

        /*
        public Color32[] Bytes()
        {
            DataToTexture();
            bytes = texture.GetPixels32();
            return bytes;
        }*/

        public NativeArray<Color32> Bytes()
        {
            return bytes;
        }

        public int Width()
        {
            return dataTexture.width;
        }

        public int Height()
        {
            return dataTexture.height;
        }


        private void DataToTexture()
        {

            //Debug.Log(request.done);
            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;

            // Set the supplied RenderTexture as the active one
            RenderTexture.active = dataTexture;

            // Create a new Texture2D and read the RenderTexture image into it
            if (texture == null || texture.width != dataTexture.width || texture.height != dataTexture.height)
            {
                texture = new Texture2D(dataTexture.width, dataTexture.height);
            }

            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

            // Restorie previously active render texture
            RenderTexture.active = currentActiveRT;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_requests.Count < 8)
                _requests.Enqueue(AsyncGPUReadback.Request(dataTexture));
            else
                Debug.Log("Too many requests.");

            while (_requests.Count > 0)
            {
                var req = _requests.Peek();

                if (req.hasError)
                {
                   // Debug.Log("GPU readback error detected.");
                    _requests.Dequeue();
                }
                else if (req.done)
                {
                     bytes = req.GetData<Color32>();

                    
                       // Debug.Log("Handel "+ _requests.Count);
                    

                    _requests.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
