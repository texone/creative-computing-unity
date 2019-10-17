using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cc.creativecomputing.ui;

namespace cc.creativecomputing.simulation
{
    public class ReactiveDiffusion : MonoBehaviour
    {
        

        public CustomRenderTexture simulationTexture;
        public int _stepsPerFrame = 4;
        
        [CCUIButton]
        public void Init()
        {
            simulationTexture.Initialize();
        }

        // Start is called before the first frame update
        void Start()
        {
            simulationTexture.Initialize();
        }

        // Update is called once per frame
        [CCUIButton]
        void Update()
        {
            simulationTexture.Update(_stepsPerFrame);
        }
    }
}
