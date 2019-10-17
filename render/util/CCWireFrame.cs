using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCWireFrame : MonoBehaviour
{
    // Attach this script to a camera, this will make it render in wireframe
    void OnPreRender()
    {
        GL.wireframe = true;
    }

    void OnPostRender()
    {
        GL.wireframe = false;
    }
}
