// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System.Net.Sockets;

namespace Squirrel.Data
{
    public class ConnectionPacketBundle
    {
        public byte[] RawBytes { get; set; }
        public Connection Connection { get; set; }
        public Socket Socket { get; set; }

        public ConnectionPacketBundle(Connection connection, Socket socket)
        {
            RawBytes = new byte[Globals.PACKET_BUFFER_SIZE];
            Connection = connection;
            Socket = socket;
        }
    }
}
