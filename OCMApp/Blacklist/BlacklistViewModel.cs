using OCMApp.Internal;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OCMApp.Blacklist
{
    public class BlacklistViewModel : BaseViewModel
    {
        private ObservableCollection<DAL.Models.Blacklist> blacklistItems;
        public ObservableCollection<DAL.Models.Blacklist> BlacklistItems
        {
            get { return blacklistItems; }
            set
            {
                blacklistItems = value;
                RaisePropertyChanged("BlacklistItems");
            }
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
            try
            {
                BlacklistItems = new ObservableCollection<DAL.Models.Blacklist>(await Global.Instance.DBContext.Blacklist());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "OnRefresh failed to load data");
            }
        }
    }
}
