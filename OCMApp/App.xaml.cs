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
        protected override void OnStartup(StartupEventArgs e)
        {
            Internal.Global.Instance.Init();
            if (Internal.Global.Instance.InitError)
            {
                StartupUri = new Uri("/OCMApp;component/ApplicationError/ApplicationErrorWindow.xaml", UriKind.Relative);
            }
            base.OnStartup(e);
        }
    }
}
