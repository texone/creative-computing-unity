using UnityEngine;
using cc.creativecomputing.util;
using System.Collections.Generic;
using cc.creativecomputing.math.util;

namespace cc.creativecomputing.effects
{

    [AddComponentMenu("effects/analyzer")]
    [ExecuteAlways]
    public class CCDataAnalyzer : MonoBehaviour
    {
        public CCEffects effects;

        public int size = 100;
        
        public CCDataHistory val = new CCDataHistory();
        public CCDataHistory vel = new CCDataHistory();
        public CCDataHistory acc = new CCDataHistory();
        public CCDataHistory jer = new CCDataHistory();

        private List<float> lastVal = new List<float>();
        private List<float> lastVel = new List<float>();
        private List<float> lastAcc = new List<float>();

        private void Init()
        {
            if (effects == null) return;

            val.data = new List<List<float>>();
            vel.data = new List<List<float>>();
            acc.data = new List<List<float>>();
            jer.data = new List<List<float>>();

            effects.effectDatas.ForEach(data =>
            {
                while (lastVal.Count <= data.id)
                {
                    lastVal.Add(0);
                    lastVel.Add(0);
                    lastAcc.Add(0);
                }
                lastVal[data.id] = 0;
                lastVel[data.id] = 0;
                lastAcc[data.id] = 0;
            });
        }

        private void Awake()
        {
            Init();
        }

        private void OnValidate()
        {
            Init();
        }

        // Use this for initialization
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void AddData(CCDataHistory theHistory, List<float> theLast,  int theStream, float theValue)
        {
            theHistory.Add(theStream,theValue);
            if(theLast != null) theLast[theStream] = theValue;
            while (theHistory.data[theStream].Count > size) theHistory.data[theStream].RemoveAt(0);
            
        }

        private void LateUpdate()
        {
            
            effects.effectDatas.ForEach(data => {


                float angle = CCMath.Degrees(data.angle);
                float velocity = (angle - lastVal[data.id]) / Time.deltaTime;
                float acceleration = (velocity - lastVel[data.id]) / Time.deltaTime;
                float jerk = (acceleration - lastAcc[data.id]) / Time.deltaTime;


                AddData(val, lastVal, data.id, angle);
                AddData(vel, lastVel, data.id, velocity);
                AddData(acc, lastAcc, data.id, acceleration);
                AddData(jer, null,data.id, jerk);

                lastVal[data.id] = angle;
                lastVel[data.id] = velocity;
                lastAcc[data.id] = acceleration;
            });
        }

        private Material mat;
        [Range(0,1)]
        public float backgroundAlpha = 1;


        void RenderGraph(List<float> theList, Color theColor, float theY0, float theY1, float theMax)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(theColor);
            for (var i = 0; i < theList.Count; i++)
            {
                var x = (float)i / (theList.Count - 1);
                var theta = x;
                var y = CCMath.Blend(theY0, theY1, CCMath.Saturate(CCMath.Norm(theList[i], -theMax, theMax)));
                GL.Vertex3(x,y,0);
            }
            GL.End();
        }

        void RenderHistory(CCDataHistory theHistory, Color theColor, float theY0, float theY1)
        {
            for (var i = 0; i < theHistory.data.Count; i++)
            {
                RenderGraph(theHistory.data[i], theColor, theY0, theY1, theHistory.max);
            }
        }

        // Will be called from camera after regular rendering is done.
        void OnRenderObject()
        {
            
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

            RenderHistory(val, Color.black, 0, 0.25f);
            RenderHistory(vel, Color.red,  0.25f, 0.5f);
            RenderHistory(acc, Color.green, 0.5f, 0.75f);
            RenderHistory(jer, Color.blue, 0.75f, 1.0f);

            GL.PopMatrix();
        }
    }
}