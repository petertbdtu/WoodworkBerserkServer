using System;

namespace WoodworkBerserkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WSService service = new WSService();
            WSSocketListener.service = service;
            WSSocketListener.StartListening();
        }
    }
}
