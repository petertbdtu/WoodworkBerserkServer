using System;
using WoodworkBerserkServer.Server;
using WoodworkBerserkServer.Message;

namespace WoodworkBerserkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WBService service = new WBService();
            service.start();
        }
    }
}
