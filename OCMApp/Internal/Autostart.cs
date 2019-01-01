using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Internal
{
    public static class Autostart
    {
        public static bool InstallMeOnStartUp()
        {
            try
            {
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                string name = curAssembly.GetName().Name;
                Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (key1.GetValue(name) == null)
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.SetValue(name, curAssembly.Location);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Registry Key could not be set");
                return false;
            }
        }

        public static bool RemoveMeOnStartUp()
        {
            try
            {
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                string name = curAssembly.GetName().Name;
                Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                if (key1.GetValue(name) != null)
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.DeleteValue(name);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Registry Key could not be removed");
                return false;
            }
        }
    }
}
