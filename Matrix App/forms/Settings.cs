using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Matrix_App.adds;

namespace Matrix_App.forms
{
    public partial class Settings : Form
    {
        private readonly SerialPort port;
        
        private readonly PortCommandQueue queue;
        
        private readonly Regex comRegex = new Regex(@"COM([\d]+)");
        
        public Settings(ref PortCommandQueue queue, ref SerialPort port)
        {
            InitializeComponent();

            this.queue = queue;
            this.port = port;
            
            GatherPortNames(WritePortCombobox, port.PortName);

            UpdateRealtimeCheckbox.Checked = queue.GetRealtimeUpdates();
            
            Show();
        }

        private void UpdateRealtimeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            queue.SetRealtimeUpdates(UpdateRealtimeCheckbox.Checked);
        }
        
        private bool disableValidation;

        /// <summary>
        /// Gathers all available ports and sets them to the combobox
        /// </summary>
        [SuppressMessage("ReSharper", "CoVariantArrayConversion", Justification = "Never got an exception, so seems to be just fine")]
        private void GatherPortNames(ComboBox portBox, string initialCom)
        {
            disableValidation = true;
            
            var ports = SerialPort.GetPortNames();
            // save previously selected 
            var selected = portBox.SelectedItem ?? initialCom;

            if (!port.IsOpen)
            {
                selected = "None";
            }
            
            // get device names from ports
            var newPorts = GetDeviceNames(ports);
            // add virtual port
            newPorts.AddLast("None");

            // search for new port
            foreach (var newPort in newPorts)
            {
                // find any new port
                var found = portBox.Items.Cast<object?>().Any(oldPort => (string) oldPort! == newPort);

                // some port wasn't found, recreate list
                if (!found)
                {
                    portBox.Items.Clear();

                    portBox.Items.AddRange(newPorts.ToArray()!);

                    break;
                }
            }

            portBox.SelectedItem = portBox.Items.Count - 1;
            
            foreach (var item in portBox.Items)
            {
                if (item == null)
                    continue;

                if (!((string) item).Contains((string) selected)) 
                    continue;
                
                
                portBox.SelectedItem = item;
                GetPortInformation();
                disableValidation = false;
                return;
            }
            
            queue.InvalidatePort();
        }

        private static LinkedList<string> GetDeviceNames(IEnumerable<string> ports)
        {
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection devicePortNames = processClass.GetInstances();

            var newPorts = new LinkedList<string>();

            foreach (var currentPort in ports)
            {
                foreach (var o in devicePortNames)
                {
                    var name = ((ManagementObject) o).GetPropertyValue("Name");
                    
                    if (name == null || !name.ToString()!.Contains(currentPort)) 
                        continue;
                    
                    newPorts.AddLast(name.ToString()!);
                    break;
                }
            }

            return newPorts;
        }

        private void ValidatePortSelection(ComboBox box, SerialPort port)
        {
            if (disableValidation)
                return;
            
            lock (port)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                
                var item = (string) box.SelectedItem;
                {
                    // extract port
                    var matches = comRegex.Matches(item);
                        
                    if(matches.Count > 0)
                    {
                        // set valid port
                        port.PortName = matches[0].Value;
                        queue.ValidatePort();

                        GetPortInformation();
                    }
                    else
                    {
                        BluetoothFlagLabel.Text = @"-";
                        USBFlagLabel.Text = @"-";
                        UploadFlagLabel.Text = @"-";

                        ControllerNameLabel.Text = @"-";
                    }
                }
            }
            
        }

        private void GetPortInformation()
        {
            if (port.IsOpen)
            {
                try
                {
                    if (port.IsOpen)
                    {
                        port.Write(new byte[]{0x06}, 0, 1);

                        var @byte = port.ReadByte();
                        var flags = port.ReadByte();
                        var name = port.ReadTo("\x4B");
                            
                        BluetoothFlagLabel.Text = (flags & 0b10000000) == 0 ? "false" : "true";
                        USBFlagLabel.Text = (flags & 0b01000000) == 0 ? "false" : "true";
                        UploadFlagLabel.Text = (flags & 0b00000001) == 0 ? "false" : "true";

                        ControllerNameLabel.Text = name;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERROR! " + e.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in WritePortCombobox.Items)
            {
                if (item == null || (string) item == "None")
                    continue;

                var number = comRegex.Match(item as string).Groups[1].ToString();
                var portNumber = UInt32.Parse(number);

                var testPort = new SerialPort {PortName = "COM" + portNumber, ReadTimeout = 500, WriteTimeout = 500};
                try
                {
                    testPort.Open();
                    testPort.Write(new byte[] {0x06}, 0, 1);
                    var answer = testPort.ReadByte();
                    testPort.DiscardInBuffer();
                    testPort.DiscardOutBuffer();

                    if (answer == 91)
                    {
                        WritePortCombobox.SelectedIndex = WritePortCombobox.Items.Count - 1;
                        WritePortCombobox.SelectedItem = item;
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    testPort.Close();
                    testPort.Dispose();
                }
            }
        }

        private void WritePortCombobox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ValidatePortSelection(WritePortCombobox, port);
        }
    }
}