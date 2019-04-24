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
        public static ServerMessage Parse(byte[] data)
        {
            ServerMessageType type = (ServerMessageType)Enum.ToObject(typeof(ServerMessageType), data[0]);
            ServerMessage msg;
            switch (type)
            {
                /*case ServerMessageType.Disconnect:
                    msg = new ServerMessage();
                    break;
                case ServerMessageType.Ping:
                    msg = new ServerMessage();
                    break;
                *//*case ServerMessageType.FullUpdate:
                    msg = new ServerMessage();
                    break;*/
                /*case ServerMessageType.EntityUpdate:
                    msg = new ServerMessage();
                    break;*/
                default:
                    msg = new ServerMessageInvalid();
                    break;
            }

            return msg;
        }
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
}
