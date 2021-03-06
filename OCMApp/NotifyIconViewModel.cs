﻿using OCMApp.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OCMApp
{
    public class NotifyIconViewModel : BaseViewModel
    {
        private ICommand _showWindowCommand;
        public ICommand ShowWindowCommand
        {
            get
            {
                if (_showWindowCommand == null)
                {
                    _showWindowCommand = new RelayCommand(
                        p => true,
                        p => ShowMainWindow());
                }
                return _showWindowCommand;
            }
        }

        private ICommand _showFavoriteWindowCommand;
        public ICommand ShowFavoriteWindowCommand
        {
            get
            {
                if (_showFavoriteWindowCommand == null)
                {
                    _showFavoriteWindowCommand = new RelayCommand(
                        p => true,
                        p => Internal.Global.Instance.ShowFavoritesWindow());
                }
                return _showFavoriteWindowCommand;
            }
        }

        private ICommand _exitApplicationCommand;
        public ICommand ExitApplicationCommand
        {
            get
            {
                if (_exitApplicationCommand == null)
                {
                    _exitApplicationCommand = new RelayCommand(
                        p => true,
                        p => ExitApplication());
                }
                return _exitApplicationCommand;
            }
        }

        private WeakReference<MainWindow> weakMainWindow;
        private void ShowMainWindow()
        {
            if (Helper.WindowCheck.IsWindowOpen<MainWindow>())
            {
                if (weakMainWindow != null && weakMainWindow.TryGetTarget(out MainWindow main))
                    main.Close();
            } else
            {
                var mainWindow = new MainWindow();
                weakMainWindow = new WeakReference<MainWindow>(mainWindow);
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        private void ExitApplication()
        {
#if DEBUG
            Internal.Global.Instance.Localize.GetMissingLocalization();
#endif
            System.Windows.Application.Current.Shutdown();
        }

        public NotifyIconViewModel()
        {
            if (Internal.Global.Instance.FirstStart)
            {
                ShowWindowCommand.Execute(null);
            }
        }
    }
}
