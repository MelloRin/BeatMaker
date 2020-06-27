using System;
using System.IO;
using System.Text;

namespace MelloRin.FileManager.lib.UUID
{
    public class UUIDManager
    {
        private static string _generateUUID()
        {
            Guid guid = Guid.NewGuid();

            StringBuilder uuidBuilder = new StringBuilder();

            foreach (char c in guid.ToString())
            {
                if (c == '-')
                    continue;

                uuidBuilder.Append(c);
            }

            return uuidBuilder.ToString();
        }

        public static byte[] ConvertUUID(byte[] rawKey)
        {
            MemoryStream byteBuffer = new MemoryStream(rawKey.Length / 2);

            byteBuffer.WriteByte(rawKey[0]);

            int isEven = rawKey[0] % 2 == 0 ? 0 : 1;
            //첫 바이트가 짝수이면 짝수번째칸, 홀수면 홀수번째칸.
            for (int i = 1 + isEven; i < rawKey.Length - 1; i += 2)
                byteBuffer.WriteByte(rawKey[i]);

            byte[] array = byteBuffer.ToArray();

            byteBuffer.Close();

            return array;
        }

        public static byte[] MakeUUID()
        {
            string UUID = _generateUUID();
            Random seed = new Random();

            byte[] uuidArr = Encoding.UTF8.GetBytes(UUID);
            MemoryStream byteBuffer = new MemoryStream(UUID.Length * 2);

            byteBuffer.WriteByte(uuidArr[0]);

            bool isEven = (uuidArr[0] % 2 == 0) ? true : false;

            //첫 바이트가 짝수이면 짝수번째칸, 홀수면 홀수번째칸.
            for (int i = 1; i < uuidArr.Length; ++i)
            {
                if (isEven)
                {
                    byteBuffer.WriteByte(uuidArr[i]);
                    byteBuffer.WriteByte((byte)seed.Next('1', '9'));
                }
                else
                {
                    byteBuffer.WriteByte((byte)seed.Next('1', '9'));
                    byteBuffer.WriteByte(uuidArr[i]);
                }
            }

            byteBuffer.WriteByte((byte)seed.Next('1', '9'));


            byte[] array = byteBuffer.ToArray();

            byteBuffer.Close();

            return array;
        }
    }
}