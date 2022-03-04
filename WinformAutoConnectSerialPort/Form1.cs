using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformAutoConnectSerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SerialPortAuto.Initialize();
            SerialPortAuto.StatusChanged += SerialPortAuto_StatusChanged;
            SerialPortAuto.DataReceived += SerialPortAuto_DataReceived;
        }

        private void SerialPortAuto_DataReceived(string message)
        {
            textBox2.Invoke(() =>
            {
                textBox2.Text = message;
            });
        }

        private void SerialPortAuto_StatusChanged(bool connected, string status)
        {
            textBox1.Invoke(() =>
            {
                textBox1.Text = status;
            });
        }
    }
}
