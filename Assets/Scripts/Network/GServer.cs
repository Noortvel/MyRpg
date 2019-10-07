using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MyRpg
{


    public class GServer
    {
        private TcpListener listener;
        private List<GTcpClientOnServer> clients;
        private GameAdmin gameAdmin;
        private byte playerCount = 0;
        //private List<Character> characters;
        public GServer(IPAddress ip, int port, GameAdmin gameAdmin)
        {
            this.gameAdmin = gameAdmin;
            Initialize(ip, port);
        }
        private void Initialize(IPAddress ip, int port)
        {
            clients = new List<GTcpClientOnServer>();
            listener = new TcpListener(ip, port);
            listener.Start();
            listener.BeginAcceptTcpClient(OnConnect, null);
        }
        private void OnConnect(IAsyncResult ar)
        {
            TcpClient client = listener.EndAcceptTcpClient(ar);

            DLogger.WriteLineToScreen("Client Connected, id: " + playerCount);
            DLogger.WriteLineToScreen("Connected clients: " + clients.Count);

            var gclient = new GTcpClientOnServer(client, this, playerCount);
            playerCount++;

            ReplicateCharacterToClients(gclient);
        
            

            clients.Add(gclient);
            listener.BeginAcceptTcpClient(OnConnect, null);
        }
        private void ReplicateCharacterToClients(GTcpClientOnServer gclient)
        {
            DLogger.WriteLineToScreen("Replicate client start");
            byte[] cur = (new NetworkCryptor.PlayerInstanceInfo(gclient.idNet, gameAdmin.SpawnPoint.position)).toBytes();
            gclient.SendAsync(cur);
            //Отсылка всем существующим клиентам player instance
            for (int i = 0; i < clients.Count; i++)
            {
                DLogger.WriteLineToScreen("new to clients: " + gclient.idNet);
                clients[i].SendAsync(cur);
            }
            //Отсылка новому клиенту информации о всех подключенных клиентах
            for (int i = 0; i < clients.Count; i++)
            {
                byte[] mes = new NetworkCryptor.PlayerInstanceInfo(
                    clients[i].idNet, gameAdmin.SpawnPoint.position).toBytes();
                DLogger.WriteLineToScreen("clients to new: " + clients[i].idNet);
                gclient.SendAsync(mes);
            }
            DLogger.WriteLineToScreen("Replicate client end");
        }
        public void SendMessageToClients(byte[] message)
        {
            foreach (var x in clients)
            {
                x.SendAsync(message);
            }
        }
        public void SendMessageToClientNotOne(byte[] message, GTcpClientOnServer client)
        {
            foreach (var x in clients)
            {
                if (x != client)
                {
                    x.SendAsync(message);
                }
            }
        }
        public void CloseConnections()
        {
            listener.Stop();
            foreach (var x in clients)
            {
                x.CloseConnection();
            }
        }
        public void RemoveClient(GTcpClientOnServer client)
        {

            clients.Remove(client);
            var message = new NetworkCryptor.PlayerDestroyInfo(client.idNet);
            SendMessageToClients(message.toBytes());
            DLogger.WriteLineToScreen("GServer: Removed client: " + client.idNet);
            //client.CloseConnection();
        }
    }
}