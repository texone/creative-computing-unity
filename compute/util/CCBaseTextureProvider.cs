using UnityEngine;

namespace compute.util{
public class CCBaseTextureProvider : CCTextureProvider
{
    public Texture texture;

    public override Texture Texture()
    {
        return texture;
    }
}
}