using System;
using System.Net.Sockets;

namespace MyRpg
{
    public abstract class GTcpClient
    {
        private TcpClient client;
        private byte[] readbuffer;
        private byte id;
        public GTcpClient(TcpClient client)
        {
            this.client = client;
            this.client.NoDelay = true;
       
            if (client.Connected)
            {
                readbuffer = new byte[256];
                client.GetStream().BeginRead(readbuffer, 0, readbuffer.Length, OnReadEnd, null);
            }
        }
        /// <summary>
        /// If it is not connected, or connected lose, return false,
        /// else message sended
        /// </summary>
        /// <param name="message"></param>
        public bool SendAsync(byte[] message)
        {
            BeforeSending();
            var stream = client.GetStream();
            if (client.Connected && stream.CanWrite)
            {
                try
                {
                    
                    client.GetStream().WriteAsync(message, 0, message.Length);
                    return true;
                }
                catch (Exception e)
                {
                    DLogger.WriteLineToScreen("GTcpClient: Send Async Fail, ex: " + e);
                }
                return false;
            }
            else
            {
                DLogger.WriteLineToScreen("Cant send, stream cant write");
            }
            return false;
        }
        private void OnReadEnd(IAsyncResult ar)
        {
            int count = client.GetStream().EndRead(ar);
            DLogger.WriteLineToScreen("GTcpClient: EndRead, count: " + count);
            if(count == 0)
            {   
                CloseConnection();
                return;
            }
            int cmdSize = NetworkCryptor.NetCommand.Size;
            int mcount = count / cmdSize;
            for(int i = 0; i < mcount; i++)
            {
                byte[] b = new byte[cmdSize];
                Buffer.BlockCopy(readbuffer, i * cmdSize, b, 0, cmdSize);
                MessageArrived(b);
            }
            var stream = client.GetStream();
            if (client.Connected && stream.CanRead)
            {
                client.GetStream().BeginRead(readbuffer, 0, readbuffer.Length, OnReadEnd, null);
            }
        }
        protected abstract void BeforeSending();
       
        protected abstract void MessageArrived(byte[] message);
        protected abstract void ConnectionAfterClosed();
        public void CloseConnection()
        {
            if (client.Connected)
            {
                DLogger.WriteLineToScreen("GTcpClient: Connection closing");
                client.GetStream().Flush();
                client.GetStream().Close();
                client.Close();
                ConnectionAfterClosed();
            }
            else
            {
                DLogger.WriteLineToScreen("GTcpClient: Called Connection closing, but it already closed");
            }
        }
        public bool Connected
        {
            get { return client.Connected; }
        }
    }
}