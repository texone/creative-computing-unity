using UnityEngine;

namespace cc.creativecomputing.util
{
    [ExecuteAlways]
    public class CCGradientToTexture : MonoBehaviour
    {

        public Gradient gradient;

        public Texture2D texture;

        void OnValidate()
        {
            CalculateGradient();
        }

        void Start()
        {
            CalculateGradient();
        }

        void CalculateGradient()
        {
            if (texture == null) return;
            
            var colors = new Color[texture.width];
            for (int x = 0; x < texture.width; x++)
            {
                float d = x / (float)texture.width;
                colors[x] = gradient.Evaluate(d);
            }
            texture.SetPixels(colors);
            texture.Apply();
        }
    }
}

