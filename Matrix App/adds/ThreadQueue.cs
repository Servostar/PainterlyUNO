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
                lock (taskQueue)
                {
                    working = taskQueue.Count > 0;
                }

                if (working)
                {
                    try
                    {
                        Task task;
                        lock (taskQueue)
                        {
                            task = taskQueue.Dequeue();
                        }

                        running = task();
                        
                        lock (taskQueue)
                        {
                            working = taskQueue.Count > 0;
                        }
                    } catch(ThreadInterruptedException)
                    {
                        thread.Interrupt();
                        running = false;
                    }
                }
                else
                {
                    lock (taskQueue)
                    {
                        Monitor.Wait(taskQueue);
                    }
                }
            }
        }

        public void Enqueue(Task task)
        {
            lock (taskQueue)
            {
                if (taskQueue.Count >= capacity) 
                    return;
             
                Monitor.Pulse(taskQueue);
                
                working = true;
                taskQueue.Enqueue(task);
            }
        }

        public bool HasWork()
        {
            return working;
        }

        public void Stop()
        {
            lock (taskQueue)
            {
                Monitor.Pulse(taskQueue);
            }
            
            running = false;

            thread.Interrupt();
            thread.Join(100);
        }
    }
}
