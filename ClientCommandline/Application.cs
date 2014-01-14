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
        private static IPAddress m_ip;
        private static Socket m_tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        private static Thread m_heartbeat = new Thread(sendHeartbeat);
        private static Thread m_receiveUdpThread = new Thread(receiveUdp);
        private static Thread m_receiveTcpThread = new Thread(receiveTcp);

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
            m_udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));

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
                    m_tcpSocket.Send(Packet.bundle(new HeartbeatPacket(1)));
                    m_udpSocket.SendTo(Packet.bundle(new HeartbeatPacket(1)), m_tcpSocket.RemoteEndPoint);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Exception");
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
