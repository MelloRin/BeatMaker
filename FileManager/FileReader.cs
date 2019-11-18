using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace MelloRin.FileManager
{
    class FileReader
    {
        public static string ReadSaveFile()
        {
            //데이터를 복호화 한 후, string 으로 넘기고 게임단에서 데이터 가공!





            throw new NotImplementedException();
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
