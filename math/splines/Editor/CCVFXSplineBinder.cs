using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.VFX.Utility;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace cc.creativecomputing.math.spline
{
    [AddComponentMenu("VFX/Property Binders/Spline Binder")]
    [VFXBinder("cc/spline to AttributeMap")]
    class CCVFXSplineBinder : VFXBinderBase
    {
        public enum AudioSourceMode
        {
            AudioSource,
            AudioListener
        }

        public string ResolutionProperty { 
            get => (string)m_ResolutionProperty;
            set => m_ResolutionProperty = value;
        }
        [VFXPropertyBinding("System.Int32"), SerializeField]
        protected ExposedProperty m_ResolutionProperty = "Resolution";

        public string TextureProperty { get { return (string)m_TextureProperty; } set { m_TextureProperty = value; } }
        [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
        protected ExposedProperty m_TextureProperty = "SpectrumTexture";

        public int Resolution = 512;
        public CCSpline[] splines = new CCSpline[8];

        private Texture2D m_Texture;
        private Color[] m_ColorCache;

        public override bool IsValid(VisualEffect component)
        {
            var hasData = splines != null && splines.Length > 0;
            var texture = component.HasTexture(TextureProperty);
            var count = component.HasUInt(ResolutionProperty);
            
            return hasData && texture && count;
        }

        private void UpdateTexture()
        {
            if (m_Texture == null || m_Texture.width != Resolution || m_Texture.height != splines.Length)
            {
                m_Texture = new Texture2D((int) Resolution, splines.Length, TextureFormat.RFloat, false);
                m_ColorCache = new Color[Resolution * splines.Length];
            }
            
            for (int s = 0; s < splines.Length;s++)
            {
                var spline = splines[s];
                if(spline == null)continue;

                var points = spline.Discretize(Resolution);
                for (var i = 0; i < Resolution; i++)
                {
                    var point = points[i];
                    m_ColorCache[s * splines.Length + i] = new Color(point.x, point.y, point.z, 0);
                }
            }

            m_Texture.SetPixels(m_ColorCache);
            m_Texture.name = "Splines" + splines.Length;
            m_Texture.Apply();
        }

        public override void UpdateBinding(VisualEffect component)
        {
            UpdateTexture();
            component.SetTexture(TextureProperty, m_Texture);
            component.SetInt(ResolutionProperty, Resolution);
        }

        public override string ToString()
        {
            return
                $"Spline : '{m_ResolutionProperty} resolution' -> {splines?.Length ?? 0} splines";
        }
    }
}