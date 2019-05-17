﻿using System;
using System.Collections.Generic;
using WoodworkBerserkServer.Server;
using WoodworkBerserkServer.Message;
using System.Net;
using System.Text;

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

            Dictionary<int, WBPlayer> players = new Dictionary<int, WBPlayer>();
            // playerId and player connection endpoint
            Dictionary<int, IPEndPoint> clients = new Dictionary<int, IPEndPoint>();
            // playerId and corresponding connection timeout
            Dictionary<int, int> timeouts = new Dictionary<int, int>();

            int connects = 0;
            running = true;
            while (running)
            {
                Console.WriteLine("tick");
                
                ClientMessage[] clientMessages = messageHandler.GetClientMessages();
                foreach (ClientMessage clientMessage in clientMessages)
                {
                    if (clientMessage.GetClientMessageType() == ClientMessageType.Connect)
                    {
                        // Add player to list of connections
                        // TODO replace connects with player ID from database.
                        // TODO check if player is already connected
                        if ( clients.TryAdd(connects, clientMessage.Origin) )
                        {
                            players.Add(connects, new WBPlayer(connects, 1, 1, 1));
                            timeouts.Add(connects, 10);
                            Console.WriteLine("added connection with id="+connects);
                            connects++;
                        }

                    }
                    else if (true) // TODO compare ID in packet with connected client? hell, maybe don't bother
                    {
                        switch (clientMessage.GetClientMessageType())
                        {
                            case ClientMessageType.Disconnect:
                                // Remove player from list of connections
                                // Probably also check if stuff matches like in command
                                //clients.Remove(0);
                                Console.WriteLine("received disconnect!");
                                break;
                            case ClientMessageType.Command:
                                Console.WriteLine("received command! " + ((ClientMessageCommand)clientMessage).PlayerAction.ToString());
                                //TODO timeouts.Add(clientMessage.PlayerId(), 10);
                                break;
                            case ClientMessageType.Invalid:
                                Console.WriteLine("received invalid");
                                break;
                            default:
                                Console.WriteLine("what");
                                break;
                        }
                    }
                }

                /*
                 * UPDATE PLAYERS
                 */
                foreach (WBPlayer player in players.Values)
                {
                    IPEndPoint dest;
                    if (clients.TryGetValue(player.Id, out dest))
                    {
                        // TODO generate state uniquely for each player
                        ServerMessage sm = new ServerMessageUpdate(player.Id, 3, 3,
                            new int[] { 0,0,0, 0,0,0, 0,0,0 }, // terrain
                            new int[] { 0,0,2,2, 1,1,3,3, 2,1,4,4 }  // entities
                            );

                        server.Send(sm, dest);
                        Console.WriteLine("sent update to player with id="+ player.Id);
                    }

                    int currTimeout;
                    if (timeouts.TryGetValue(player.Id, out currTimeout))
                        timeouts[player.Id] = currTimeout-1;
                }

                System.Threading.Thread.Sleep(2000);
            }
        }

        public void stop()
        {
            running = false;
        }
    }
}
