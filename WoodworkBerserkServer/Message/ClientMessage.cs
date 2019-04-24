using System;
using System.Net;
using System.Text;

namespace WoodworkBerserkServer.Message
{
    enum ClientMessageType : byte
    {
        Invalid = 0,
        Connect = 1,
        Disconnect = 2,
        Pong = 3,
        Command = 4
    }
    abstract class ClientMessage
    {
        IPEndPoint origin;
        public static ClientMessage Parse(byte[] data, IPEndPoint source)
        {
            ClientMessageType type = (ClientMessageType)Enum.ToObject(typeof(ClientMessageType), data[0]);
            ClientMessage msg;
            switch (type)
            {
                case ClientMessageType.Connect:
                    msg = new ClientMessageConnect();
                    break;
                case ClientMessageType.Disconnect:
                    msg = new ClientMessageDisconnect();
                    break;
                case ClientMessageType.Pong:
                    msg = new ClientMessagePong();
                    break;
                case ClientMessageType.Command:
                    if (data.Length == 9)
                    {
                        msg = new ClientMessageCommand();
                        ((ClientMessageCommand)msg).ConnectionId = BitConverter.ToInt32(data, 1);
                        ((ClientMessageCommand)msg).PlayerAction = (PlayerAction)BitConverter.ToInt32(data, 5);

                    }
                    else
                    {
                        msg = new ClientMessageInvalid();
                    }
                    break;
                default:
                    msg = new ClientMessageInvalid();
                    break;
            }

            msg.SetOrigin(source);

            return msg;
        }

        private static object PlayerAction(ClientMessageCommand msg)
        {
            throw new NotImplementedException();
        }

        public static String ConvertToString(ClientMessage msg)
        {
            byte[] data = msg.Bytes();
            StringBuilder sb = new StringBuilder();
            sb.Append(msg.GetClientMessageType().ToString() + "{" + data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                sb.Append(", " + data[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
        public void SetOrigin(IPEndPoint origin)
        {
            this.origin = origin;
        }
        public IPEndPoint GetOrigin()
        {
            return origin;
        }
        abstract public ClientMessageType GetClientMessageType();
        abstract public byte[] Bytes();
        abstract public int NumBytes();
    }
    class ClientMessageInvalid : ClientMessage
    {
        private byte[] data;
        public ClientMessageInvalid()
        {
            data = new byte[] { (byte)ClientMessageType.Invalid };
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Invalid;
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
    class ClientMessageConnect : ClientMessage
    {
        private byte[] data;
        public ClientMessageConnect()
        {
            data = new byte[] { (byte)ClientMessageType.Connect };
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Connect;
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
    class ClientMessageDisconnect : ClientMessage
    {
        private byte[] data;
        public ClientMessageDisconnect()
        {
            data = new byte[] { (byte)ClientMessageType.Disconnect };
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Disconnect;
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
    class ClientMessagePong : ClientMessage
    {
        private byte[] data;
        public ClientMessagePong()
        {
            data = new byte[] { (byte)ClientMessageType.Pong };
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Pong;
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
    class ClientMessageCommand : ClientMessage
    {
        public int ConnectionId { get; set; }
        public PlayerAction PlayerAction { get; set; }
        public ClientMessageCommand()
        {
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Command;
        }
        public override byte[] Bytes()
        {
            byte[] data = new byte[9];
            data[0] = (byte)ClientMessageType.Command;

            byte[] idBytes = BitConverter.GetBytes(ConnectionId);
            data[1] = idBytes[0];
            data[2] = idBytes[1];
            data[3] = idBytes[2];
            data[4] = idBytes[3];

            byte[] actionBytes = BitConverter.GetBytes((int)PlayerAction);
            data[5] = actionBytes[0];
            data[6] = actionBytes[1];
            data[7] = actionBytes[2];
            data[8] = actionBytes[3];

            return data;
        }
        public override int NumBytes()
        {
            return 9;
        }
    }
}
