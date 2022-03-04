using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WinformAutoConnectSerialPort
{
    public static class SerialPortAuto
    {
        //private static Array myPort; // COM Port yang terdeteksi pada sistem akan disimpan disini
        private static SerialPort _serialPort;
        public static string Port { get; set; }
        public delegate void SerialPortEventHandler(bool connected, string status);
        public static event SerialPortEventHandler StatusChanged;
        public delegate void SerialPortDataReceivedEventHandler(string message);
        public static event SerialPortDataReceivedEventHandler DataReceived;
        private static System.Timers.Timer _timerFirstConnect;
        private static System.Timers.Timer _timerStandby;

        public static void Initialize()
        {
            _serialPort = new SerialPort();
            // connect PLC first when startup app
            _timerFirstConnect = new System.Timers.Timer();
            _timerFirstConnect.Interval = 1000;
            _timerFirstConnect.Elapsed += _timerFirstConnect_Elapsed;
            
            _timerFirstConnect.Enabled = true;

            _timerStandby = new System.Timers.Timer();
            _timerStandby.Interval = 1000;
            _timerStandby.Elapsed += _timerStandby_Elapsed;
        }

        private static void _timerStandby_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("_serialPort.IsOpen : " + _serialPort.IsOpen);
            if (!_serialPort.IsOpen)
            {
                Port = "";
                if (StatusChanged != null)
                    StatusChanged(false, "Disconnected serial port");
                _timerStandby.Enabled = false;
                _serialPort = new SerialPort();
                _timerFirstConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Auto check if port available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void _timerFirstConnect_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timerFirstConnect.Enabled = false;
            try
            {
                // Gets an array of serial port names for the current computer.
                var myPort = SerialPort.GetPortNames();
                System.Diagnostics.Debug.WriteLine("port available : " + myPort.Length);

                if (myPort.Length > 0)
                {
                    _timerFirstConnect.Enabled = false;

                    Port = myPort[0];
                    
                    _serialPort.PortName = myPort[0];
                    _serialPort.BaudRate = 9600;
                    _serialPort.DataBits = 8;
                    _serialPort.Parity = Parity.None;
                    _serialPort.StopBits = StopBits.One;

                    _serialPort.Open();
                    _serialPort.DataReceived += _serialPort_DataReceived;


                    if (StatusChanged != null)
                        StatusChanged(true, "Connected to port: " + myPort[0]);

                    _timerStandby.Enabled = true;


                }
                else
                {
                    if (StatusChanged != null)
                        StatusChanged(false, "No Serial Port Available");
                    Port = "";
                    _timerFirstConnect.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                if (StatusChanged != null)
                    StatusChanged(false, ex.Message);
                Port = "";
                _timerFirstConnect.Enabled = true;
                //cmbSerialPort.Text = string.Empty;
            }
        }

        private static void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var message = _serialPort.ReadLine();
            if (DataReceived != null)
                DataReceived(message);
        }

    }

}

