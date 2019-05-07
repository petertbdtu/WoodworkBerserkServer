using System;
using System.Text;

namespace WoodworkBerserkServer.Message
{
    enum ServerMessageType : byte
    {
        Invalid = 0,
        Disconnect = 1,
        Update = 2 // used to confirm connection establishment as well.
    }
    abstract class ServerMessage
    {
        public static String ConvertToString(ServerMessage msg)
        {
            byte[] data = msg.Bytes();
            StringBuilder sb = new StringBuilder();
            sb.Append(msg.GetServerMessageType().ToString() + "{" + data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                sb.Append(", " + data[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
        abstract public ServerMessageType GetServerMessageType();
        abstract public byte[] Bytes();
        abstract public int NumBytes();
    }
    class ServerMessageInvalid : ServerMessage
    {
        private byte[] data;
        public ServerMessageInvalid()
        {
            data = new byte[] { (byte)ServerMessageType.Invalid };
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Invalid;
        }
        public override byte[] Bytes()
        {
            return data;
        }
        public override int NumBytes()
        {
            return data.Length;
        }
    }

    class ServerMessageDisconnect : ServerMessage
    {
        private byte[] data;
        public ServerMessageDisconnect()
        {
            this.data = new byte[] { (byte) ServerMessageType.Disconnect };
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Disconnect;
        }
        public override byte[] Bytes()
        {
            return data;
        }
        public override int NumBytes()
        {
            return data.Length;
        }
    }

    class ServerMessageUpdate : ServerMessage
    {
        private int playerId;
        // TODO move map data to client
        // int mapId { get; set } // replace map stuff below with this.
        private int mapWidth;
        private int mapHeight;
        private int[] terrain;
        private int[] entitiesData;
        public ServerMessageUpdate(int playerId, int mapWidth, int mapHeight, int[] terrain, int[] entities)
        {
            this.playerId = playerId;
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.terrain = terrain;
            this.entitiesData = entities;
        }

        public override byte[] Bytes()
        {
            Console.WriteLine("length:"+NumBytes());
            byte[] data = new byte[NumBytes()];
            data[0] = (byte)ServerMessageType.Update;

            byte[] playerIdBytes = BitConverter.GetBytes(playerId);
            data[1] = playerIdBytes[0];
            data[2] = playerIdBytes[1];
            data[3] = playerIdBytes[2];
            data[4] = playerIdBytes[3];
            
            byte[] mapWidthBytes = BitConverter.GetBytes(mapWidth);
            data[5] = mapWidthBytes[0];
            data[6] = mapWidthBytes[1];
            data[7] = mapWidthBytes[2];
            data[8] = mapWidthBytes[3];
            
            byte[] mapHeightBytes = BitConverter.GetBytes(mapHeight);
            data[9] = mapHeightBytes[0];
            data[10] = mapHeightBytes[1];
            data[11] = mapHeightBytes[2];
            data[12] = mapHeightBytes[3];

            int loc = 13;
            foreach (int t in terrain)
            {
                byte[] tBytes = BitConverter.GetBytes(t);
                data[loc] = tBytes[0];
                data[loc+1] = tBytes[1];
                data[loc+2] = tBytes[2];
                data[loc+3] = tBytes[3];
                loc += 4;
            }

            foreach (int e in entitiesData)
            {
                byte[] eBytes = BitConverter.GetBytes(e);
                data[loc] = eBytes[0];
                data[loc + 1] = eBytes[1];
                data[loc + 2] = eBytes[2];
                data[loc + 3] = eBytes[3];
                loc += 4;
            }

            return data;
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Update;
        }

        public override int NumBytes()
        {
            // when replacing map stuff, make it like this:
            // playerId + mapId + entities
            // figure out how many bytes entities have.
            // might be just id and 2d location.

            // type, playerId, mapWidth, mapHeight, terrain, entities
            return 1 + sizeof(int) * (1 + 1 + 1 + terrain.Length + entitiesData.Length);
        }
    }
}
