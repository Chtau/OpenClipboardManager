using MahApps.Metro.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OCMApp.ApplicationError
{
    /// <summary>
    /// Interaction logic for ApplicationErrorWindow.xaml
    /// </summary>
    public partial class ApplicationErrorWindow : MetroWindow
    {
        public ApplicationErrorWindow()
        {
            InitializeComponent();
        }

        private void LogFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Helper.Folder.GetUserFolder());
            } catch (Exception ex)
            {
                Log.Error(ex, "Can't open log file");
            }
        }
    }
}
