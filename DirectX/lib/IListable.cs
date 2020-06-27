namespace DirectX.lib
{
    public interface IListable
    {
        void add(string tag, ListData data);

        void delete(string tag);
    }
}
