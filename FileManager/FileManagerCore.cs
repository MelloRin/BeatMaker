﻿using MelloRin.FileManager.lib.Encrypt;
using MelloRin.FileManager.lib.UUID;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileManager
{
    public class FileManagerCore
    {
        public static readonly short longSize = 8;
        public static readonly short shortSize = 2;

        public static readonly short byteGardSize = 1;

        public static readonly string resourceFileName = "resource.dat";

        public static readonly string currentDir = Directory.GetCurrentDirectory();

        public static readonly string dataPackageDir = Path.Combine(currentDir, resourceFileName);
        public static readonly string saveFileDir = Path.Combine(currentDir, "savedata.mlr");

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string ReadSettingData()
        {
            if (File.Exists(saveFileDir))
            {
                StreamReader reader = new StreamReader(saveFileDir);

                byte[] UUID = _readUUID(reader.BaseStream);

                byteBoundaryWork(reader.BaseStream, false);

                string rawData = reader.ReadToEnd();

                reader.Close();

                return AES256Manager.decode(rawData, UUID);
            }
            else
                throw new FileNotFoundException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SaveSettingData(string data)
        {
            byte[] fakeUUID = UUIDManager.MakeUUID();

            byte[] UUID = UUIDManager.ConvertUUID(fakeUUID);

            string rawData = AES256Manager.encode(data, UUID);

            if (File.Exists(saveFileDir))
                File.Delete(saveFileDir);

            StreamWriter writer = new StreamWriter(saveFileDir);

            writer.Write(fakeUUID);
            byteBoundaryWork(writer.BaseStream, true);
            writer.Write(rawData);

            writer.Close();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static MemoryStream ReadGameData(Stream stream)
        {
            if (File.Exists(dataPackageDir))
            {
                BinaryReader reader = new BinaryReader(stream);

                byte[] UUID = _readUUID(reader.BaseStream);
                byteBoundaryWork(reader.BaseStream, false);

                Console.WriteLine("uuid = {0}", Encoding.UTF8.GetString(UUID));

                long headerLen = reader.ReadInt64();
                byteBoundaryWork(reader.BaseStream, false);

                byte[] encHeaderData = new byte[headerLen];

                Console.WriteLine("headerLen = {0}", headerLen);

                readBytes(encHeaderData, reader.BaseStream.Position, reader.BaseStream);
                byte[] headerData = AES256Manager.decodeToByteArray(Encoding.UTF8.GetString(encHeaderData), UUID);

                MemoryStream ms = new MemoryStream(headerData);

                byteBoundaryWork(stream, false);
                
                return ms;
            }
            else
                throw new FileNotFoundException();
        }

        public static void readBytes(byte[] target, long startIndex,Stream stream)
        {
            stream.Position = startIndex;
            for(long idx = 0; idx < target.LongLength; ++idx)
            {
                target[idx] = (byte)stream.ReadByte();
            }
        }

        private static byte[] _readUUID(Stream stream)
        {
            byte[] rawUUID = new byte[64];

            stream.Read(rawUUID, 0, rawUUID.Length);

            return UUIDManager.ConvertUUID(rawUUID);
        }

        public static void byteBoundaryWork(Stream stream, bool isWriteWork)
        {
            for(int i = 0; i < byteGardSize; ++i)
            {
                if (isWriteWork)
                    stream.WriteByte(0);
                else
                    stream.ReadByte();
            }
        }
    }
}