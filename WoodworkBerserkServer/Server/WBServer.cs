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
            c.Close();
            t.Join();
        }
        public void Send(ServerMessage smsg, IPEndPoint dest)
        {
            c.SendAsync(smsg.Data, smsg.Data.Length, dest);
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
                catch (SocketException se)
                {
                    // apparently UDP has connections now
                    // clients are disconnected on a timeout (unless they send disconnect)
                    // since 10054 comes from a previous failed send
                    if (se.SocketErrorCode != SocketError.ConnectionReset)
                    {
                        throw se;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
