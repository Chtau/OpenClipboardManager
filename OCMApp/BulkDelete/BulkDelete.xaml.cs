using MahApps.Metro.Controls;
using OCMApp.Internal;
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

namespace OCMApp.BulkDelete
{
    /// <summary>
    /// Interaction logic for BulkDelete.xaml
    /// </summary>
    public partial class BulkDelete : MetroWindow
    {
        public BulkDelete()
        {
            InitializeComponent();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            string appNameValue = appName.Text;
            if (string.IsNullOrWhiteSpace(appNameValue))
                appNameValue = null;
            if (appNameValue != null || beforeDate.SelectedDate != null)
            {
                if (await Global.Instance.DBContext.ItemsDelete(appNameValue, beforeDate.SelectedDate))
                {
                    itemsLabel.Content = 0;
                    appName.Text = null;
                    beforeDate.SelectedDate = null;
                }
            }
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            string appNameValue = appName.Text;
            if (string.IsNullOrWhiteSpace(appNameValue))
                appNameValue = null;
            if (appNameValue != null || beforeDate.SelectedDate != null)
            {
                int items = await Global.Instance.DBContext.ItemsToDelete(appNameValue, beforeDate.SelectedDate);
                itemsLabel.Content = items;
            }
        }
    }
}
