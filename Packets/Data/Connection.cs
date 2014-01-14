using System.Net.Sockets;

namespace Squirrel.Data
{
    public class Connection
    {
        public int ClientId { get; set; }

        public Socket TcpSocket { get; set; }
        public bool TcpReady { get; set; }

        public Socket UdpSocket { get; set; }
        public bool UdpReady { get; set; }

        public override string ToString()
        {
            string value = "Client ID: " + ClientId;

            if (TcpSocket.RemoteEndPoint != null)
                value += " IP: " + TcpSocket.RemoteEndPoint;

            return value;
        }
    }
}
