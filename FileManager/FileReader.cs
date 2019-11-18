using MelloRin.FileManager.lib.Encrypt;
using MelloRin.FileManager.lib.UUID;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace MelloRin.FileManager
{
    public class FileReader
    {
        private static readonly string currentDir = Directory.GetCurrentDirectory();

        private static readonly string dataPackageName = Path.Combine(currentDir, "resource.dat");
        private static readonly string saveFileName = Path.Combine(currentDir, "savedata.mlr");

        public static string ReadSettingData()
        {
            if (File.Exists(saveFileName))
            {
                StreamReader reader = new StreamReader(saveFileName);

                char[] rawUUID = new char[64];

                reader.Read(rawUUID, 0, rawUUID.Length);

                string UUID = UUIDManager.ReadUUID(Encoding.UTF8.GetBytes(rawUUID));

                string rawData = reader.ReadToEnd();

                return AES256Manager.decrypt(rawData, UUID);
            }
            else
                throw new FileNotFoundException();
        }

        public static void SaveSettingData(string data)
        {
            string fakeUUID = UUIDManager.MakeUUID();

            string UUID = UUIDManager.ReadUUID(Encoding.UTF8.GetBytes(fakeUUID));

            string rawData = AES256Manager.encrypt(data, UUID);

            if (File.Exists(saveFileName))
            {
                if (File.Exists(saveFileName))
                    File.Delete(saveFileName);
            }

            StreamWriter writer = new StreamWriter(saveFileName);

            writer.Write(fakeUUID + rawData);
            writer.Close();
        }

        public static ConcurrentDictionary<string, MemoryStream> ReadGameData()
        {





            throw new NotImplementedException();
        }
    }
}



/*

ffffff...fffffffff(64byte) 	암호화키    ( {32(키)} + {32(더미)} )
ffffffffff(1+1+4+4byte)		디렉토리정보 ( {1(자신의id)} + {1(상위폴더id)} + {4(이름데이터 주소(시작))} + {4(이름데이터 주소(끝))} )
fffffffff(1+4+4byte)		파일정보 	 ( {1(디렉토리id)} + {4(이름데이터 주소(시작))} + {4(이름데이터 주소(끝))} )
-------------------------------
파일/디렉토리 이름데이터


-------------------------------
파일 raw데이터





-------------------------------
*/