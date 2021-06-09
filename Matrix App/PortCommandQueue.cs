using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using static MatrixDesigner.Defaults;

namespace Matrix_App
{
    public class PortCommandQueue
    {
        private const string threadDeliverName = "Arduino Port Deliver Thread";

        private Queue<byte[]> byteWriteQueue = new Queue<byte[]>();

        private Thread portDeliverThread;

        private SerialPort port;

        private bool running = false;

        private volatile bool kill = false;

        private volatile bool isPortValid = false;

        private byte[] recived = new byte[ARDUINO_RECIVCE_BUFFER_SIZE];
        private int mark;
        
        public PortCommandQueue(ref SerialPort port)
        {
            this.port = port;

            portDeliverThread = new Thread(new ThreadStart(ManageQueue));
            portDeliverThread.Name = threadDeliverName;
        }

        private void ManageQueue()
        {
            try
            {
                while (!kill)
                {
                    if (byteWriteQueue.Count > 0)
                    {
                        byte[] bytes;
                        lock (byteWriteQueue)
                        {
                            bytes = byteWriteQueue.Dequeue();
                        }

                        lock (port)
                        {
                            if (isPortValid)
                            {
                                port.Open();
                                port.Write(bytes, 0, bytes.Length);

                                int b;
                                mark = 0;
                                while((b = port.ReadByte()) != ARDUINO_SUCCESS_BYTE)
                                {
                                    recived[mark++] = (byte) b;
                                }

                                port.Close();
                            }
                        }
                    }
                }
            } catch (ThreadInterruptedException)
            {
                Thread.CurrentThread.Interrupt();
                return;
            }
            catch (Exception)
            {
                // omit
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public void Close()
        {
            try
            {
                if (running)
                {
                    kill = true;
                    portDeliverThread.Interrupt();
                    portDeliverThread.Join(1000);
                }
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

            if (byteWriteQueue.Count < ARDUINO_COMMAND_QUEUE_SIZE)
            {
                lock (byteWriteQueue)
                {
                    byteWriteQueue.Enqueue(bytes);
                }
            }
        }

        public void EnqueueArduinoCommand(byte opcode, params byte[] data)
        {
            byte[] wrapper = new byte[data.Length + 1];
            System.Buffer.BlockCopy(data, 0, wrapper, 1, data.Length);
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
                lock(byteWriteQueue)
                {
                    wait = byteWriteQueue.Count != 0;
                }
                timeCount++;
                Thread.Sleep(500);

                wait = timeCount == DEQUEUE_WAIT_TIMEOUT_COUNTER;
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
