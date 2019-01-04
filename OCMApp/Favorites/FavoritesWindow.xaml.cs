using MahApps.Metro.Controls;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace OCMApp.Favorites
{
    /// <summary>
    /// Interaction logic for FavoritesWindow.xaml
    /// </summary>
    public partial class FavoritesWindow : MetroWindow
    {
        public FavoritesWindow()
        {
            InitializeComponent();
            OnLoadState();
            OnRefresh();
        }

        private void OnLoadState()
        {
            if (Internal.Global.Instance.FavoriteWindowState != null
                && Internal.Global.Instance.FavoriteWindowState.HasRememberState())
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Top = Internal.Global.Instance.FavoriteWindowState.Top.Value;
                this.Left = Internal.Global.Instance.FavoriteWindowState.Left.Value;
                this.Height = Internal.Global.Instance.FavoriteWindowState.Height.Value;
            }
        }

        private void OnSaveState()
        {
            if (Internal.Global.Instance.FavoriteWindowState != null)
            {
                Internal.Global.Instance.FavoriteWindowState.Top = this.Top;
                Internal.Global.Instance.FavoriteWindowState.Left = this.Left;
                Internal.Global.Instance.FavoriteWindowState.Height = this.Height;
                Internal.Global.Instance.SaveFavoriteWindowState();
            }
        }

        private void OnRefresh()
        {
            this.Dispatcher.Invoke(() =>
            {
                var result = Internal.Global.Instance.FavoriteItems;
                FavoritesWrapper.Children.Clear();
                foreach (FavoriteItemViewModel item in result)
                {
                    FavoritesWrapper.Children.Add(
                    new FavoriteItem
                    {
                        DataContext = item
                    }
                    );
                }
            });
        }

        public void Refresh()
        {
            OnRefresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            OnSaveState();
            base.OnClosing(e);
        }
    }
}
