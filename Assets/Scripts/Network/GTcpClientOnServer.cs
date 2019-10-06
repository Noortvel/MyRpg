using System;
using System.Net.Sockets;


namespace MyRpg
{
    public class GTcpClientOnServer : GTcpClient
    {
        private GServer server;
        public byte idNet;
        public GTcpClientOnServer(TcpClient client, GServer server, byte idNet) : base(client)
        {
            this.server = server;
            this.idNet = idNet;
        }
        protected override void MessageArrived(byte[] message)
        {
            if (message.Length >= 3)
            {
                char c1 = Convert.ToChar(message[1]);
                char c2 = Convert.ToChar(message[2]);
                string str = String.Format("1,2,3 bytes {0}, {1}, {2}", message[0], c1, c2);
                DLogger.WriteLineToScreen(str);
            }
            else
            {
                DLogger.WriteLineToScreen("Message error");
            }
            var type = NetworkCryptor.GetTypeOfMessage(message);
            if(type == NetworkCryptor.NetworkMessageType.MoveTo)
            {
                server.SendMessageToClientNotOne(message, this);
            }
        }
        protected override void BeforeSending()
        {
            //if (!Connected)
            //{
            //    DLogger.WriteLineToScreen("Client disconnected, removing it, id: " + idNet);
            //    server.RemoveClient(this);
            //}
        }

        protected override void ConnectionAfterClosed()
        {
            DLogger.WriteLineToScreen("GTcpClientOnServer: Disconnected, removing it from server list, id: " + idNet);
            server.RemoveClient(this);
        }
    }
}