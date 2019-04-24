using WoodworkBerserkServer.Message;

namespace WoodworkBerserkServer.Server
{
    interface IClientMessageCallback
    {
        void Call(ClientMessage msg);
    }
}
