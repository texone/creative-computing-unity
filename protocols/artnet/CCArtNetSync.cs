using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace cc.creativecomputing.artnet
{
    public class CCArtNetSync : MonoBehaviour
    {

        private CCArtNetIO _myIO;

        public int currentMillis = 0;

        public PlayableDirector director;

        private Boolean _myIsUpdated = false;

        // Start is called before the first frame update
        void Start()
        {

            _myIO = GetComponent<CCArtNetIO>();
            _myIO.isServer = true;
            _myIO.ReceiveDMX += (object sender, CCArtNetDMXPaket dmx)=> {
                Array.Reverse(dmx.data);
                Debug.Log(dmx.data.Length);
                currentMillis = BitConverter.ToInt32(dmx.data, dmx.data.Length - 4);
                _myIsUpdated = true;
            };
        }
        
        // Update is called once per frame
        void Update()
        {
            if (director == null) return;
            if (!_myIsUpdated) return;
            
            director.time = currentMillis / 1000f;
            
        }
    }
}
