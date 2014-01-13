using System.Net;
using System.Net.Sockets;

namespace Squirrel.Client
{
    public class Client
    {
        private const int m_port = 37500;
        private readonly IPAddress m_ip = IPAddress.Parse("127.0.0.1");
        private IPEndPoint m_endPoint;

        private bool m_running = true;
        private readonly Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        public void run()
        {
            m_endPoint = new IPEndPoint(m_ip, m_port);

            const string message = "HI I'M FROM THE INTERNET";
            m_socket.Connect(m_endPoint);
        }
    }
}
