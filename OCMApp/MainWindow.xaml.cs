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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OCMApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            InitializeComponent();

            MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem tab)
            {
                if (tab.Name == "TabText")
                {
                    _viewModel.ActiveTabRows = _viewModel.ClipDataTexts.Count;
                }
                else if (tab.Name == "TabImage")
                {
                    _viewModel.ActiveTabRows = _viewModel.ClipDataImages.Count;
                }
                else if (tab.Name == "TabFile")
                {
                    _viewModel.ActiveTabRows = _viewModel.ClipDataFiles.Count;
                }
                else if (tab.Name == "TabSummary")
                {
                    _viewModel.ActiveTabRows = _viewModel.Summary.Count;
                }
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsFlyout.IsOpen = !this.SettingsFlyout.IsOpen;
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            this.InfoFlyout.IsOpen = !this.InfoFlyout.IsOpen;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source is Button button && button.DataContext != null)
                {
                    if (button.DataContext is DAL.Models.ClipText textEntity)
                    {
                        Task.Run(() => Internal.Global.Instance.DBContext.DeleteClipText(textEntity)).Wait();
                        _viewModel.RefreshCommand.Execute(null);
                    }
                    else if (button.DataContext is DAL.Models.ClipImage imageEntity)
                    {
                        Task.Run(() => Internal.Global.Instance.DBContext.DeleteClipImage(imageEntity)).Wait();
                        _viewModel.RefreshCommand.Execute(null);
                    }
                    else if (button.DataContext is DAL.Models.ClipFile fileEntity)
                    {
                        Task.Run(() => Internal.Global.Instance.DBContext.DeleteClipFile(fileEntity)).Wait();
                        _viewModel.RefreshCommand.Execute(null);
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Delete DataGrid Item");
            }
        }

        private void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source is Button button && button.DataContext != null)
                {
                    if (button.DataContext is DAL.Models.ClipText textEntity)
                    {
                        Internal.Global.Instance.Clip.Post(textEntity.Value, OCMClip.ClipHandler.Entities.Enums.TextDataFormat.Text);
                    }
                    else if (button.DataContext is DAL.Models.ClipImage imageEntity)
                    {
                        Internal.Global.Instance.Clip.Post(OCMClip.ClipHandler.ConvertImage.ByteArrayToImage(imageEntity.Value));
                    }
                    else if (button.DataContext is DAL.Models.ClipFile fileEntity)
                    {
                        Internal.Global.Instance.Clip.Post(fileEntity.GetListValue());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post DataGrid Item to Clipboard");
            }
        }
    }
}
