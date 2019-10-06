using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyRpg
{
    public class GTcpClientOnClient : GTcpClient
    {
        private GameAdmin gameAdmin;

        public GTcpClientOnClient(TcpClient client, GameAdmin gameAdmin) : base(client)
        {
            this.gameAdmin = gameAdmin;
        }

        protected override void BeforeSending()
        {
            if (!Connected)
            {
                DLogger.WriteLineToScreen("Client not connected, send fail");
            }
        }

        protected override void ConnectionAfterClosed()
        {
            DLogger.WriteLineToScreen("GClientOnClient disconnected");

        }

        protected override void MessageArrived(byte[] message)
        {
            //DLogger.WriteLineToScreen("Message Length: " + message.Length);
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


            if (type == NetworkCryptor.NetworkMessageType.InstanceCharacter)
            {
                var pi = NetworkCryptor.PlayerInstanceInfo.fromBytes(message);
                DLogger.WriteLineToScreen("Instance id: " + pi.id);
                gameAdmin.AddMessageToQueue(pi);
            }
            if(type == NetworkCryptor.NetworkMessageType.MoveTo)
            {
                var pi = NetworkCryptor.PlayerMoveToInfo.fromBytes(message);
                gameAdmin.AddMessageToQueue(pi);
            }
        }
    }
}
