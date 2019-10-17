using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cc.creativecomputing.effects
{
    [ExecuteAlways]
    public class CCEffectBlendAnalyzer : MonoBehaviour
    {
        public CCEffects effects;

        private Material mat;
        [Range(0, 1)]
        public float backgroundAlpha = 1;

        private void OnValidate()
        {
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        void RenderBlends()
        {
            effects.effectDatas.ForEach(data => {
                GL.Begin(GL.LINE_STRIP);
                GL.Color(Color.red);
                for (var i = 0; i <= 500; i++)
                {
                    var x = (float)i / 500;
                    var y = effects.EffectBlend(data, x);
                    GL.Vertex3(x, y, 0);
                }
                GL.End();
            });

        }

        // Will be called from camera after regular rendering is done.
        void OnRenderObject()
        {
            if (effects == null) return;
            if (!mat)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things. In this case, we just want to use
                // a blend mode that inverts destination colors.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                mat.hideFlags = HideFlags.HideAndDontSave;
                // Set blend mode to invert destination colors.
                // mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                // mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                // Turn off backface culling, depth writes, depth test.
                mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                mat.SetInt("_ZWrite", 0);
                mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }

            GL.PushMatrix();
            GL.LoadOrtho();

            // activate the first shader pass (in this case we know it is the only pass)
            mat.SetPass(0);
            // draw a quad over whole screen
            GL.Begin(GL.QUADS);
            GL.Color(new Color(1.0f, 1.0f, 1.0f, backgroundAlpha));
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(0, 0.25f, 0);
            GL.Vertex3(1, 0.25f, 0);
            GL.Vertex3(0, 0.5f, 0);
            GL.Vertex3(1, 0.5f, 0);
            GL.Vertex3(0, 0.75f, 0);
            GL.Vertex3(1, 0.75f, 0);
            GL.Vertex3(0, 1f, 0);
            GL.Vertex3(1, 1f, 0);
            GL.End();

            RenderBlends();
            //RenderHistory(val, Color.black, 0, 0.25f);
            //RenderHistory(vel, Color.red, 0.25f, 0.5f);
            //RenderHistory(acc, Color.green, 0.5f, 0.75f);
            //RenderHistory(jer, Color.blue, 0.75f, 1.0f);

            GL.PopMatrix();
        }

    }
}
