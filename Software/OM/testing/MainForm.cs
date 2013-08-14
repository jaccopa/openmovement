﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OmApiNet;

namespace OMTesting
{
    public partial class MainForm : Form
    {
        protected const int MIN_BATTERY_TO_CONFIGURE = 85;

        // Device status tracker
        class DeviceStatus
        {
            // Create the Panopticon command-line-interface wrapper
            Configure configure;

            public enum DeviceMode { Unknown = -2, Unexpected = -1, Waiting = 0, Charging = 1, Configuring = 2, Complete = 3, Failed = 4, Error = 5 };
            public DeviceStatus(int id) 
            { 
                string configCommand = @"omapi-examples.exe";
                string args = "record -id $id";
                string basePath;

                BatteryLevel = -1;

                //basePath = AppDomain.CurrentDomain.BaseDirectory;
                basePath = Path.GetDirectoryName(Application.ExecutablePath);
                configCommand = Path.Combine(basePath, configCommand);

                configure = new Configure(configCommand, args);
                configure.Completed += configure_Completed;
                this.Id = id; 
                this.Mode = DeviceMode.Unknown;
                this.Info = "";
            }

            public int Id { get; protected set; }
            public DeviceMode Mode { get; set; }
            public string Info { get; set; }
            public int BatteryLevel { get; protected set; }
            public bool Changed { get; set; }

            public string Label
            {
                get
                {
                    if (Mode == DeviceMode.Failed) { return "Failed (" + Info + ")"; }
                    else if (Mode == DeviceMode.Error) { return "Error, retry? (" + Info + ")"; }
                    else if (Mode == DeviceMode.Charging && BatteryLevel >= 0) { return "Charging (" + BatteryLevel + "%)"; }
                    return Mode.ToString();
                }
            }

            // Is the device expected?
            public bool Expected
            { 
                get 
                { 
                    return !(Mode == DeviceMode.Unknown || Mode == DeviceMode.Unexpected); 
                }
                set
                {
                    // Only setting true (only remove if not attached and not expected)
                    if (value)
                    {
                        if (Mode == DeviceMode.Unknown) { Mode = DeviceMode.Waiting; }
                        else if (Mode == DeviceMode.Unexpected) { Mode = DeviceMode.Charging; }
                    }
                }
            }

            // Is the device connected?
            public bool Connected 
            { 
                get 
                { 
                    return !(Mode == DeviceMode.Unknown || Mode == DeviceMode.Waiting); 
                }
                set
                {
                    // Only setting true (on disconnect, remove from list)
                    if (value)
                    {
                        if (Mode == DeviceMode.Unknown) { Mode = DeviceMode.Unexpected; }
                        else if (Mode == DeviceMode.Waiting) { Mode = DeviceMode.Charging; }
                    }
                }
            }

            public OmDevice GetDevice()
            {
                OmDevice omdev = Om.Instance.GetDevice(Id);
                return omdev; 
            }

            private DateTime lastUpdatedBattery = DateTime.MinValue;

            public bool Update(bool batteryInteresting)
            {
                bool ret;
                OmDevice omdev = GetDevice();

                if (omdev == null) { return false; }
                ret = omdev.Update();

                if (batteryInteresting && lastUpdatedBattery == DateTime.MinValue || DateTime.Now - lastUpdatedBattery > TimeSpan.FromSeconds(5))
                {
                    int batteryLevel = OmApi.OmGetBatteryLevel(Id);
                    if (batteryLevel != this.BatteryLevel)
                    {
                        this.BatteryLevel = batteryLevel;
                        ret = true;
                    }
                }

                return ret;
            }

            public bool CanConfigure
            {
                get
                {
                    if (Mode != DeviceStatus.DeviceMode.Charging) { return false; }
                    OmDevice omdev = GetDevice();
                    if (omdev == null) { return false; }
                    if (omdev.BatteryLevel < MIN_BATTERY_TO_CONFIGURE) { return false;  }
                    return true;
                }
            }


            public bool StartConfigure()
            {
                // See if a charging device should be configured
                if (Mode != DeviceStatus.DeviceMode.Charging) { return false; }

                Mode = DeviceStatus.DeviceMode.Configuring;

                // Begin background configure
                configure.ConfigureAsync(Id);

                return true;
            }

            void configure_Completed(object sender, Configure.CompletedEventArgs e)
            {
                string label;
                string info;
                if (e.Error != null)        // Internal exception
                {
                    label = "ERROR";
                    info = e.Error.ToString();
                    Mode = DeviceStatus.DeviceMode.Error;
                }
                else if (e.ExitCode < 0)   // External error code
                {
                    label = "ERROR";
                    info = "" + e.ExitCode + " " + (e.ErrorMessages == null ? "" : e.ErrorMessages);
                    Mode = DeviceStatus.DeviceMode.Error;
                }
                else if (e.ExitCode > 0)   // 
                {
                    label = "FAIL";
                    info = "" + e.Error + " " + (e.ErrorMessages == null ? "" : e.ErrorMessages);
                    Mode = DeviceStatus.DeviceMode.Failed;
                    OmDevice omdev = GetDevice();
                    if (omdev != null) { omdev.SetLed(OmApi.OM_LED_STATE.OM_LED_BLUE); }
                }
                else
                {
                    label = "OK";
                    info = "";
                    Console.WriteLine("Configuration stopped - OK");
                    Mode = DeviceStatus.DeviceMode.Complete;
                    OmDevice omdev = GetDevice();
                    if (omdev != null) { omdev.SetLed(OmApi.OM_LED_STATE.OM_LED_MAGENTA); }
                }
                this.Info = info;
                Console.WriteLine("Configuration stopped - " + label + "," + info);
                Log("" + DateTime.Now.ToString() + "," + Id + "," + label);
                Changed = true;
            }

        }

        // Keep device status for each device
        IDictionary<int, DeviceStatus> devices = new Dictionary<int, DeviceStatus>();

        // Log file
        protected static string LOG_FILE = "log.csv";

        // Log a message
        public static bool Log(string entry)
        {
            try
            {
                string appendText = entry + Environment.NewLine;
                File.AppendAllText(LOG_FILE, appendText);
                return true;
            }
            catch
            {
                return false;
            }
        }
       


        public MainForm(string loadFile = null)
        {
            InitializeComponent();

            (new TextBoxStreamWriter(textBoxLog)).SetConsoleOut();

            Console.WriteLine("Started.");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Om.Instance.DeviceAttached += Instance_DeviceAttached;
            Om.Instance.DeviceRemoved += Instance_DeviceRemoved;
            Om.Instance.ForAllAttachedDevices(Instance_DeviceAttached);
        }

        void Instance_DeviceAttached(object sender, OmApiNet.OmDeviceEventArgs e)
        {
            this.BeginInvoke(new Om.OmDeviceEventHandler(AddDevice), new object[] { sender, e });
        }

        void Instance_DeviceRemoved(object sender, OmApiNet.OmDeviceEventArgs e)
        {
            this.BeginInvoke(new Om.OmDeviceEventHandler(RemoveDevice), new object[] { sender, e });
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // KeyPress handler takes care of these -- don't let another control see them
            if ((e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9) || e.KeyValue == 8)
            {
                e.Handled = true;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            // KeyPress handler takes care of these -- don't let another control see them
            if ((e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9) || e.KeyValue == 8)
            {
                e.Handled = true;
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            // KeyPress handler to control FormattedNumericUpDown control
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                int c = (e.KeyChar - '0');
                formattedNumericUpDownId.AddDigit(c);
                e.Handled = true;
            }
            if ((e.KeyChar == '+' || e.KeyChar == '=') && formattedNumericUpDownId.Value < formattedNumericUpDownId.Maximum)
            {
                formattedNumericUpDownId.Value = formattedNumericUpDownId.Value + 1;
                e.Handled = true;
            }
            if (e.KeyChar == '-' && formattedNumericUpDownId.Value > formattedNumericUpDownId.Minimum)
            {
                formattedNumericUpDownId.Value = formattedNumericUpDownId.Value - 1;
                e.Handled = true;
            }
            if (e.KeyChar == 8) // Backspace
            {
                formattedNumericUpDownId.RemoveDigit();
                e.Handled = true;
            }
        }

        // Re-enable 'add' when number incremented
        private void formattedNumericUpDownId_ValueChanged(object sender, EventArgs e)
        {
            buttonAdd.Enabled = true;
        }





        private void AddExpectedDevice(int id)
        {
            // Add to list if not present, get from list
            DeviceStatus device;
            if (!devices.ContainsKey(id))
            {
                devices[id] = new DeviceStatus(id);
            }
            device = devices[id];

            // Set expected flag
            if (device.Expected)
            {
                MessageBox.Show(this, "Device ID " + id + " already expected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                device.Expected = true;
            }

            UpdateDevices();
        }

        private void RemoveExpectedDevice(int id)
        {
            // Remove a device only if it's not connected
            if (devices.ContainsKey(id) && !devices[id].Connected)
            {
                devices.Remove(id);
            }
            UpdateDevices();
        }

        private void AddDevice(object sender, OmApiNet.OmDeviceEventArgs e)
        {
            int id = e.Device.DeviceId;
            DeviceStatus device;

            if (!devices.ContainsKey(id))
            {
                devices[id] = new DeviceStatus(id);
            }
            device = devices[id];

            if (device.Connected)
            {
                Console.WriteLine("WARNING: Device ID " + id + " already connected!");
                return;
            }

            device.Connected = true;
            UpdateDevices();
        }

        private void RemoveDevice(object sender, OmApiNet.OmDeviceEventArgs e)
        {
            int id = e.Device.DeviceId;
            if (devices.ContainsKey(id))
            {
                // Always remove device
                devices.Remove(id);
            }
            UpdateDevices();
        }


        public void UpdateDevices()
        {
            //listViewDevices.Items.Clear();

            // Remove any devices that are no longer there
            ListViewItem[] currentItems = new ListViewItem[listViewDevices.Items.Count];
            listViewDevices.Items.CopyTo(currentItems, 0);
            foreach (ListViewItem item in currentItems)
            {
                if (!devices.ContainsKey((int)item.Tag))
                {
                    listViewDevices.Items.RemoveByKey(item.Name);
                }
            }

            // Update/add any new devices
            foreach (DeviceStatus device in devices.Values)
            {
                // Add a new item
                if (!listViewDevices.Items.ContainsKey(device.Id.ToString()))
                {
                    ListViewItem newItem;
                    string[] parts = new string[2];
                    parts[0] = "" + device.Id;
                    parts[1] = "-";
                    newItem = new ListViewItem(parts);
                    newItem.Tag = device.Id;
                    newItem.Name = device.Id.ToString();   // Name -> Key
                    listViewDevices.Items.Add(newItem);
                }

                // Get item, update label if needed
                ListViewItem item = listViewDevices.Items[device.Id.ToString()];
                if (item.SubItems[1].Text != device.Label)
                {
                    item.SubItems[1].Text = device.Label;
                }
            }

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            buttonAdd.Enabled = false;
            formattedNumericUpDownId.WasSubmitted();
            int id = (int)formattedNumericUpDownId.Value;
            if (id < 1)
            {
                MessageBox.Show(this, "ID cannot be less than 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (id > 32767)
            {
                MessageBox.Show(this, "ID cannot be greater than 32767", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AddExpectedDevice(id);
            }
            UpdateDevices();
        }


        private void listViewDevices_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            bool allUserAdded = (listViewDevices.SelectedIndices.Count > 0);
            bool allConnected = (listViewDevices.SelectedIndices.Count > 0);

            foreach (int i in listViewDevices.SelectedIndices)
            {
                int id = (int)listViewDevices.Items[i].Tag;
                if (devices.ContainsKey(id) && devices[id].Connected) { allUserAdded = false; }
                if (devices.ContainsKey(id) && !devices[id].Connected) { allConnected = false; }
            }

            buttonRemove.Enabled = allUserAdded;
            buttonIdentify.Enabled = allConnected;
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (int i in listViewDevices.SelectedIndices)
            {
                int id = (int)listViewDevices.Items[i].Tag;
                RemoveExpectedDevice(id);
            }
        }



        int checkIndex = 0;
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            int count = listViewDevices.Items.Count;
            bool changed = false;

            for (int i = 0; i < count; i++)
            {
                int testId = (int)listViewDevices.Items[(checkIndex + i) % count].Tag;
                DeviceStatus device = devices[testId];

                if (device.Changed) { changed = true; device.Changed = false; }

                if (device.Update(device.Mode == DeviceStatus.DeviceMode.Charging)) { changed = true; }

                // See if a charging device should be configured
                if (device.Mode == DeviceStatus.DeviceMode.Charging && device.CanConfigure)
                {
                    device.StartConfigure();
                    changed = true;
                }
            }

            if (changed) { UpdateDevices(); }
        }


        private void timerIdentify_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewDevices.Items.Count; i++)
            {
                int id = (int)listViewDevices.Items[i].Tag;
                DeviceStatus device = devices[id];

                if (identify.ContainsKey(id))
                {
                    int value = identify[id];
                    identify[id] = value - 1;
                    OmApi.OM_LED_STATE led;

                    if (value == 0)
                    {
                        identify.Remove(id);

                        // Emulate setup task LED colours
                        if (device.Mode == DeviceStatus.DeviceMode.Complete) { led = OmApi.OM_LED_STATE.OM_LED_MAGENTA; }
                        else if (device.Mode == DeviceStatus.DeviceMode.Failed) { led = OmApi.OM_LED_STATE.OM_LED_BLUE; }
                        else { led = OmApi.OM_LED_STATE.OM_LED_AUTO; }
                    }
                    else
                    {
                        bool phase = (value & 1) != 0;
                        led = phase ? OmApi.OM_LED_STATE.OM_LED_WHITE : OmApi.OM_LED_STATE.OM_LED_OFF;
                    }

                    if (device.Connected)
                    {
                        device.GetDevice().SetLed(led);
                    }
                }

            }
        }


        IDictionary<int, int> identify = new Dictionary<int, int>();
        const int IDENTIFY_COUNT = 12;

        private void buttonIdentify_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewDevices.SelectedItems)
            {
                int id = (int)item.Tag;
                if (identify.ContainsKey(id)) { identify[id] = IDENTIFY_COUNT; }
                else { identify.Add(id, IDENTIFY_COUNT); }
            }
        }


    }
}
