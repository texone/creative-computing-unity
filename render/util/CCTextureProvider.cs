using UnityEngine;

namespace cc.creativecomputing.render
{
    public abstract class CCTextureProvider :MonoBehaviour
    {
        public abstract RenderTexture Texture();
    }
}