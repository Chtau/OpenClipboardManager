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
    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        public enum Tabs
        {
            Text,
            Image,
            File,
            Summary
        }

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

        private ObservableCollection<DAL.Models.Summary> summary;
        public ObservableCollection<DAL.Models.Summary> Summary
        {
            get { return summary; }
            set
            {
                summary = value;
                RaisePropertyChanged("Summary");
            }
        }

        private int activeTabRows = 0;
        public int ActiveTabRows
        {
            get { return activeTabRows; }
            set
            {
                activeTabRows = value;
                RaisePropertyChanged("ActiveTabRows");
            }
        }

        private Tabs activeTab = Tabs.Text;
        public Tabs ActiveTab
        {
            get { return activeTab; }
            set
            {
                activeTab = value;
                OnChangeTabRows();
                RaisePropertyChanged("ActiveTab");
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
            Summary = new ObservableCollection<DAL.Models.Summary>(await Global.Instance.DBContext.GetSummary());
            OnChangeTabRows();
        }

        private void OnChangeTabRows()
        {
            switch (ActiveTab)
            {
                case Tabs.Text:
                    if (ClipDataTexts != null)
                        ActiveTabRows = ClipDataTexts.Count;
                    break;
                case Tabs.Image:
                    if (ClipDataImages != null)
                        ActiveTabRows = ClipDataImages.Count;
                    break;
                case Tabs.File:
                    if (ClipDataFiles != null)
                        ActiveTabRows = ClipDataFiles.Count;
                    break;
                case Tabs.Summary:
                    if (Summary != null)
                        ActiveTabRows = Summary.Count;
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    clipDataFiles = null;
                    clipDataImages = null;
                    clipDataTexts = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
