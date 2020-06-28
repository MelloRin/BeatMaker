using DirectX.lib.Interface;

namespace DirectX.util.Task
{
    internal class QueueData
    {
        private QueueData nextData = null;
        public ITask task { get; }

        public QueueData(ITask task) => this.task = task;

        public QueueData getNext() => nextData;
        public void setNext(QueueData next) => nextData = next;
    }
}
