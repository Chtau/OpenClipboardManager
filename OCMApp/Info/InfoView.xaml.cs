using OCMApp.Helper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCMApp.Info
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();

            Version.Text = GetVersion();
        }

        private string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.Major + "." +
                Assembly.GetEntryAssembly().GetName().Version.MajorRevision + "." +
                Assembly.GetEntryAssembly().GetName().Version.Minor;
        }

        private void WebsiteLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                if (e.Uri.IsFile)
                {
                    Folder.GetFolder(e.Uri.PathAndQuery);
                }
                System.Diagnostics.Process.Start(e.Uri.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not open Uri request");
            }
        }

        private void UserFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Internal.Global.Instance.AppUserFolder);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Can't open log file");
            }
        }
    }
}
