using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


namespace MyRpg
{
    public class GClient
    {
        private TcpClient tcpClient;
        private GameAdmin gameAdmin;
        private GTcpClientOnClient gConnectedClient;
        public bool isConnected
        {
            private set;
            get;
        }
        public GTcpClientOnClient connectedClient
        {
            get { return gConnectedClient; }
        }
        public GClient(IPAddress ip, int port, GameAdmin gameAdmin)
        {
            this.gameAdmin = gameAdmin;
            Initialize(ip, port);
        }
        private void Initialize(IPAddress ip, int port)
        {
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(ip, port);
                isConnected = true;
            }
            catch (SocketException e)
            {
                isConnected = false;
                DLogger.WriteLineToScreen("GClient tcp connect error: " + e);
            }
            gConnectedClient = new GTcpClientOnClient(tcpClient, gameAdmin);
        }
        public void CloseConnection()
        {
            if (gConnectedClient.Connected)
            {
                gConnectedClient.CloseConnection();
            }
        }
    }
}