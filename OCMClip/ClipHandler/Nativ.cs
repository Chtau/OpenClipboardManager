using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal static class Nativ
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Gets the Windows Title and the Process of the current foreground Window
        /// </summary>
        /// <returns>Window Title and Process instance</returns>
        public static Tuple<string, Process> GetActiveWindow()
        {
            try
            {
                string windowTitle = null;
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);

                IntPtr hwnd = GetForegroundWindow();
                if (GetWindowText(hwnd, Buff, nChars) > 0)
                {
                    windowTitle = Buff.ToString();
                }
                GetWindowThreadProcessId(hwnd, out uint pid);

                return new Tuple<string, Process>(windowTitle, Process.GetProcessById((int)pid));
            }
            catch (Exception ex)
            {
                Manager.logger.Error(ex);
                return null;
            }
        }

        public static string GetActiveWindowTitle()
        {
            try
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();

                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    return Buff.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                Manager.logger.Error(ex);
                return null;
            }
        }


        public static Process GetActiveProcess()
        {
            try
            {
                IntPtr hwnd = GetForegroundWindow();
                GetWindowThreadProcessId(hwnd, out uint pid);
                return Process.GetProcessById((int)pid);
            }
            catch (Exception ex)
            {
                Manager.logger.Error(ex);
                return null;
            }
        }
    }
}
