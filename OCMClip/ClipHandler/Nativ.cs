using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(IntPtr b1, IntPtr b2, IntPtr count);

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

        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, (IntPtr)len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        public static bool CompareMemCmp(Image b1, Image b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            return CompareMemCmp(new Bitmap(b1), new Bitmap(b2));
        }
    }
}
