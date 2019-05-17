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
        Command = 3
    }
    abstract class ClientMessage
    {
        public byte[] Data { get; set; }
        public int AssumedPlayerId { get; set; }
        public IPEndPoint Origin { get; set; }
        public static ClientMessage Parse(byte[] data, IPEndPoint source)
        {
            ClientMessageType type = (ClientMessageType)Enum.ToObject(typeof(ClientMessageType), data[0]);
            int apid = BitConverter.ToInt32(data, 1);
            ClientMessage msg;
            switch (type)
            {
                case ClientMessageType.Connect:
                    msg = new ClientMessageConnect(data);
                    break;
                case ClientMessageType.Disconnect:
                    msg = new ClientMessageDisconnect();
                    break;
                case ClientMessageType.Command:
                    if (data.Length == 9)
                    {
                        msg = new ClientMessageCommand(data);
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

            msg.Data = data;
            msg.AssumedPlayerId = apid;
            msg.Origin = source;

            Console.WriteLine(msg.ToString());
            return msg;
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(((ClientMessageType)Data[0]).ToString() + "{" + Data[0]);
            for (int i = 1; i < Data.Length; i++)
            {
                sb.Append(", " + Data[i]);
            }
            sb.Append("}");
            return sb.ToString();
        }
        abstract public ClientMessageType GetClientMessageType();
    }
    class ClientMessageInvalid : ClientMessage
    {
        public ClientMessageInvalid() { }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Invalid;
        }
    }
    class ClientMessageConnect : ClientMessage
    {
        // TODO consider using SecureString... as if.
        public String Username { get; }
        public String Password { get; }
        public ClientMessageConnect(byte[] data)
        {
            // Parse string from data.
            // byte[0] is type
            // byte[1-4] is playerId
            // byte[5-8] is info about where username and password splits
            int splitLoc = BitConverter.ToInt32(data, 5);
            
            byte[] nameData = new byte[splitLoc-9];
            byte[] passData = new byte[data.Length-splitLoc];
            Console.WriteLine("len="+data.Length+", namestart="+9+" split="+(nameData.Length+9)+" passstop="+(splitLoc+passData.Length) );
            Array.Copy(data, 9, nameData, 0, nameData.Length);
            Array.Copy(data, splitLoc, passData, 0, passData.Length);

            Username = Encoding.ASCII.GetString(nameData);
            Password = Encoding.ASCII.GetString(passData);
            Console.WriteLine("read UN:"+Username+" PW:"+Password);
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Connect;
        }
    }
    class ClientMessageDisconnect : ClientMessage
    {
        public ClientMessageDisconnect() { }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Disconnect;
        }
    }
    class ClientMessageCommand : ClientMessage
    {
        public PlayerAction PlayerAction { get; set; }
        public ClientMessageCommand(byte[] data)
        {
            PlayerAction = (PlayerAction)BitConverter.ToInt32(data, 5);
        }
        public override ClientMessageType GetClientMessageType()
        {
            return ClientMessageType.Command;
        }
    }
}
