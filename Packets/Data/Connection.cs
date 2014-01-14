using System.Net;
using System.Net.Sockets;

namespace Squirrel.Data
{
    public class Connection
    {
        public int ClientId { get; set; }

        public Socket TcpSocket { get; set; }
        public bool TcpReady { get; set; }
        public long TcpLastReceived { get; set; }

        public Socket UdpSocket { get; set; }
        public bool UdpReady { get; set; }
        public long UdpLastReceived { get; set; }

        public override string ToString()
        {
            string value = "Client ID: " + ClientId;

            if (TcpSocket.RemoteEndPoint != null)
                value += " IP: " + TcpSocket.RemoteEndPoint;

            return value;
        }

        // Checks if the provided connection is valid, has a proper client ID associated with it, and its
        // sockets have been properly initialised
        public static bool connectionValid(Connection connection)
        {
            return connection != null && connection.ClientId != -1 && connection.TcpSocket != null &&
                   connection.UdpSocket != null;
        }
    }
}
