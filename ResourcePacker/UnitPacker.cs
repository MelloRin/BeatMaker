using FileManager;
using FileManager.util.Encrypt;
using FileManager.util.UUID;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ResourcePacker
{
    class UnitPacker
    {
        private static readonly string currentDir = Directory.GetCurrentDirectory();

        private readonly string targetDir;

        private readonly ResFileListReader resFileListReader;

        private readonly byte[] fakeUUID;
        private readonly byte[] UUID;

        public UnitPacker(string targetDir)
        {
            this.targetDir = targetDir;
            resFileListReader = new ResFileListReader();

            fakeUUID = UUIDManager.MakeUUID();
            UUID = UUIDManager.ConvertUUID(fakeUUID);
        }

        public byte[] getPackedData()
        {
            FileManagerCore.logger.Info(this, "start Packing... target Dir = " + targetDir);
            resFileListReader.searchDir(targetDir, targetDir, null);

            Dictionary<string, short> nameInfo = _getNameDataList();

            MemoryStream ms = new MemoryStream();

            ms.Write(fakeUUID, 0, fakeUUID.Length);
            FileManagerCore.byteBoundaryWork(ms, true);

            long encHeaderLen = _getHeaderLen(nameInfo);

            byte[] headerLenData = BitConverter.GetBytes(encHeaderLen);

            ms.Write(headerLenData, 0, headerLenData.Length);
            FileManagerCore.byteBoundaryWork(ms, true);

            long resumePosition = ms.Position;

            long nameDataStartPoint = fakeUUID.Length + FileManagerCore.byteGardSize + FileManagerCore.longSize + FileManagerCore.byteGardSize + (encHeaderLen + FileManagerCore.byteGardSize);

            ms.Position = nameDataStartPoint;

            MemoryStream headerMaker = new MemoryStream();

            byte[] vOut = BitConverter.GetBytes((long)nameInfo.Count);
            headerMaker.Write(vOut, 0, vOut.Length);
            FileManagerCore.byteBoundaryWork(headerMaker, true);

            foreach (string name in nameInfo.Keys)
            {
                vOut = BitConverter.GetBytes(nameInfo[name]);
                headerMaker.Write(vOut, 0, vOut.Length);

                vOut = BitConverter.GetBytes(ms.Position);
                headerMaker.Write(vOut, 0, vOut.Length);

                byte[] data = Encoding.UTF8.GetBytes(name);
                ms.Write(data, 0, data.Length);

                vOut = BitConverter.GetBytes((long)data.Length);
                headerMaker.Write(vOut, 0, vOut.Length);
                FileManagerCore.byteBoundaryWork(headerMaker, true);

                FileManagerCore.byteBoundaryWork(ms, true);
            }

            vOut = BitConverter.GetBytes((long)resFileListReader.dirList.Count);
            headerMaker.Write(vOut, 0, vOut.Length);
            FileManagerCore.byteBoundaryWork(headerMaker, true);

            foreach (Dir dir in resFileListReader.dirList)
            {
                vOut = BitConverter.GetBytes(dir.id);
                headerMaker.Write(vOut, 0, vOut.Length);

                if (dir.parentDir != null)
                    vOut = BitConverter.GetBytes(dir.parentDir.id);
                else
                    vOut = BitConverter.GetBytes((short)0);

                headerMaker.Write(vOut, 0, vOut.Length);

                vOut = BitConverter.GetBytes(dir.nameId);
                headerMaker.Write(vOut, 0, vOut.Length);

                FileManagerCore.byteBoundaryWork(headerMaker, true);
            }

            vOut = BitConverter.GetBytes((long)resFileListReader.fileList.Count);
            headerMaker.Write(vOut, 0, vOut.Length);
            FileManagerCore.byteBoundaryWork(headerMaker, true);

            foreach (ResFile file in resFileListReader.fileList)
            {
                vOut = BitConverter.GetBytes(file.id);
                headerMaker.Write(vOut, 0, vOut.Length);

                vOut = BitConverter.GetBytes(file.parentDir.id);
                headerMaker.Write(vOut, 0, vOut.Length);

                vOut = BitConverter.GetBytes(file.nameId);
                headerMaker.Write(vOut, 0, vOut.Length);

                vOut = BitConverter.GetBytes(ms.Position);
                headerMaker.Write(vOut, 0, vOut.Length);

                file.rawData.WriteTo(ms);

                vOut = BitConverter.GetBytes(file.rawData.Length);
                headerMaker.Write(vOut, 0, vOut.Length);

                FileManagerCore.byteBoundaryWork(ms, true);
                FileManagerCore.byteBoundaryWork(headerMaker, true);
            }

            FileManagerCore.logger.Info(this, "file packing done!");

            ms.Position = resumePosition;

            string encData = AES256Manager.encode(headerMaker.ToArray(), UUID);
            headerMaker.Close();

            byte[] encHeader = Encoding.UTF8.GetBytes(encData);

            ms.Write(encHeader, 0, encHeader.Length);

            byte[] result = ms.ToArray();
            ms.Close();

            FileManagerCore.logger.Info(this, "header Write done!");

            return result;
        }

        private Dictionary<string, short> _getNameDataList()
        {
            short toAllocateID = 0;
            Dictionary<string, short> nameInfo = new Dictionary<string, short>();
            foreach (Dir dir in resFileListReader.dirList)
            {
                if (nameInfo.ContainsKey(dir.dirName))
                    dir.nameId = nameInfo[dir.dirName];
                else
                {
                    nameInfo.Add(dir.dirName, toAllocateID);
                    dir.nameId = toAllocateID++;
                }
            }
            foreach (ResFile file in resFileListReader.fileList)
            {
                if (nameInfo.ContainsKey(file.fileName))
                    file.nameId = nameInfo[file.fileName];
                else
                {
                    nameInfo.Add(file.fileName, toAllocateID);
                    file.nameId = toAllocateID++;
                }
            }

            return nameInfo;
        }

        private long _getHeaderLen(Dictionary<string, short> nameInfo)
        {
            int nameHeaderLen = nameInfo.Count * (FileManagerCore.shortSize + (2 * FileManagerCore.longSize) + FileManagerCore.byteGardSize);
            int dirHeaderLen = resFileListReader.dirList.Count * ((3 * FileManagerCore.shortSize) + FileManagerCore.byteGardSize);
            int fileHeaderLen = resFileListReader.fileList.Count * ((3 * FileManagerCore.shortSize) + (2 * FileManagerCore.longSize) + FileManagerCore.byteGardSize);
            int lenDatalen = 3 * (FileManagerCore.longSize + FileManagerCore.byteGardSize);

            int realHeaderSize = nameHeaderLen + dirHeaderLen + fileHeaderLen + lenDatalen;
            byte[] dumyArr = new byte[realHeaderSize];

            string encData = AES256Manager.encode(dumyArr, UUID);
            long encHeaderLen = Encoding.UTF8.GetByteCount(encData);

            return encHeaderLen;
        }
    }
}

/*
ffffff....fff (64byte)  암호화키        ( {32(키)} + {32(더미)} )
ffff		  (1 * long)  헤더 길이

ffff		  (1 * long ) 이름 정보 갯수
fffffffff         이름정보    (short + 2 * long)    ( {1(자신의ID)} + 데이터 시작점 + 데이터 사이즈)
ffff		  (1 * long ) 디렉토리 정보 갯수
fff	  (3 * short)	디렉토리정보    ( {1(자신의ID)} + {1(상위폴더ID)} + {1(이름정보ID)})
ffff		  (1 * long ) 파일정보 갯수
ff      (2 * short ) + (2 * long)  파일정보        ( {1(디렉토리ID)} + {1(이름정보ID)} + 데이터 시작점 + 데이터 사이즈)
-------------------------------
파일/디렉토리 이름데이터


-------------------------------
파일 raw데이터





-------------------------------
*/
