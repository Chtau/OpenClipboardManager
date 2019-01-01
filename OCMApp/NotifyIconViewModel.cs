using OCMApp.Internal;
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

        private MainWindow _mainWindow;
        private MainWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                    _mainWindow = new MainWindow();
                return _mainWindow;
            }
            set
            {
                _mainWindow = value;
            }
        }

        private void ShowMainWindow()
        {
            if (Helper.WindowCheck.IsWindowOpen<MainWindow>())
            {
                MainWindow.Close();
                MainWindow = null;
            }
            else
            {
                if (MainWindow.Visibility == System.Windows.Visibility.Visible)
                {
                    MainWindow = null;
                }
                MainWindow.Show();
                MainWindow.Activate();
            }
        }

        private void ExitApplication()
        {
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
