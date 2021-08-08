using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Security.Permissions;
using System.Threading;
using static Matrix_App.Defaults;

namespace Matrix_App.adds
{
    public class PortCommandQueue
    {
        private const string ThreadDeliverName = "Arduino Port Deliver Thread";

        private readonly Queue<byte[]> byteWriteQueue = new();
        private readonly Queue<byte> statusByteQueue = new();

        private readonly Thread portDeliverThread;

        private readonly SerialPort tx;

        private bool running;

        private volatile bool kill;

        private volatile bool isPortValid;

        private readonly byte[] received = new byte[ArduinoReceiveBufferSize];
        private int mark;

        private volatile bool synchronized;
        private bool updateRealtime;

        public PortCommandQueue(ref SerialPort tx)
        {
            this.tx = tx;

            portDeliverThread = new Thread(ManageQueue) { Name = ThreadDeliverName };
        }

        private void ManageQueue()
        {
            try
            {
                while (!kill)
                {
                    if (!isPortValid) continue;
                    if (byteWriteQueue.Count <= 0)
                    {
                        lock (byteWriteQueue)
                        {
                            Monitor.Wait(byteWriteQueue);
                        }
                        continue;
                    }

                    byte[] bytes;
                    int statusByte;
                    lock (byteWriteQueue)
                    {
                        bytes = byteWriteQueue.Dequeue();
                        statusByte = statusByteQueue.Dequeue();
                    }

                    lock (tx)
                    {
                        var success = false;
                        var tryCounter = 0;
                        
                        do
                        {
                            try
                            {
                                tx.Write(bytes, 0, bytes.Length);

                                var b = ArduinoErrorByte;
                                var errorFlag = false;
                                mark = 0;
                                while (!errorFlag && (b = tx.ReadByte()) != statusByte)
                                {
                                    received[mark++] = (byte) b;

                                    errorFlag = b == ArduinoErrorByte;
                                }
                                synchronized = b == ArduinoSynchronizationByte;
                                success = !errorFlag;
                                Debug.WriteLine("===================> Com Success !");
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("=============================> ERROR <=================================");
                                Debug.WriteLine(e.Message);
                            }

                            tryCounter++;
                        } while (!success && tryCounter < 3);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Thread.CurrentThread.Interrupt();
            }
        }

        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
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
            finally
            {
                if (tx.IsOpen)
                {
                    tx.Close();
                }
            }
        }

        private void EnqueueArduinoCommand(params byte[] bytes)
        {
            if (!updateRealtime && bytes[0] != ArduinoInstruction.OpcodeScale &&
                bytes[0] != ArduinoInstruction.OpcodePush)
                return;
            
            if (!running)
            {
                running = true;
                portDeliverThread.Start();
            }
            
            lock (byteWriteQueue)
            {
                Monitor.Pulse(byteWriteQueue);
                if (byteWriteQueue.Count >= ArduinoCommandQueueSize) return;
                lock (byteWriteQueue)
                {
                    byteWriteQueue.Enqueue(bytes);
                }
            }
        }

        public void EnqueueArduinoCommand(byte opcode, params byte[] data)
        {
            byte[] wrapper = new byte[data.Length + 1];
            Buffer.BlockCopy(data, 0, wrapper, 1, data.Length);
            wrapper[0] = opcode;

            statusByteQueue.Enqueue(ArduinoSuccessByte);
            EnqueueArduinoCommand(wrapper);
        }

        public void DequeueAll()
        {
            lock (byteWriteQueue)
            {
                byteWriteQueue.Clear();
            }
        }

        public static void WaitForLastDequeue()
        {
            var timeCount = 0;
            var wait = true;
            
            while(wait)
            {
                timeCount++;
                Thread.Sleep(500);

                wait = timeCount == DequeueWaitTimeoutCounter;
            }
        }

        public byte[] GetLastData()
        {
            return received;
        }

        public void ValidatePort()
        {
            try
            {
                if (!tx.IsOpen)
                {
                    tx.Open();
                    isPortValid = true;
                }
            }
            catch (Exception e)
            {
                isPortValid = false;
                
                Debug.WriteLine("Failed opening port: " + e.Message);
            }
        }

        public void InvalidatePort()
        {
            isPortValid = false;
            tx.Close();
        }

        /// <summary>
        /// Returns last location of written byte in the received buffer.
        /// Call clears the mark. Any other call will return 0, unless the mark has been
        /// altered by reading new data from the serial port.
        /// </summary>
        /// <returns></returns>
        public int GetMark()
        {
            var tmp = mark;
            mark = 0;
            return tmp;
        }

        public void EnqueueArduinoCommandSynchronized(byte[] bytes)
        {
            statusByteQueue.Enqueue(ArduinoSynchronizationByte);
            EnqueueArduinoCommand(bytes);

            while (!synchronized)
            {
            }
            Debug.WriteLine("======================> Synchronized!");
            synchronized = false;
        }

        public void SetRealtimeUpdates(bool @checked)
        {
            updateRealtime = @checked;
        }

        public bool GetRealtimeUpdates()
        {
            return updateRealtime;
        }
    }
}
