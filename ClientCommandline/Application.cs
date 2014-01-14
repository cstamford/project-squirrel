using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Squirrel;
using Squirrel.Data;
using Squirrel.Packets;

namespace ClientCommandline
{
    public class Application
    {
        private const int m_port = 37500;
        private static int m_clientId = -1;
        private static IPAddress m_ip;
        private static readonly Socket m_tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly Socket m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private static readonly Thread m_heartbeat = new Thread(sendHeartbeat);
        private static readonly Thread m_receiveUdpThread = new Thread(receiveUdp);
        private static readonly Thread m_receiveTcpThread = new Thread(receiveTcp);

        private static void Main(string[] args)
        {
            m_ip = IPAddress.Parse("94.174.148.248");

            Console.Title = "Project Squirrel Client";

            Console.WriteLine("###########################################");
            Console.WriteLine("#                                         #");
            Console.WriteLine("#            PROJECT SQUIRREL             #");
            Console.WriteLine("#                Client                   #");
            Console.WriteLine("#                                         #");
            Console.WriteLine("###########################################");
            Console.WriteLine();

            m_tcpSocket.Connect(new IPEndPoint(m_ip, m_port));
            m_udpSocket.Bind(m_tcpSocket.LocalEndPoint);

            byte[] rawBuffer = new byte[Globals.PACKET_BUFFER_SIZE];

            try
            {
                // Now that we've got a TCP connection, receive client ID and orientation from the server
                m_tcpSocket.Receive(rawBuffer, rawBuffer.Count(), SocketFlags.None);

                // Unbundle the packets received
                Packet[] packetsReceived = Packet.unbundle(rawBuffer);

                // Select the orientation packet from the bundle
                foreach (NewClientPacket ncPacket in packetsReceived.Where(packet => packet.PacketType == PacketType.NEW_CLIENT_PACKET).Cast<NewClientPacket>())
                {
                    m_clientId = ncPacket.ClientId;
                    m_udpSocket.SendTo(Packet.bundle(new NewClientPacket(ncPacket.ClientId, ncPacket.Orientation)), m_tcpSocket.RemoteEndPoint);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            m_heartbeat.Start();
            m_receiveUdpThread.Start();
            m_receiveTcpThread.Start();

            while (true)
            {
            }
        }

        private static void sendHeartbeat()
        {
            while (true)
            {
                try
                {
                    m_tcpSocket.Send(Packet.bundle(new HeartbeatPacket(m_clientId)));
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Heartbeat exception");
                    Console.WriteLine(exception);
                }

                Thread.Sleep(Globals.PACKET_HEARTBEAT_FREQUENCY);
            }
        }

        private static void receiveTcp()
        {
            while (true)
            {
                try
                {
                    byte[] rawArray = new byte[Globals.PACKET_BUFFER_SIZE];
                    m_tcpSocket.Receive(rawArray, rawArray.Count(), SocketFlags.None);
                    Packet[] packets = Packet.unbundle(rawArray);
                    Console.WriteLine("[TCP]" + packets[0]);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Rceive TCP exception");
                    Console.WriteLine(exception);
                }
            }
        }

        private static void receiveUdp()
        {
            while (true)
            {
                try
                {
                    byte[] rawArray = new byte[Globals.PACKET_BUFFER_SIZE];
                    EndPoint ep = m_tcpSocket.RemoteEndPoint;
                    m_udpSocket.Receive(rawArray, rawArray.Count(), SocketFlags.None);
                    Packet[] packets = Packet.unbundle(rawArray);
                    Console.WriteLine("[UDP]" + packets[0]);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Rceive UDP exception");
                    Console.WriteLine(exception);
                }
            }
        }
    }
}
