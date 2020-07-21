using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace cc.creativecomputing.artnet {
    public class CCArtnetSender : MonoBehaviour
    {

        private CCArtNetIO[] artnetIOs;

        public bool send = true;

        public List<string> localSubnets = new List<string>();

        public bool checkSubnet = true;

        public void FindDevices()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    var localIP = ip.ToString();
                    localSubnets.Add(localIP.Substring(0, localIP.LastIndexOf('.')));
                }
            }

            CCArtNetIO[] myIOS = GetComponents<CCArtNetIO>();
            int maxDevice = 0;
            foreach (CCArtNetIO myIO in myIOS)
            {
                maxDevice = System.Math.Max(maxDevice, myIO.deviceID);
            }
            artnetIOs = new CCArtNetIO[maxDevice + 1];
            foreach (CCArtNetIO myIO in myIOS)
            {
                artnetIOs[myIO.deviceID] = myIO;
            }
        }

        public void CreateDevices(string theIp, int theStart, int theDevices)
        {
            CCArtNetIO[] myIOS = GetComponents<CCArtNetIO>();
            int maxDevice = 0;
            foreach (CCArtNetIO myIO in myIOS)
            {
                maxDevice = System.Math.Max(maxDevice, myIO.deviceID);
            }
            artnetIOs = new CCArtNetIO[theDevices];
            for (int i = 0; i < theDevices;i++)
            {
                CCArtNetIO myIO = gameObject.AddComponent<CCArtNetIO>();
                myIO.deviceID = i;
                myIO.remoteIP = theIp + (theStart + i);
                artnetIOs[i] = myIO;
            }
        }

        public void SendDMX()
        {
            if (!send) return;
            foreach (CCArtNetIO myIO in artnetIOs)
            {
                if (checkSubnet && !localSubnets.Contains(myIO.Subnet())) continue;
                myIO.Send();
            }
        }

        public CCArtNetIO Device(int theDevice)
        {
            return artnetIOs[theDevice];
        }

        public CCArtNetIO[] Devices()
        {
            return artnetIOs;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }
        
    }
}
