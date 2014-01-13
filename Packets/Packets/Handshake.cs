using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Squirrel.Packets
{
    [Serializable]
    public class Handshake
    {
        public String messageOne;
        public String messageTwo;

        public static byte[] serialize(Handshake packet)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, packet);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        public static Handshake deserialize(byte[] raw)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(raw))
            {
                return (Handshake)formatter.Deserialize(stream);
            }
        }
    }
}
