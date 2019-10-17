using UnityEngine;
using System.Collections;

public abstract class CCFilter : MonoBehaviour
{
    public bool bypass = true;
    [Range(0,60)]
    public float sampleRate = 5;

   
    /*
    public virtual void PrepareValues()
    {

    }*/

    protected internal int _myChannels = 1;

    public abstract float Process(int theChannel, float theData, float theDeltaTime);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
