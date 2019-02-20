using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OCMApp.Blacklist
{
    /// <summary>
    /// Interaction logic for Blacklist.xaml
    /// </summary>
    public partial class Blacklist : MetroWindow
    {
        private readonly BlacklistViewModel _viewModel;

        public Blacklist()
        {
            _viewModel = new BlacklistViewModel();
            DataContext = _viewModel;

            InitializeComponent();
            _viewModel.RefreshCommand.Execute(null);
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputValue.Text))
            {
                await Internal.Global.Instance.DBContext.InsertBlacklist(new DAL.Models.Blacklist(inputValue.Text));
                inputValue.Text = null;
                _viewModel.RefreshCommand.Execute(null);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.DataContext != null)
            {
                if (button.DataContext is DAL.Models.Blacklist blacklist)
                {
                    Task.Run(() => Internal.Global.Instance.DBContext.DeleteBlacklist(blacklist)).Wait();
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
