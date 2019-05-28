using System;
using System.Collections.Generic;
using WoodworkBerserkServer.Server;
using WoodworkBerserkServer.Message;
using System.Net;
using System.Text;
using WoodworkBerserk.Controllers;

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

        DatabaseConnector dbc;
        WBServer server;
        ClientMessageHandler messageHandler;
        bool running = false;
        
        public void start()
        {
            dbc = new DatabaseConnector();
            messageHandler = new ClientMessageHandler();
            server = new WBServer(PORT);
            server.StartListening(messageHandler);

            Dictionary<int, WBPlayer> players = new Dictionary<int, WBPlayer>();
            // playerId and player connection endpoint
            Dictionary<int, IPEndPoint> clients = new Dictionary<int, IPEndPoint>();
            // playerId and corresponding connection timeout
            Dictionary<int, int> timeouts = new Dictionary<int, int>();
            
            running = true;
            while (running)
            {
                Console.WriteLine("tick");
                
                ClientMessage[] clientMessages = messageHandler.GetClientMessages();
                foreach (ClientMessage clientMessage in clientMessages)
                {
                    if (clientMessage.GetClientMessageType() == ClientMessageType.Connect)
                    {
                        string username = ((ClientMessageConnect)clientMessage).Username;
                        string password = ((ClientMessageConnect)clientMessage).Password;
                        Console.WriteLine("logged in username: "+username+" password: "+password);

                        // checks if username and password is a registered player
                        // also checks if player is already logged in
                        if (dbc.Authenticate(username, password) && !dbc.getPlayer_active(username, password))
                        {
                            int playerId = dbc.getPlayer_Id(username, password);
                            if (clients.TryAdd(playerId, clientMessage.Origin))
                            {
                                players.Add(playerId, new WBPlayer(playerId, 1, 200, 200));
                                timeouts.Add(playerId, 10);
                                Console.WriteLine("added connection with id=" + playerId);
                            }
                        }
                        else
                        {
                            //wrong login credentials give information to client
                            server.Send(new ServerMessageUpdate(-1, 0, 0, new int[0], new int[0]), clientMessage.Origin);
                        }
                    }
                    else if (true) // TODO compare ID in packet with connected client? hell, maybe don't bother
                    {
                        switch (clientMessage.GetClientMessageType())
                        {
                            case ClientMessageType.Disconnect:
                                Console.WriteLine("received disconnect!");
                                //set player_active to false in database
                                int disconnectedPlayerId = clientMessage.AssumedPlayerId;
                                dbc.updatePlayer_active(disconnectedPlayerId, false);
                                // TODO Save player to database and remove from game.
                                clients.Remove(disconnectedPlayerId);
                                timeouts.Remove(disconnectedPlayerId);
                                
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
                            new int[] { player.Id,0,200,200, 1,1,3,3, 2,1,4,4 }  // entities
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
