using System.Collections;
using System.Collections.Generic;
using protocols.tuio;
using UnityEngine;

public class CCTuioDebugger : MonoBehaviour
{

    public CCTuioEventReceiver tuioReceiver;
    // Start is called before the first frame update
    private void Start()
    {
        if (!tuioReceiver) return;
        
        tuioReceiver.OnAdd(c =>Debug.Log("ADD:" + c ));
        tuioReceiver.OnUpdate(c =>Debug.Log("UPDATE:" + c ));
        tuioReceiver.OnRemove(c =>Debug.Log("REMOVE:" + c ));
    }

    private static Material _lineMaterial;

    private static void CreateLineMaterial()
    {
        if (_lineMaterial) return;
        
        // Unity has a built-in shader that is useful for drawing
        // simple colored things.
        var shader = Shader.Find("Hidden/Internal-Colored");
        _lineMaterial = new Material(shader) {hideFlags = HideFlags.HideAndDontSave};
        // Turn on alpha blending
        _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn backface culling off
        _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        // Turn off depth writes
        _lineMaterial.SetInt("_ZWrite", 0);
    }
    public int lineCount = 100;
    public float radius = 3.0f;
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (!tuioReceiver) return;
        
        CreateLineMaterial();
        // Apply the line material
        _lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        foreach (var tuioReceiverActiveCursor in tuioReceiver.ActiveCursors)
        {
            var x = tuioReceiverActiveCursor.X;
            var y = tuioReceiverActiveCursor.Y;
            // Draw lines
            GL.Begin(GL.LINE_STRIP);
            for (var i = 0; i <= lineCount; ++i)
            {
                var a = i / (float)lineCount;
                var angle = a * Mathf.PI * 2;
                // Vertex colors change from red to green
                GL.Color(new Color(a, 1 - a, 0, 0.8F));
                // Another vertex at edge of circle
                GL.Vertex3(Mathf.Cos(angle) * radius + x, Mathf.Sin(angle) * radius + y, 0);
            }
            GL.End();
        }
        
        GL.PopMatrix();
    }
}
