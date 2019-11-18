using System;
using System.IO;
using System.Text;

namespace MelloRin.ResourcePacker
{
    class FileListReader
    {
        static void searchDir(string targetDir, string prevDir, int level)
        {
            try
            {
                Console.WriteLine(setBlank(level, true) + cutRestDir(targetDir, prevDir));

                foreach (string currentFile in Directory.EnumerateFiles(targetDir, "*"))
                {
                    string fileName = cutRestDir(currentFile, targetDir);

                    Console.WriteLine(setBlank(level, false) + fileName);
                }

                foreach (string currentDir in Directory.EnumerateDirectories(targetDir, "*"))
                {
                    searchDir(currentDir, targetDir, level + 1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static string cutRestDir(string target, string currentDir)
        {
            if (target.Length == currentDir.Length)
                return "\\";

            return target.Substring(currentDir.Length + 1);
        }


        static string setBlank(int level, bool isDir)
        {
            StringBuilder blankBuilder = new StringBuilder();

            for (int i = 0; i < level; ++i)
                blankBuilder.Append("\t");

            if(!isDir)
                blankBuilder.Append("└ ");

            return blankBuilder.ToString();
        }
	}
}