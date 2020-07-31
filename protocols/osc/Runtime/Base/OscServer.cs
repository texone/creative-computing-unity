// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

#if UNITY_EDITOR
#define OSC_SERVER_LIST
#endif

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#if OSC_SERVER_LIST
using System.Collections.Generic;
using System.Collections.ObjectModel;
[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("jp.keijiro.osc-jack.Editor")] 
#endif

namespace OscJack
{
    public sealed class OscServer : IDisposable
    {
        #region Public Properties And Methods

        public OscMessageDispatcher MessageDispatcher { get; private set; }

        public OscServer(int listenPort)
        {
            MessageDispatcher = new OscMessageDispatcher();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) {ReceiveTimeout = 100};

            // On some platforms, it's not possible to cancel Receive() by
            // just calling Close() -- it'll block the thread forever!
            // Therefore, we heve to set timeout to break ServerLoop.

            _socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));

            _thread = new Thread(ServerLoop);
            _thread.Start();

            #if OSC_SERVER_LIST
            Servers.Add(this);
            #endif
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            #if OSC_SERVER_LIST
            Servers?.Remove(this);
#endif
        }

        #endregion

        #region IDispose implementation

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (!disposing) return;
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            if (_thread != null)
            {
                _thread.Join();
                _thread = null;
            }

            MessageDispatcher = null;
        }

        ~OscServer()
        {
            Dispose(false);
        }

        #endregion

        #region For editor functionalities

        #if OSC_SERVER_LIST

        private static readonly List<OscServer> Servers = new List<OscServer>(8);
        private static ReadOnlyCollection<OscServer> _serversReadOnly;

        internal static IEnumerable<OscServer> ServerList => _serversReadOnly ?? (_serversReadOnly = new ReadOnlyCollection<OscServer>(Servers));

#endif

        #endregion

        #region Private Objects And Methods

        private Socket _socket;
        private Thread _thread;
        private bool _disposed;

        private void ServerLoop()
        {
            var parser = new OscPacketParser(MessageDispatcher);
            var buffer = new byte[4096];

            while (!_disposed)
            {
                try
                {
                    var dataRead = _socket.Receive(buffer);
                    if (!_disposed && dataRead > 0)
                        parser.Parse(buffer, dataRead);
                }
                catch (SocketException)
                {
                    // It might exited by timeout. Nothing to do.
                }
                catch (ThreadAbortException)
                {
                    // Abort silently.
                }
                catch (Exception e)
                {
                #if UNITY_EDITOR || UNITY_STANDALONE
                    if (!_disposed) UnityEngine.Debug.Log(e);
                #else
                    if (!_disposed) System.Console.WriteLine(e);
                #endif
                    break;
                }
            }
        }

        #endregion
    }
}
