using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using static Matrix_App.Defaults;

namespace Matrix_App
{
    public class PortCommandQueue
    {
        private const string ThreadDeliverName = "Arduino Port Deliver Thread";

        private Queue<byte[]> byteWriteQueue = new Queue<byte[]>();

        private Thread portDeliverThread;

        private SerialPort port;

        private bool running;

        private volatile bool kill;

        private volatile bool isPortValid;

        private readonly byte[] recived = new byte[ArduinoReceiveBufferSize];
        private int mark;
        
        public PortCommandQueue(ref SerialPort port)
        {
            this.port = port;

            portDeliverThread = new Thread(ManageQueue) { Name = ThreadDeliverName };
        }

        private void ManageQueue()
        {
            try
            {
                while (!kill)
                {
                    if (byteWriteQueue.Count <= 0) continue;
                    
                    byte[] bytes;
                    lock (byteWriteQueue)
                    {
                        bytes = byteWriteQueue.Dequeue();
                    }

                    lock (port)
                    {
                        if (!isPortValid) continue;
                        
                        port.Open();
                        port.Write(bytes, 0, bytes.Length);
                        
                        int b;
                        mark = 0;
                        while((b = port.ReadByte()) != ArduinoSuccessByte)
                        {
                            recived[mark++] = (byte) b;
                        }

                        port.Close();
                    }
                }
            } catch (ThreadInterruptedException)
            {
                Thread.CurrentThread.Interrupt();
            } 
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void Close()
        {
            try
            {
                if (!running) return;
                kill = true;
                portDeliverThread.Interrupt();
                portDeliverThread.Join(1000);
            }
            catch (ThreadStartException)
            {
                // omit
            }
            catch (ThreadInterruptedException)
            {
                // omit
            }
        }

        public void EnqueueArduinoCommand(params byte[] bytes)
        {
            if (!running)
            {
                running = true;
                portDeliverThread.Start();
            }

            lock (byteWriteQueue)
            {
                if (byteWriteQueue.Count < ArduinoCommandQueueSize)
                {
                    lock (byteWriteQueue)
                    {
                        byteWriteQueue.Enqueue(bytes);
                    }
                }
            }
        }

        public void EnqueueArduinoCommand(byte opcode, params byte[] data)
        {
            byte[] wrapper = new byte[data.Length + 1];
            Buffer.BlockCopy(data, 0, wrapper, 1, data.Length);
            wrapper[0] = opcode;

            EnqueueArduinoCommand(wrapper);
        }

        public void DequeueAll()
        {
            lock (byteWriteQueue)
            {
                byteWriteQueue.Clear();
            }
        }

        internal void WaitForLastDequeue()
        {
            int timeCount = 0;
            
            bool wait = true;
            while(wait)
            {
                timeCount++;
                Thread.Sleep(500);

                wait = timeCount == DequeueWaitTimeoutCounter;
            }
        }

        public byte[] GetLastData()
        {
            return recived;
        }

        public void ValidatePort()
        {
            isPortValid = true;
        }

        public void InvalidatePort()
        {
            isPortValid = false;
        }

        public int GetMark()
        {
            int tmp = mark;
            mark = 0;
            return tmp;
        }
    }
}
