using DirectX.lib.Interface;

namespace DirectX.util.Task
{
    public class TaskQueue
    {
        private QueueData head = null;
        private bool running = false;

        public void addTask(ITask data)
        {
            if (head == null)
            {
                if (running)
                    while (!running) ;

                running = true;
                head = new QueueData(data);

                QueueData temp = head;
                head = temp.getNext();

                temp.task.run(this);
                running = false;
            }
            else
            {
                QueueData temp = head;
                while (temp.getNext() != null)
                {
                    temp = temp.getNext();
                }
                temp.setNext(new QueueData(data));
            }
        }

        public void runNext()
        {
            if (head != null)
            {
                running = true;

                QueueData temp = head;
                head = temp.getNext();

                temp.task.run(this);
                running = false;
            }
        }
    }
}