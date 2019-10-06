using System;
using UnityEngine;

namespace MyRpg
{

    public class NetworkCryptor
    {
        public enum NetworkMessageType
        {
            None,
            InstanceCharacter,
            MoveTo
        }
        public class NetCommand
        {
            public static int Size
            {
                get { return 16; }
            }
        }
        public class PlayerInstanceInfo : NetCommand
        {
            public byte id;
            public Vector3 position;
            public PlayerInstanceInfo(byte id, Vector3 position)
            {
                this.id = id;
                this.position = position;
            }
            public static PlayerInstanceInfo fromBytes(byte[] b)
            {
                if (isThis(b))
                {
                    var position = NetworkCryptor.ReadVector3f(ref b, 4);
                    var pinf = new PlayerInstanceInfo(b[3], position);
                    return pinf;
                }
                return null;
            }
            public byte[] toBytes()
            {
                var b = new byte[4 + 12];
                b[0] = 16;
                b[1] = Convert.ToByte('g');
                b[2] = Convert.ToByte('c');
                b[3] = id;
                WriteVector3f(ref b, 4, position);
                return b;
            }
            public static bool isThis(byte[] b)
            {
                if (b.Length >= 3 && b[0] == 16 && b[1] == Convert.ToByte('g') && b[2] == Convert.ToByte('c'))
                {
                    return true;
                }
                return false;
            }
        }
        public class PlayerMoveToInfo : NetCommand
        {
            public byte id;
            public Vector3 destonation;
            public PlayerMoveToInfo(byte id, Vector3 destonation)
            {
                this.id = id;
                this.destonation = destonation;
            }
            public byte[] toBytes()
            {
                byte[] message = new byte[12 + 4];
                message[0] = 16;
                message[1] = Convert.ToByte('t');
                message[2] = Convert.ToByte('m');
                message[3] = id;
                int offset = WriteVector3f(ref message, 4, destonation);
                return message;
            }
            public static bool isThis(byte[] b)
            {
                if (b.Length >= 3 && b[0] == 16 && b[1] == Convert.ToByte('t') && b[2] == Convert.ToByte('m'))
                {
                    return true;
                }
                return false;
            }
            public static PlayerMoveToInfo fromBytes(byte[] b)
            {
                if (isThis(b))
                {
                    byte id = b[3];
                    Vector3 v = ReadVector3f(ref b, 4);
                    var pmti = new PlayerMoveToInfo(id, v);
                    return pmti;
                }
                return null;
            }

        }
        public static NetworkMessageType GetTypeOfMessage(byte[] b)
        {
            if (PlayerMoveToInfo.isThis(b))
            {
                return NetworkMessageType.MoveTo;
            }
            else if (PlayerInstanceInfo.isThis(b))
            {
                return NetworkMessageType.InstanceCharacter;
            }
            return NetworkMessageType.None;
        }
        private static int WriteVector3f(ref byte[] bytes, int startIndex, Vector3 vector)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(vector.x), 0, bytes, startIndex, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector.y), 0, bytes, startIndex + 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vector.z), 0, bytes, startIndex + 8, 4);
            return 12;
        }
        private static Vector3 ReadVector3f(ref byte[] srcb, int startIndexSrc)
        {
            Vector3 v = new Vector3();
            v.x = BitConverter.ToSingle(srcb, startIndexSrc);
            v.y = BitConverter.ToSingle(srcb, startIndexSrc + 4);
            v.z = BitConverter.ToSingle(srcb, startIndexSrc + 8);
            return v;
        }
    }
}
