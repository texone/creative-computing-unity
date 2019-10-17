using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using UnityEngine;

using ArtNet.Sockets;
using ArtNet.Packets;
using ArtNet.Enums;


namespace cc.creativecomputing.artnet
{
    public class CCArtNetIO : MonoBehaviour
    {
        private static IPAddress FindFromHostName(string hostname)
        {
            var address = IPAddress.None;
            try
            {
                if (IPAddress.TryParse(hostname, out address))
                {
                    return address;
                }

                var addresses = Dns.GetHostAddresses(hostname);
                for (var i = 0; i < addresses.Length; i++)
                {
                    if (addresses[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = addresses[i];
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarningFormat(
                    "Failed to find IP for :\n host name = {0}\n exception={1}",
                    hostname, e);
            }
            return address;
        }
        
        public bool useBroadcast;
        public string remoteIP = "localhost";
        public string localIP = "localhost";
        public int universes = 1;
        public int startUniverse = 0;
        public int deviceID = 0;
        IPEndPoint remote;
        
        public bool isServer;

        ArtNetSocket artnet;
        

        private ArtNetDmxPacket[] myPackets;

        public event EventHandler<CCArtNetDMXPaket> ReceiveDMX;

        void Start()
        {
            artnet = new ArtNetSocket();
            InitPackets();
            IPAddress myAddress1 = FindFromHostName(localIP);
            Debug.Log(myAddress1);
            if (isServer)
                artnet.Open(FindFromHostName(localIP), null);

            artnet.NewPacket += (object sender, NewPacketEventArgs<ArtNetPacket> e) =>
            {
                if (e.Packet.OpCode != ArtNetOpCodes.Dmx) return;

                if (ReceiveDMX == null) return;
          
                var packet = e.Packet as ArtNetDmxPacket;
                var universe = packet.Universe;
                byte[] data = packet.DmxData.ToArray();
                ReceiveDMX(this, new CCArtNetDMXPaket(universe, data));
            };

            if (remoteIP != null && (!useBroadcast || !isServer))
            {
                IPAddress myAddress = FindFromHostName(remoteIP);
                if (myAddress == null) return;
                remote = new IPEndPoint(myAddress, ArtNetSocket.Port);
            }


        }

        private void Update()
        {
            /*
            foreach (var r in receiver)
            {
                r.Update();
            }*/
        }


        private void OnDestroy()
        {
            if (artnet == null) return;
            artnet.Close();
        }


        public void Send()
        {
            if (remote == null) return;
            foreach(ArtNetDmxPacket myPacket in myPackets)
            {
                if (useBroadcast && isServer)
                    artnet.Send(myPacket);
                else
                    artnet.Send(myPacket, remote);
            }
           
        }

        public string Subnet()
        {
            return remoteIP.Substring(0, remoteIP.LastIndexOf('.'));
        }

        /*
        public void Send(short universe, byte[] dmxData)
        {
            myPackets.Universe = universe;
            System.Buffer.BlockCopy(dmxData, 0, myPackets.DmxData, 0, dmxData.Length);
            if (useBroadcast && isServer)
                artnet.Send(myPackets);
            else
                artnet.Send(myPackets, remote);
        }*/

        private void InitPackets()
        {
            myPackets = new ArtNetDmxPacket[universes];
            for (short i = 0; i < universes; i++)
            {
                myPackets[i] = new ArtNetDmxPacket
                {
                    Universe = (short)(startUniverse + i),
                    DmxData = new byte[512]
                };
            }
        }

        public void ResetData()
        {
            for (short i = 0; i < universes; i++)
            {
                for (short j = 0; j < 512; j++)
                {
                    myPackets[i].DmxData[j] = 0;
                }
            }
        }

        private void OnValidate()
        {
            /*
            foreach(var r in receiver)
            {
                r.Validate();
            }
            */
            InitPackets();
        }

        public void SetData(int theUniverse, int theChannel, byte theValue)
        {
            if (myPackets == null) return;
            if (theUniverse > myPackets.Length) return;
            if (theChannel > 512) return;

            myPackets[theUniverse].DmxData[theChannel] = theValue;
        }

        public void SetData(int theUniverse, int theChannel, byte theR, byte theG, byte theB)
        {
            SetData(theUniverse, theChannel, theR);
            SetData(theUniverse, theChannel + 1, theG);
            SetData(theUniverse, theChannel + 2, theB);
        }

        public void SetData(int theUniverse, int theChannel, byte theR, byte theG, byte theB, byte theA)
        {
            SetData(theUniverse, theChannel, theR);
            SetData(theUniverse, theChannel + 1, theG);
            SetData(theUniverse, theChannel + 2, theB);
            SetData(theUniverse, theChannel + 3, theA);
        }

        public void SetDataRGBA(int theUniverse, int theChannel, Color32 theColor) {
            SetData(theUniverse, theChannel, theColor.r, theColor.g, theColor.b, theColor.a);
        }



    }
}
