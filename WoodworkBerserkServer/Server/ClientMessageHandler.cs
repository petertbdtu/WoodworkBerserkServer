using System.Collections.Generic;
using WoodworkBerserkServer.Message;

namespace WoodworkBerserkServer.Server
{
    class ClientMessageHandler : IClientMessageCallback
    {
        private object messagesLock = new object();
        List<ClientMessage> messages = new List<ClientMessage>();

        public void Call(ClientMessage msg)
        {
            lock (messagesLock)
            {
                messages.Add(msg);
            }
        }

        public ClientMessage[] GetClientMessages()
        {
            lock (messagesLock)
            {
                ClientMessage[] msgs = messages.ToArray();
                messages.Clear();
                return msgs;
            }
        }
    }
}
