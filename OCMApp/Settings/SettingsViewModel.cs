﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public Settings Settings { get; set; }
        public List<KeyValuePair<int, string>> ClipWatcherImageFormatTypeEnum { get; set; }
        public int ClipWatcherImageFormatTypeEnumSelected { get; set; }

        public List<KeyValuePair<int, string>> CultureEnum { get; set; }
        public int CultureEnumSelected { get; set; }

        public List<KeyValuePair<int, string>> AccentEnum { get; set; }
        public int AccentEnumSelected { get; set; }

        public List<KeyValuePair<int, string>> ThemeColorEnum { get; set; }
        public int ThemeColorEnumSelected { get; set; }

        public HotKeyConfigurationViewModel ClipHotKey { get; set; }
        public HotKeyConfigurationViewModel ClipPasteHotKey { get; set; }
        public HotKeyConfigurationViewModel FavoritesWindowHotKey { get; set; }

        public SettingsViewModel()
        {
            Settings = Internal.Global.Instance.Settings;
            ClipWatcherImageFormatTypeEnumSelected = (int)Settings.ClipWatcherDefaultImageFormat;
            ClipWatcherImageFormatTypeEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(OCMClip.ClipHandler.Entities.Enums.ImageFormatType));

            CultureEnumSelected = (int)Settings.Culture;
            CultureEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(Internal.Localize.Language), false);
            ClipHotKey = new HotKeyConfigurationViewModel();
            ClipHotKey.SetKey(Settings.ClipKey);
            ClipHotKey.SetModifier(Settings.ClipKeyModifier);
            ClipHotKey.Title = "Hotkey_copy_value";

            ClipPasteHotKey = new HotKeyConfigurationViewModel();
            ClipPasteHotKey.SetKey(Settings.ClipPostKey);
            ClipPasteHotKey.SetModifier(Settings.ClipPostKeyModifier);
            ClipPasteHotKey.Title = "Hotkey_paste_value";

            FavoritesWindowHotKey = new HotKeyConfigurationViewModel();
            FavoritesWindowHotKey.SetKey(Settings.FavoritesWindowKey);
            FavoritesWindowHotKey.SetModifier(Settings.FavoritesWindowModifier);
            FavoritesWindowHotKey.Title = "Hotkey_Favorites_Window";

            AccentEnumSelected = (int)Settings.Accent;
            AccentEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(Internal.Theme.Accent));

            ThemeColorEnumSelected = (int)Settings.ThemeColor;
            ThemeColorEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(Internal.Theme.ThemeColor));
        }
    }
}
