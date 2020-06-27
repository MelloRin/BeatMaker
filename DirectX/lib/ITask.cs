using DirectX.Lib;

namespace DirectX.lib
{
    public interface ITask
    {
        void run(TaskQueue taskQueue);
        void initialize();
    }
}
