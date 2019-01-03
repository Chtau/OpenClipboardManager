using OCMApp.Internal;
using OCMApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OCMApp.Favorites
{
    public class FavoriteItemViewModel : BaseViewModel
    {
        public DAL.Models.Favorite Favorite { get; private set; }
        public DAL.Models.FavoriteContentText FavoriteContentText { get; private set; }
        public DAL.Models.FavoriteContentImage FavoriteContentImage { get; private set; }
        public DAL.Models.FavoriteContentFile FavoriteContentFile { get; private set; }
        public HotKeyConfigurationViewModel HotKey { get; private set; }

        private bool showConfig = false;
        public bool ShowConfig
        {
            get { return showConfig; }
            set
            {
                showConfig = value;
                RaisePropertyChanged("ShowConfig");
            }
        }

        public bool IsTextContent
        {
            get
            {
                if (Favorite != null)
                {
                    if (Favorite.Type == DAL.Models.Favorite.ContentType.File || Favorite.Type == DAL.Models.Favorite.ContentType.Text)
                        return true;
                }
                return false;
            }
        }

        public string TextContent
        {
            get
            {
                if (FavoriteContentText != null)
                    return FavoriteContentText.Content;
                else if (FavoriteContentFile != null)
                    return FavoriteContentFile.Content;
                return null;
            }
        }

        public byte[] ImageContent
        {
            get
            {
                if (FavoriteContentImage != null)
                    return FavoriteContentImage.Content;
                return null;
            }
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite)
        {
            HotKey = new HotKeyConfigurationViewModel
            {
                KeyEnumSelected = favorite.Key,
                Modifier1EnumSelected = favorite.Modifier1,
                Modifier2EnumSelected = favorite.Modifier2,
                Title = "Hotkey"
            };
        }

        private ICommand _configCommand;
        public ICommand ConfigCommand
        {
            get
            {
                if (_configCommand == null)
                {
                    _configCommand = new RelayCommand(
                        p => true,
                        p => 
                        {
                            ShowConfig = !ShowConfig;
                        });
                }
                return _configCommand;
            }
        }

        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        p => true,
                        async p =>
                        {
                            await Internal.Global.Instance.DBContext.DeleteFavorite(this);
                        });
                }
                return _deleteCommand;
            }
        }

        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(
                        p => true,
                        p =>
                        {
                            switch (Favorite.Type)
                            {
                                case DAL.Models.Favorite.ContentType.Text:
                                    Internal.Global.Instance.PostAndGet(FavoriteContentText.Content);
                                    break;
                                case DAL.Models.Favorite.ContentType.Image:
                                    Internal.Global.Instance.PostAndGet(FavoriteContentImage.Content);
                                    break;
                                case DAL.Models.Favorite.ContentType.File:
                                    Internal.Global.Instance.PostAndGet(FavoriteContentFile.GetListValue());
                                    break;
                            }
                        });
                }
                return _copyCommand;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        p => true,
                        async p =>
                        {
                            Favorite.Key = HotKey.KeyEnumSelected;
                            Favorite.Modifier1 = HotKey.Modifier1EnumSelected;
                            Favorite.Modifier2 = HotKey.Modifier2EnumSelected;
                            await Internal.Global.Instance.DBContext.UpdateFavorite(this);
                        });
                }
                return _saveCommand;
            }
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentText favoriteContentText) : this(favorite)
        {
            Favorite = favorite;
            FavoriteContentText = favoriteContentText;
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentImage favoriteContentImage) : this(favorite)
        {
            Favorite = favorite;
            FavoriteContentImage = favoriteContentImage;
        }

        public FavoriteItemViewModel(DAL.Models.Favorite favorite, DAL.Models.FavoriteContentFile favoriteContentFile) : this(favorite)
        {
            Favorite = favorite;
            FavoriteContentFile = favoriteContentFile;
        }
    }
}
