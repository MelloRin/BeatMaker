using System.Collections.Generic;
using System.Linq;

namespace FileManager.lib.Directory
{
    class ResDirectory
    {
        public readonly short id;
        public readonly string dirName;

        private readonly Dictionary<string, ResDirectory> childDirs;
        private readonly Dictionary<string, ResFile> childFiles;

        public ResDirectory(short id, string dirName)
        {
            this.id = id;
            this.dirName = dirName;

            childDirs = new Dictionary<string, ResDirectory>();
            childFiles = new Dictionary<string, ResFile>();
        }

        public void addChildDir(ResDirectory dir)
        {
            childDirs.Add(dir.dirName, dir);
        }

        public void addChildFile(ResFile file)
        {
            childFiles.Add(file.name, file);
        }

        public string[] getChildDirList()
        {
            return childDirs.Keys.ToArray();
        }

        public string[] getChildFileList()
        {
            return childFiles.Keys.ToArray();
        }

        public bool getChildDir(out ResDirectory resDir, string childName)
        {
            if (childDirs.ContainsKey(childName))
            {
                resDir = childDirs[childName];
                return true;
            }

            resDir = null;
            return false;
        }

        public bool getChildFile(out ResFile resFile, string childName)
        {
            if (childFiles.ContainsKey(childName))
            {
                resFile = childFiles[childName];
                return true;
            }

            resFile = null;
            return false; 
        }
    }
}