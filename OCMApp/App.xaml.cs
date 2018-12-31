using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OCMApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Hardcodet.Wpf.TaskbarNotification.TaskbarIcon NotifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            Internal.Global.Instance.Init();
            if (Internal.Global.Instance.InitError)
            {
                StartupUri = new Uri("/OCMApp;component/ApplicationError/ApplicationErrorWindow.xaml", UriKind.Relative);
            } else
            {
                NotifyIcon = (Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)FindResource("NotifyIcon");
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Internal.Global.Instance.Close();
            NotifyIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
