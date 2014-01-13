using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Squirrel.Data;
using Squirrel.Packets;

namespace ClientCommandline
{
    public class Application
    {
        private const int m_port = 37500;
        private static readonly IPAddress m_ip = IPAddress.Parse("127.0.0.1");

        private static bool m_running = true;
        private static readonly Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static int m_clientId = -1;

        private static void Main(string[] args)
        {
            Console.Title = "Project Squirrel Client";

            Console.WriteLine("###########################################");
            Console.WriteLine("#                                         #");
            Console.WriteLine("#            PROJECT SQUIRREL             #");
            Console.WriteLine("#                Client                   #");
            Console.WriteLine("#                                         #");
            Console.WriteLine("###########################################");
            Console.WriteLine();

            beginTcpConnection();

            while (m_running)
            {
            }
        }

        private static void beginTcpConnection()
        {
            SocketAsyncEventArgs connEvent = new SocketAsyncEventArgs();
            connEvent.RemoteEndPoint = new IPEndPoint(m_ip, m_port);
            connEvent.Completed += onConnect;
            Console.WriteLine("Starting connection attempt to " + connEvent.RemoteEndPoint);
            m_socket.ConnectAsync(connEvent);
        }

        private static void onConnect(object sender, SocketAsyncEventArgs e)
        {
            if (e.ConnectSocket != null)
            {
                Console.WriteLine("Connected to " + e.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine("Connection to " + e.RemoteEndPoint + " failed");
                return;
            }

            Socket socket = e.ConnectSocket;

            socket.Send(Packet.serialize(new ChatPacket(m_clientId,
                "HELLO I AM ON THE MOON")));

            Thread.Sleep(500);

            socket.Send(Packet.serialize(new GamePacket(
                m_clientId, 1.0f, 1.0f, 45.0f, 1.0f, 2.0f, 45.0f, 0.0f, -1.0f)));
        }

    }
}
