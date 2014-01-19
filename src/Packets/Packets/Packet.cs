// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Squirrel.Packets
{
    [Serializable]
    public class Packet
    {
        [NonSerialized] public Socket Socket;
        public PacketType PacketType { get; set; }
        public int ClientId { get; set; }

        public Packet()
        {
            PacketType = PacketType.EMPTY_PACKET;
            ClientId = -1;
        }

        public Packet(PacketType packetType, int clientId)
        {
            PacketType = packetType;
            ClientId = clientId;
        }

        public override string ToString()
        {
            return "Packet: " + PacketType + " Client ID: " + ClientId;
        }

        public static byte[] bundle(Packet packet)
        {
            Packet[] packetArray = new Packet[1];
            packetArray[0] = packet;

            return bundle(packetArray);
        }

        public static byte[] bundle(Packet[] packets)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, packets);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        public static Packet[] unbundle(byte[] raw)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(raw))
            {
                return (Packet[])formatter.Deserialize(stream);
            }
        }
    }
}
