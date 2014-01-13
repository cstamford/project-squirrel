using System.Net.Sockets;

namespace Squirrel.Data
{
    public class Connection
    {
        public int ClientId { get; set; }
        public Socket TcpSocket { get; set; }
        public Socket UdpSocket { get; set; }

        public string toString()
        {
            string value = "Client ID: " + ClientId;

            if (TcpSocket.RemoteEndPoint != null)
                value += " IP: " + TcpSocket.RemoteEndPoint;

            return value;
        }
    }
}
