﻿using System;
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

namespace OCMApp.Settings
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsView()
        {
            _viewModel = new SettingsViewModel();
            DataContext = _viewModel;
            
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Settings.ClipWatcherDefaultImageFormat = (OCMClip.ClipHandler.Entities.Enums.ImageFormatType)_viewModel.ClipWatcherImageFormatTypeEnumSelected;
            _viewModel.Settings.Culture = (Internal.Localize.Language)_viewModel.CultureEnumSelected;
            _viewModel.Settings.Accent = (Internal.Theme.Accent)_viewModel.AccentEnumSelected;
            _viewModel.Settings.ThemeColor = (Internal.Theme.ThemeColor)_viewModel.ThemeColorEnumSelected;

            if (!_viewModel.Settings.UseWatcher)
            {
                _viewModel.Settings.ClipKey = _viewModel.ClipHotKey.GetKey();
                _viewModel.Settings.ClipKeyModifier = _viewModel.ClipHotKey.GetModifier();
            }
            _viewModel.Settings.ClipPostKey = _viewModel.ClipPasteHotKey.GetKey();
            _viewModel.Settings.ClipPostKeyModifier = _viewModel.ClipPasteHotKey.GetModifier();

            Internal.Global.Instance.SaveSettings(_viewModel.Settings);
        }
    }
}
