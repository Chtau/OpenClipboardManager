using OCMApp.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OCMApp
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<DAL.Models.ClipText> clipDataTexts;
        public ObservableCollection<DAL.Models.ClipText> ClipDataTexts
        {
            get { return clipDataTexts; }
            set
            {
                clipDataTexts = value;
                RaisePropertyChanged("ClipDataTexts");
            }
        }

        private ObservableCollection<DAL.Models.ClipImage> clipDataImages;
        public ObservableCollection<DAL.Models.ClipImage> ClipDataImages
        {
            get { return clipDataImages; }
            set
            {
                clipDataImages = value;
                RaisePropertyChanged("ClipDataImages");
            }
        }

        private ObservableCollection<DAL.Models.ClipFile> clipDataFiles;
        public ObservableCollection<DAL.Models.ClipFile> ClipDataFiles
        {
            get { return clipDataFiles; }
            set
            {
                clipDataFiles = value;
                RaisePropertyChanged("ClipDataFiles");
            }
        }

        public MainWindowViewModel()
        {
            RefreshCommand.Execute(null);
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        p => true,
                        async p => await OnRefresh());
                }
                return _refreshCommand;
            }
        }

        private async Task OnRefresh()
        {
            ClipDataTexts = new ObservableCollection<DAL.Models.ClipText>(await Internal.Global.Instance.DBContext.GetClipText());
            ClipDataImages = new ObservableCollection<DAL.Models.ClipImage>(await Internal.Global.Instance.DBContext.GetClipImage());
            ClipDataFiles = new ObservableCollection<DAL.Models.ClipFile>(await Internal.Global.Instance.DBContext.GetClipFile());
        }
    }
}
