using System;
using System.Collections.Generic;
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
        bool running = false;
        
        public void start()
        {
            messageHandler = new ClientMessageHandler();
            server = new WBServer(PORT);
            server.StartListening(messageHandler);

            // playerId and player connection endpoint
            Dictionary<int, IPEndPoint> clients = new Dictionary<int, IPEndPoint>();

            int connects = 0;
            running = true;
            while (running)
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
                foreach (int pid in clients.Keys)
                {
                    IPEndPoint dest;
                    if (clients.TryGetValue(pid, out dest))
                    {
                        // TODO generate state uniquely for each player
                        ServerMessage sm = new ServerMessageUpdate(pid, 3, 3,
                            new int[] { 0,0,0, 0,0,0, 0,0,0 }, // terrain
                            new int[] { 0,0,2,2, 1,1,3,3, 2,1,4,4 }  // entities
                            );

                        server.Send(sm, dest);
                        Console.WriteLine("sent update to player with id="+pid);
                    }
                }

                System.Threading.Thread.Sleep(2000);
            }
        }

        public void stop()
        {
            // shouldn't be necessary to sync this, right?
            running = false;
        }
    }
}
