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
        public byte[] Data { get; set; }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(((ServerMessageType)Data[0]).ToString() + "{" + Data[0]);
            for (int i = 1; i < Data.Length; i++)
            {
                sb.Append(", " + Data[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
        abstract public ServerMessageType GetServerMessageType();
    }
    class ServerMessageInvalid : ServerMessage
    {
        public ServerMessageInvalid()
        {
            Data = new byte[] { (byte)ServerMessageType.Invalid };
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Invalid;
        }
    }

    class ServerMessageDisconnect : ServerMessage
    {
        public ServerMessageDisconnect()
        {
            Data = new byte[] { (byte) ServerMessageType.Disconnect };
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Disconnect;
        }
    }

    class ServerMessageUpdate : ServerMessage
    {
        public ServerMessageUpdate(int playerId, int mapWidth, int mapHeight, int[] terrain, int[] entitiesData)
        {
            // when replacing map stuff, make it like this:
            // playerId + mapId + entities
            // figure out how many bytes entities have.
            // might be just id and 2d location.

            // type, playerId, mapWidth, mapHeight, terrain, entities
            // 1 + sizeof(int) * (1 + 1 + 1 + terrain.Length + entitiesData.Length);
            int dataSize = 1 + sizeof(int) * (1 + 1 + 1 + terrain.Length + entitiesData.Length);
            byte[] data = new byte[dataSize];
            Data[0] = (byte)ServerMessageType.Update;

            byte[] playerIdBytes = BitConverter.GetBytes(playerId);
            Data[1] = playerIdBytes[0];
            Data[2] = playerIdBytes[1];
            Data[3] = playerIdBytes[2];
            Data[4] = playerIdBytes[3];

            byte[] mapWidthBytes = BitConverter.GetBytes(mapWidth);
            Data[5] = mapWidthBytes[0];
            Data[6] = mapWidthBytes[1];
            Data[7] = mapWidthBytes[2];
            Data[8] = mapWidthBytes[3];

            byte[] mapHeightBytes = BitConverter.GetBytes(mapHeight);
            Data[9] = mapHeightBytes[0];
            Data[10] = mapHeightBytes[1];
            Data[11] = mapHeightBytes[2];
            Data[12] = mapHeightBytes[3];

            int loc = 13;
            foreach (int t in terrain)
            {
                byte[] tBytes = BitConverter.GetBytes(t);
                Data[loc] = tBytes[0];
                Data[loc + 1] = tBytes[1];
                Data[loc + 2] = tBytes[2];
                Data[loc + 3] = tBytes[3];
                loc += 4;
            }

            foreach (int e in entitiesData)
            {
                byte[] eBytes = BitConverter.GetBytes(e);
                Data[loc] = eBytes[0];
                Data[loc + 1] = eBytes[1];
                Data[loc + 2] = eBytes[2];
                Data[loc + 3] = eBytes[3];
                loc += 4;
            }
        }
        public override ServerMessageType GetServerMessageType()
        {
            return ServerMessageType.Update;
        }
    }
}
