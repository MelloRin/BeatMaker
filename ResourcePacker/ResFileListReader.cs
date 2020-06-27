using FileManager;
using System;
using System.Collections.Generic;
using System.IO;

namespace ResourcePacker
{
    class ResFileListReader
    {
        public List<Dir> dirList = new List<Dir>();
        public List<ResFile> fileList = new List<ResFile>();

        private readonly Dir baseDir = new Dir("");

        private short toAllocateID = 1;

        public void searchDir(string targetDir, string prevDir, Dir parentDir)
        {
            try
            {
                Dir currentSearchingDir = new Dir(toAllocateID++, cutRestDir(targetDir, prevDir), parentDir);

                dirList.Add(currentSearchingDir);
                FileManagerCore.logger.Info(this, "packing dir : " + currentSearchingDir.dirName);

                foreach (string currentFile in Directory.EnumerateFiles(targetDir, "*"))
                {
                    string fileName = cutRestDir(currentFile, targetDir);
                    FileManagerCore.logger.Info(this, "packing file : " + fileName);
                    MemoryStream ms = new MemoryStream();

                    StreamReader reader = new StreamReader(currentFile);

                    reader.BaseStream.CopyTo(ms);

                    ResFile currentFileInst = new ResFile(toAllocateID++, fileName, ms, currentSearchingDir);

                    fileList.Add(currentFileInst);
                }

                foreach (string currentDir in Directory.EnumerateDirectories(targetDir, "*"))
                {
                    searchDir(currentDir, targetDir, currentSearchingDir);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string cutRestDir(string target, string currentDir)
        {
            if (target.Length == currentDir.Length)
                return "/";

            return target.Substring(currentDir.Length + 1);
        }
    }

    class Dir
    {
        public readonly short id;
        public short nameId;

        public readonly string dirName;
        public readonly Dir parentDir;

        public Dir(short id, string dirName, Dir parentDir)
        {
            this.id = id;
            this.dirName = dirName;
            this.parentDir = parentDir;
        }

        public Dir(string dirName)
        {
            this.dirName = dirName;
        }
    }

    class ResFile
    {
        public readonly short id;
        public short nameId;

        public readonly string fileName;
        public readonly MemoryStream rawData;

        public readonly Dir parentDir;

        public ResFile(short id, string fileName, MemoryStream rawData, Dir parentDir)
        {
            this.id = id;
            this.fileName = fileName;
            this.rawData = rawData;
            this.parentDir = parentDir;
        }
    }
}

/*
ffffff....fff(64byte)  암호화키( { 32(키)} + {32(더미)} )
ffff(1 * long)  헤더 길이
fffffffff 이름정보(short + 2 * long)    ( {1(자신의ID)} + 데이터 시작점 + 데이터 끝점)
fff(3 * short)  디렉토리정보( { 1(자신의ID)} + {1(상위폴더ID)} + {1(이름정보ID)})
ff(2 * short ) + (2 * long)  파일정보( { 1(디렉토리ID)} + {1(이름정보ID)} + 데이터 시작점 + 데이터 끝점)
-------------------------------
파일/디렉토리 이름데이터


-------------------------------
파일 raw데이터





-------------------------------
*/