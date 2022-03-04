using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformAutoConnectSerialPort
{
    public static class CrossThreadExtensions
    {
        /// <summary>
        /// Custom Invoke
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action"></param>
        public static void Invoke(this System.Windows.Forms.Control target, Action action)
        {
            if (target.InvokeRequired)
            {
                target.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
