using System;
using System.Collections.Generic;
using System.Text;
using WoodworkBerserkServer.Server;
using WoodworkBerserkServer.Message;
using System.Net;

namespace WoodworkBerserkServer
{
    enum PlayerAction
    {
        Nothing,
        MoveUp,
        MoveDown,
        MoveRight,
        MoveLeft,
        Interact
    };
    class WBService
    {
        public static readonly int PORT = 19567;

        WBServer server;
        ClientMessageHandler messageHandler;
        
        public void start()
        {
            messageHandler = new ClientMessageHandler();
            server = new WBServer(PORT);
            server.StartListening(messageHandler);

            // playerId and player connection endpoint
            Dictionary<int, IPEndPoint> clients = new Dictionary<int, IPEndPoint>();

            int connects = 0;
            while (true)
            {
                Console.WriteLine("tick");
                
                ClientMessage[] clientMessages = messageHandler.GetClientMessages();
                foreach (ClientMessage clientMessage in clientMessages)
                {
                    // VERIFY MESSAGE WITH CONNECTION ID AND SEE IF IT MATCHES.
                    // NEED TO ADD CONNECTION ID TO MESSAGE.
                    // IGNORE IF IT DOESN'T MATCH (UNLESS IT'S A CONNECT)
                    Console.WriteLine(ClientMessage.ConvertToString(clientMessage));
                    switch (clientMessage.GetClientMessageType())
                    {
                        case ClientMessageType.Connect:
                            // Add player to list of connections
                            // testing, please only connect once.
                            // TODO get actual player id instead of 0.
                            clients.Add(connects++, clientMessage.GetOrigin());
                            Console.WriteLine("received connect!");
                            break;
                        case ClientMessageType.Disconnect:
                            // Remove player from list of connections
                            // Probably also check if stuff matches like in command :)
                            //clients.Remove(0);
                            Console.WriteLine("received disconnect!");
                            break;
                        case ClientMessageType.Pong:
                            // Refresh player timeout?
                            break;
                        case ClientMessageType.Command:
                            // Do shit
                            // Check if connectionID matches IPEndPoint in clients
                            Console.WriteLine("received command! "+((ClientMessageCommand)clientMessage).PlayerAction.ToString());
                            break;
                        case ClientMessageType.Invalid:
                            Console.WriteLine("received invalid");
                            break;
                        default:
                            Console.WriteLine("what");
                            break;
                    }
                }

                // TODO send data to players loop

                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
