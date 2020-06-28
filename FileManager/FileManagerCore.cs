using FileManager.util.Encrypt;
using FileManager.util.Log;
using FileManager.util.UUID;
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
        public static readonly string saveFileName = "savedata.mlr";
        public static readonly string logFileName = "log.txt";

        public static readonly string currentDir = Directory.GetCurrentDirectory();

        public static readonly string dataPackageDir = Path.Combine(currentDir, resourceFileName);
        public static readonly string saveFileDir = Path.Combine(currentDir, saveFileName);

        public static readonly Logger logger = new Logger(logFileName);

        private static readonly FileManagerCore inst = new FileManagerCore();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string ReadSettingData()
        {
            logger.Info(inst, "Loading SaveFile... SaveFile dir = " + saveFileDir);
            if (File.Exists(saveFileDir))
            {
                StreamReader reader = new StreamReader(saveFileDir);

                byte[] UUID = _readUUID(reader.BaseStream);

                byteBoundaryWork(reader.BaseStream, false);

                string rawData = reader.ReadToEnd();

                reader.Close();

                string result = AES256Manager.decode(rawData, UUID);
                logger.Info(inst, "Loading SaveFile Success!");

                return result;
            }
            else
            {
                logger.Warning(inst, "Loading SaveFile Fail... File not found ");
                throw new FileNotFoundException();
            } 
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SaveSettingData(string data)
        {
            logger.Info(inst, "Saving SaveFile... SaveFile dir = " + saveFileDir);
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
            logger.Info(inst, "Loading SaveFile Success!");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static MemoryStream ReadGameData(Stream stream)
        {
            logger.Info(inst, "Loading GameFile... ResourceFile dir = " + dataPackageDir);
            if (File.Exists(dataPackageDir))
            {
                BinaryReader reader = new BinaryReader(stream);

                byte[] UUID = _readUUID(reader.BaseStream);
                byteBoundaryWork(reader.BaseStream, false);

                long headerLen = reader.ReadInt64();
                byteBoundaryWork(reader.BaseStream, false);

                byte[] encHeaderData = new byte[headerLen];

                readBytes(encHeaderData, reader.BaseStream.Position, reader.BaseStream);
                byte[] headerData = AES256Manager.decodeToByteArray(Encoding.UTF8.GetString(encHeaderData), UUID);

                MemoryStream ms = new MemoryStream(headerData);

                byteBoundaryWork(stream, false);

                logger.Info(inst, "Loading GameFileHeader Success!");
                return ms;
            }
            else
            {
                logger.Warning(inst, "Loading GameFile Fail... File not found ");
                throw new FileNotFoundException();
            }  
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
