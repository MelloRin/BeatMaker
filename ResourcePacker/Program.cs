using System;
using System.IO;

namespace ResourcePacker
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitPacker unitPacker = new UnitPacker("..\\..\\..\\res");

            byte[] result = unitPacker.getPackedData();

            File.WriteAllBytes(FileManager.FileManagerCore.resourceFileName, result);

            Console.ReadKey();
        }
    }
}