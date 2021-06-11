using System.Collections.Generic;
using System.Threading;

namespace Matrix_App
{
    public class ThreadQueue
    {
        public delegate bool Task();

        private readonly Queue<Task> taskQueue = new Queue<Task>();

        private readonly Thread thread;

        private volatile bool running;
        private volatile bool working;

        private readonly int capacity;

        public ThreadQueue(string name, int capacity)
        {
            this.capacity = capacity;
            running = true;

            thread = new Thread(IdleQueue)
            {
                Name = name, 
                IsBackground = true, 
                Priority = ThreadPriority.Normal
            };
            thread.Start();
        }

        private void IdleQueue()
        {
            while(running)
            {
                Task? task = null;

                lock(taskQueue)
                {
                    if (taskQueue.Count > 0)
                    {
                        working = true;
                        task = taskQueue.Dequeue();
                    }
                    else
                    {
                        working = false;
                    }
                }

                if (task != null)
                {
                    running = task();
                } else
                {
                    try
                    {
                        Thread.Sleep(10);
                    } catch(ThreadInterruptedException)
                    {
                        thread.Interrupt();
                        running = false;
                    }
                }
            }
        }

        public void Enqueue(Task task)
        {
            lock (taskQueue)
            {
                if (taskQueue.Count < capacity)
                {
                    taskQueue.Enqueue(task);
                    working = true;
                }
            }
        }

        public bool HasWork()
        {
            return working;
        }

        public void Stop()
        {
            running = false;

            thread.Interrupt();
            thread.Join(1500);
        }
    }
}
