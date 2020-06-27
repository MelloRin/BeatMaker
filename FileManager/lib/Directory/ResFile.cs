namespace FileManager.lib.Directory
{
    public class ResFile
    {
        public readonly byte[] rawData;
        public readonly string name;
        public readonly short id;

        public ResFile(short id, string name, byte[] rawData)
        {
            this.id = id;
            this.name = name;
            this.rawData = rawData;
        }
    }
}