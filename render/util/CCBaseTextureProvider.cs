using System.Collections;
using System.Collections.Generic;
using cc.creativecomputing.render;
using UnityEngine;

namespace cc.creativecomputing.util{
public class CCBaseTextureProvider : CCTextureProvider
{
    public Texture texture;

    public override Texture Texture()
    {
        return texture;
    }
}
}