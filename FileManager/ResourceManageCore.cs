using FileManager.util.Directory;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileManager
{
    public class ResourceManageCore
    {
        private readonly Dictionary<short, string> nameData;
        private readonly Dictionary<short, ResDirectory> directoryData;
        private ResDirectory baseDir;

        public ResourceManageCore()
        {
            nameData = new Dictionary<short, string>();
            directoryData = new Dictionary<short, ResDirectory>();
        }

        public bool getFile(out ResFile result, string dir)
        {
            result = null;
            if (!dir.StartsWith("/"))
                return false;

            string[] dirs = dir.Split('/');

            ResDirectory resDir = baseDir;

            for (int level = 1; level < dirs.Length - 1; ++level)
            {
                if (!baseDir.getChildDir(out resDir, dirs[level]))
                    return false;
            }

            resDir.getChildFile(out result, dirs[dirs.Length - 1]);

            return true;
        }

        public bool getDirectory(out ResDirectory result, string dir)
        {
            result = null;
            if (!dir.StartsWith("/"))
                return false;

            string[] dirs = dir.Split('/');

            ResDirectory resDir = baseDir;

            for (int level = 1; level < dirs.Length - 1; ++level)
            {
                if (!baseDir.getChildDir(out resDir, dirs[level]))
                    return false;
            }

            resDir.getChildDir(out result, dirs[dirs.Length - 1]);

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void readResource(string resourceFileDir)
        {
            BinaryReader reader = new BinaryReader(File.Open(resourceFileDir, FileMode.Open));

            MemoryStream headerStream = FileManagerCore.ReadGameData(reader.BaseStream);
            BinaryReader headerReader = new BinaryReader(headerStream);

            long nameDataLen = headerReader.ReadInt64();
            FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);

            for (long i = 0; i < nameDataLen; ++i)
            {
                short id = headerReader.ReadInt16();
                long offset = headerReader.ReadInt64();
                long size = headerReader.ReadInt64();

                byte[] data = new byte[size];
                FileManagerCore.readBytes(data, offset, reader.BaseStream);

                nameData.Add(id, Encoding.UTF8.GetString(data));

                FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);
            }

            long directoryDataLen = headerReader.ReadInt64();
            FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);

            for (long i = 0; i < directoryDataLen; ++i)
            {
                short id = headerReader.ReadInt16();
                short parentID = headerReader.ReadInt16();
                short nameID = headerReader.ReadInt16();

                ResDirectory dir = new ResDirectory(id, nameData[nameID]);

                directoryData.Add(id, dir);

                if (directoryData.ContainsKey(parentID))
                {
                    directoryData[parentID].addChildDir(dir);
                }
                else
                {
                    baseDir = dir;
                }
                
                FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);
            }

            long fileDataLen = headerReader.ReadInt64();
            FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);
            for (long i = 0; i < fileDataLen; ++i)
            {
                short id = headerReader.ReadInt16();
                short parentID = headerReader.ReadInt16();
                short nameID = headerReader.ReadInt16();
                long offset = headerReader.ReadInt64();
                long size = headerReader.ReadInt64();

                byte[] data = new byte[size];
                FileManagerCore.readBytes(data, offset, reader.BaseStream);
                

                ResFile file = new ResFile(id, nameData[nameID], data);

                directoryData[parentID].addChildFile(file);

                FileManagerCore.byteBoundaryWork(headerReader.BaseStream, false);
            }
        }

        public void readResource()
        {
            readResource(FileManagerCore.dataPackageDir);
        }
    }
}
