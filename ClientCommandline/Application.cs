﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private static bool m_running = true;
        private static readonly Stopwatch m_timer = new Stopwatch();
        private static readonly Socket m_tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly Socket m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static IPEndPoint m_endPoint;
        private static readonly IPAddress m_ip = IPAddress.Parse("94.174.148.248");
        private const int m_port = 37500;

        private static int m_clientId = -1;


        private static void Main(string[] args)
        {
            m_endPoint = new IPEndPoint(m_ip, m_port);

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
            connEvent.RemoteEndPoint = m_endPoint;
            connEvent.Completed += onConnect;
            Console.WriteLine("Starting connection attempt to " + connEvent.RemoteEndPoint);
            m_tcpSocket.ConnectAsync(connEvent);
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

            byte[] rawArray = new byte[Globals.PACKET_BUFFER_SIZE];

            // Now we need to block until we get the ID designation packet
            m_tcpSocket.Receive(rawArray, rawArray.Count(), SocketFlags.None);

            Packet[] packets = Packet.unbundle(rawArray);

            foreach (Packet packet in packets)
            {
                // Get the designated client ID bundled inside the ID designation packet
                if (packet.PacketType == PacketType.NEW_CLIENT_PACKET)
                {
                    m_clientId = packet.ClientId;
                    Console.WriteLine("Received " + packet + ", client ID set");
                }
            }

            // Let's connect to UDP now
            m_udpSocket.Connect(m_endPoint);

            m_timer.Start();

            while (m_running)
            {
                if (m_timer.ElapsedMilliseconds > Globals.UPDATES_TICK_TIME)
                {
                    m_tcpSocket.Send(Packet.bundle(new Packet[]
                    {
                        new ChatPacket(m_clientId, "Jim",
                            "HELLO I AM ON THE MOON"),
                    }));

                    m_udpSocket.Send(Packet.bundle(new Packet[]
                    {
                        new GamePacket(m_clientId,
                            new Orientation(1.0f, 1.0f, 1.0f),
                            new Orientation(2.0f, 2.0f, 2.0f),
                            new Vec2F(1.0f, 1.0f)), 
                        new GamePacket(m_clientId,
                            new Orientation(1.0f, 1.0f, 1.0f),
                            new Orientation(2.0f, 2.0f, 2.0f),
                            new Vec2F(1.0f, 1.0f)), 
                        new GamePacket(m_clientId,
                            new Orientation(1.0f, 1.0f, 1.0f),
                            new Orientation(2.0f, 2.0f, 2.0f),
                            new Vec2F(1.0f, 1.0f)), 
                        new GamePacket(m_clientId,
                            new Orientation(1.0f, 1.0f, 1.0f),
                            new Orientation(2.0f, 2.0f, 2.0f),
                            new Vec2F(1.0f, 1.0f)), 
                        new GamePacket(m_clientId,
                            new Orientation(1.0f, 1.0f, 1.0f),
                            new Orientation(2.0f, 2.0f, 2.0f),
                            new Vec2F(1.0f, 1.0f)), 

                    }));

                    try
                    {
                        m_tcpSocket.Send(new byte[0]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("We have been disconnected, shutting down");
                        Thread.Sleep(2500);
                        m_running = false;
                        break;
                    }

                    m_timer.Restart();
                }
            }
        }
    }
}
