using DirectX.util.Task;

namespace DirectX.lib.Interface
{
    public interface ITask
    {
        void run(TaskQueue taskQueue);
        void initialize();
    }
}