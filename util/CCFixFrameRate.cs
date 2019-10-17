using UnityEngine;
using System.Collections;

public class CCFixFrameRate : MonoBehaviour
{

    public int captureRate = 30;

    private void OnEnable()
    {

        Time.captureFramerate = captureRate;
    }

    private void OnDisable()
    {

        Time.captureFramerate = 0;
    }
}
