using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WoodworkBerserkServer.Message;

namespace WoodworkBerserkServer.Server
{
    class WBServer
    {
        int ownPort;
        UdpClient c;
        IClientMessageCallback listener;
        bool keepListening;
        Thread t;

        public WBServer(int ownPort)
        {
            this.ownPort = ownPort;
            c = new UdpClient(ownPort);
        }
        public void StartListening(IClientMessageCallback listener)
        {
            this.listener = listener;
            keepListening = true;
            t = new Thread(Receive);
            t.Start();
        }
        public void StopListening()
        {
            keepListening = false;
            // Is this necessary? Feels safer.
            t.Join();
        }
        public void Send(ServerMessage smsg, IPEndPoint dest)
        {
            c.SendAsync(smsg.Bytes(), smsg.NumBytes(), dest);
        }
        private void Receive()
        {
            while (keepListening)
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    listener.Call(ClientMessage.Parse(c.Receive(ref remoteIpEndPoint), remoteIpEndPoint));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
