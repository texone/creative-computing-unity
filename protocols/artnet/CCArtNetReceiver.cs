using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using ArtNet.Packets;

namespace cc.creativecomputing.artnet
{
    public class CCArtNetDMXPaket : EventArgs
    {
        public int universe;

        public byte[] data;

        public CCArtNetDMXPaket(int theUniverse, byte[] theData)
        {
            universe = theUniverse;
            data = theData;
        }

        public int GetInt(int theOffset)
        {
            return BitConverter.ToInt32(data, theOffset);
        }
    }
    
}