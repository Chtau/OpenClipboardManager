using MahApps.Metro;
using Microsoft.Win32;
using OCMClip;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OCMApp.Internal
{
    public class Global
    {
        #region Singleton
        private static volatile Global instance;
        private static readonly object syncRoot = new Object();

        private Global()
        {
        }

        public static Global Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Global();
                    }
                }

                return instance;
            }
        }
        #endregion

        const string HotKey_Event_ClipboardCopy = "getclipboard";
        const string HotKey_Event_ClipboardPaste = "postclipboard";
        const string HotKey_Event_FavoritesWindow = "favoriteswindow";
        const string HotKey_Event_FavoritesItem_Prefix = "favorite_";
        const string Log_File = "log.txt";
        const string DB_File = "ocm.db";
        const string Clipboard_Get_Send_Key = "^c";
        const string Clipboard_Post_Send_Key = "^v";
        const int Init_Start_Clip_Watcher_Seconds_Delay = 10;

        public OCMClip.OCMClip Clip { get; private set; }
        public OCMHotKey.OCMHotKey HotKey { get; private set; }
        public Settings.Settings Settings { get; private set; } = new Settings.Settings();
        public Localize Localize { get; private set; }
        public DAL.DBContext DBContext { get; private set; }
        public DAL.Models.LastClip LastClip { get; private set; }
        public bool InitError { get; private set; } = false;
        public bool FirstStart { get; private set; } = false;
        public Settings.WindowState FavoriteWindowState { get; private set; } = new Settings.WindowState();

        private OCMHotKey.HotKey clipboardHotKeyGet;
        private OCMHotKey.HotKey clipboardHotKeyPost;
        private OCMHotKey.HotKey favoritesWindowHotKey;
        private bool isInit = false;

        private string appUserFolder = null;
        public string AppUserFolder
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(appUserFolder))
                    return appUserFolder;
                else
                    return Helper.Folder.GetUserFolder();
            }
        }

        public string LogFile
        {
            get
            {
                return System.IO.Path.Combine(AppUserFolder, Log_File);
            }
        }

        public void Init(string appUserFolder = null)
        {
            if (!isInit)
            {
                try
                {
                    isInit = true;
                    if (!string.IsNullOrWhiteSpace(appUserFolder))
                    {
                        if (Helper.Folder.IsValidFolder(appUserFolder))
                            this.appUserFolder = appUserFolder;
                        else
                            this.appUserFolder = null;
                    }

                    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.File(LogFile,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();

                    Localize = new Localize();
                    Clip = new OCMClip.OCMClip(new OCMClipLogger());
                    Clip.ClipboardFileChanged += Clip_ClipboardFileChanged;
                    Clip.ClipboardImageChanged += Clip_ClipboardImageChanged;
                    Clip.ClipboardTextChanged += Clip_ClipboardTextChanged;
                    HotKey = new OCMHotKey.OCMHotKey();
                    HotKey.HotKeyPressed += HotKey_HotKeyPressed;

                    OnLoadSettings();
                    DBContext = new DAL.DBContext(System.IO.Path.Combine(AppUserFolder, DB_File));

                    OnSettingsChange(true);
                    RefreshFavorites();
                    LoadFavoriteWindowState();

                    SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
                } catch (Exception ex)
                {
                    Log.Error(ex, "Critical Application Error (restart needed)");
                    InitError = true;
                }
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            try
            {
                switch (e.Mode)
                {
                    case PowerModes.Resume:
                        OnSetupClipGet();
                        break;
                    case PowerModes.StatusChange:
                        break;
                    case PowerModes.Suspend:
                        Clip.StopWatcher();
                        break;
                }
            } catch (Exception ex)
            {
                Log.Error(ex, $"System Powermode changed => New State: {Enum.GetName(typeof(PowerModes), e.Mode)}");
            }
        }

        public void Close()
        {
            Clip.Dispose();
            Log.CloseAndFlush();
        }

        #region Settings
        public bool SaveSettings(Settings.Settings settings)
        {
            Settings = settings;
            OnSettingsChange();
            return OnSaveSettings(settings);
        }

        private bool OnSaveSettings(Settings.Settings settings)
        {
            try
            {
                string file = System.IO.Path.Combine(Internal.Global.Instance.AppUserFolder, GlobalValues.SettingsFile);
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(Settings);
                System.IO.File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to write Settings file");
                return false;
            }
            return true;
        }

        private bool OnLoadSettings()
        {
            try
            {
                string file = System.IO.Path.Combine(Internal.Global.Instance.AppUserFolder, GlobalValues.SettingsFile);
                if (System.IO.File.Exists(file))
                {
                    string content = System.IO.File.ReadAllText(file);
                    var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings.Settings>(content);
                    if (settings != null)
                    {
                        Settings = settings;
                        return true;
                    }
                } else
                {
                    if (Settings == null)
                        Settings = new Settings.Settings();
                    FirstStart = true;
                    OnSaveSettings(Settings);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read Settings file");
            }
            return false;
        }

        private void OnSettingsChange(bool delayClipLoad = false)
        {
            try
            {
                LastClip = Task.Run(DBContext.GetLastValues).GetAwaiter().GetResult();
                string defaultText = LastClip.ClipText?.Value;
                System.Drawing.Image defaultImage = null;
                if (LastClip.ClipImage?.Value != null)
                    defaultImage = OCMClip.ClipHandler.ConvertImage.ByteArrayToImage(LastClip.ClipImage.Value);
                List<string> defaultFile = LastClip.ClipFile?.GetListValue();

                Clip.Load(new Configuration(
                        new ConfigurationWatcher(Settings.ClipWatcherRefreshRateMilliseconds,
                            Settings.ClipWatcherRefreshRateSeconds,
                            Settings.ClipWatcherActiveText,
                            Settings.ClipWatcherActiveImage,
                            Settings.ClipWatcherActiveFile),
                        defaultText,
                        defaultImage,
                        defaultFile,
                        Settings.ClipWatcherDefaultImageFormat
                        ));
                if (delayClipLoad)
                {
                    // if we don't wait on the startup with starting the watcher 
                    // there could be a confilict with the STA thread of the watcher and the WPF STA thread
                    Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep((int)TimeSpan.FromSeconds(Init_Start_Clip_Watcher_Seconds_Delay).TotalMilliseconds);
                        OnSetupClipGet();
                    });
                } else
                {
                    OnSetupClipGet();
                }
                
                Localize.SetLanguage();

                OnSetupClipPost();
                OnSetupAutostart();
                OnSetupFavorites();
                OnSetupTheme();
                if (FavoriteWindowState == null)
                    FavoriteWindowState = new Settings.WindowState();
                if (Settings.FavoriteWindowStateRemember)
                    FavoriteWindowState.CurrentState = OCMApp.Settings.WindowState.State.Remember;
                else
                    FavoriteWindowState.CurrentState = OCMApp.Settings.WindowState.State.WindowsDefault;
            } catch (Exception ex)
            {
                Log.Error(ex, "Change Application Settings");
            }
        }

        private void OnSetupTheme()
        {
            try
            {
                ThemeManager.ChangeAppStyle(Application.Current,
                                    ThemeManager.GetAccent(Enum.GetName(typeof(Internal.Theme.Accent), Settings.Accent)),
                                    ThemeManager.GetAppTheme(Enum.GetName(typeof(Internal.Theme.ThemeColor), Settings.ThemeColor)));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Setup Application Theme");
            }
        }

        private void OnSetupClipGet()
        {
            try
            {
                if (Settings.UseWatcher)
                {
                    if (clipboardHotKeyGet != null)
                        HotKey.Remove(clipboardHotKeyGet.Id);
                    // use the Watcher to intercept the default Clipboard
                    Clip.StartWatcher();
                }
                else
                {
                    // use a defined Keyboard Shortcut only to retrive the Clipboard
                    Clip.StopWatcher();
                    if (clipboardHotKeyGet != null)
                        HotKey.Remove(clipboardHotKeyGet.Id);
                    clipboardHotKeyGet = new OCMHotKey.HotKey(Settings.ClipKey, Settings.ClipKeyModifier, HotKeyGetClipboardPressed, HotKey_Event_ClipboardCopy);
                    HotKey.Add(clipboardHotKeyGet);
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Setup Clip Hotkey/Watcher");
            }
        }

        private void OnSetupClipPost()
        {
            try
            {
                if (clipboardHotKeyPost != null)
                    HotKey.Remove(clipboardHotKeyPost.Id);
                clipboardHotKeyPost = new OCMHotKey.HotKey(Settings.ClipPostKey, Settings.ClipPostKeyModifier, HotKeyPostClipboardPressed, HotKey_Event_ClipboardPaste);
                HotKey.Add(clipboardHotKeyPost);
            } catch (Exception ex)
            {
                Log.Error(ex, "Setup Hotkey Post last Clipboard");
            }
        }

        private void OnSetupAutostart()
        {
            if (Settings.AutoStart)
                Autostart.InstallMeOnStartUp();
            else
                Autostart.RemoveMeOnStartUp();
        }

        private void OnSetupFavorites()
        {
            try
            {
                if (favoritesWindowHotKey != null)
                    HotKey.Remove(favoritesWindowHotKey.Id);
                favoritesWindowHotKey = new OCMHotKey.HotKey(Settings.FavoritesWindowKey, Settings.FavoritesWindowModifier, HotKeyFavoritesWindowPressed, HotKey_Event_FavoritesWindow);
                HotKey.Add(favoritesWindowHotKey);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Setup Hotkey Favorites Window");
            }
        }

        private void OnSetupHotKeysFavoritesItem(List<Favorites.FavoriteItemViewModel> favoriteItems)
        {
            try
            {
                if (favoriteItems != null)
                {
                    foreach (Favorites.FavoriteItemViewModel item in favoriteItems)
                    {
                        HotKey.Add(new OCMHotKey.HotKey(item.Favorite.GetKey(), item.Favorite.GetModifier(), null, HotKey_Event_FavoritesItem_Prefix + item.Favorite.Id));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Setup Hotkeys for Favorites Items");
            }
        }

        private void OnRemoveHotKeysFavoritesItem(List<Favorites.FavoriteItemViewModel> favoriteItems)
        {
            try
            {
                if (favoriteItems != null)
                {
                    foreach (Favorites.FavoriteItemViewModel item in favoriteItems)
                    {
                        HotKey.Remove(HotKey_Event_FavoritesItem_Prefix + item.Favorite.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Remove Hotkeys for Favorites Items");
            }
        }
        #endregion

        #region Favorites
        public void SaveFavoriteWindowState()
        {
            try
            {
                string file = System.IO.Path.Combine(AppUserFolder, GlobalValues.WindowStateFavoriteFile);
                var content = Newtonsoft.Json.JsonConvert.SerializeObject(FavoriteWindowState);
                System.IO.File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save Favorite Window state to file");
            }
        }

        public bool LoadFavoriteWindowState()
        {
            try
            {
                string file = System.IO.Path.Combine(AppUserFolder, GlobalValues.WindowStateFavoriteFile);
                if (System.IO.File.Exists(file))
                {
                    string content = System.IO.File.ReadAllText(file);
                    var state = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings.WindowState>(content);
                    if (state != null)
                    {
                        FavoriteWindowState = state;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load Favorite Window state to file");
            }
            return false;
        }
        #endregion

        #region Clipboard
        public void PostAndGet(string value)
        {
            try
            {
                Clip.Post(value, OCMClip.ClipHandler.Entities.Enums.TextDataFormat.Text);
                Clip.Get();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post and Get Text Clipboard");
            }
        }

        public void PostAndGet(byte[] value)
        {
            try
            {
                Clip.Post(OCMClip.ClipHandler.ConvertImage.ByteArrayToImage(value));
                Clip.Get();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post and Get Byte Array Clipboard");
            }
        }

        public void PostAndGet(List<string> value)
        {
            try
            {
                Clip.Post(value);
                Clip.Get();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post and Get File list Clipboard");
            }
        }

        public void PostAndGet(DAL.Models.ClipText textEntity)
        {
            PostAndGet(textEntity.Value);
        }

        public void PostAndGet(DAL.Models.ClipImage imageEntity)
        {
            PostAndGet(imageEntity.Value);
        }

        public void PostAndGet(DAL.Models.ClipFile fileEntity)
        {
            PostAndGet(fileEntity.GetListValue());
        }

        private void Clip_ClipboardTextChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataText e)
        {
            try
            {
                if (AllowClip(e))
                {
                    LastClip.ClipText = new DAL.Models.ClipText(e);
                    DBContext.InsertClipText(LastClip.ClipText);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Clipboard text value save to DB");
            }
        }

        private void Clip_ClipboardImageChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataImage e)
        {
            try
            {
                if (AllowClip(e))
                {
                    LastClip.ClipImage = new DAL.Models.ClipImage(e);
                    DBContext.InsertClipImage(LastClip.ClipImage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Clipboard image value save to DB");
            }
        }

        private void Clip_ClipboardFileChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataFile e)
        {
            try
            {
                if (AllowClip(e))
                {
                    LastClip.ClipFile = new DAL.Models.ClipFile(e);
                    DBContext.InsertClipFile(LastClip.ClipFile);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Clipboard files value save to DB");
            }
        }
        #endregion

        #region HotKey
        private void HotKeyGetClipboardPressed(OCMHotKey.HotKey e)
        {
            try
            {
                HotKey.SendKeys(Clipboard_Get_Send_Key);
                Clip.Get();
            } catch (Exception ex)
            {
                Log.Error(ex, "Get current Clipboard data");
            }
        }

        private void HotKeyPostClipboardPressed(OCMHotKey.HotKey e)
        {
            try
            {
                if (LastClip != null)
                {
                    bool hasPostValue = false;
                    if (LastClip.ClipFile != null && LastClip.ClipFile.DateCreated > LastClip.ClipImage?.DateCreated && LastClip.ClipFile.DateCreated > LastClip.ClipText?.DateCreated)
                    {
                        Clip.Post(LastClip.ClipFile.GetListValue());
                        hasPostValue = true;
                    }
                    else if (LastClip.ClipImage != null && LastClip.ClipImage.DateCreated > LastClip.ClipText?.DateCreated && LastClip.ClipImage.DateCreated > LastClip.ClipFile?.DateCreated)
                    {
                        Clip.Post(OCMClip.ClipHandler.ConvertImage.ByteArrayToImage(LastClip.ClipImage.Value));
                        hasPostValue = true;
                    }
                    else if (LastClip.ClipText != null)
                    {
                        Clip.Post(LastClip.ClipText.Value, OCMClip.ClipHandler.Entities.Enums.TextDataFormat.Text);
                        hasPostValue = true;
                    }
                    if (hasPostValue)
                    {
                        if (!Settings.OnlySetClipboardOnPaste)
                            HotKey.SendKeys(Clipboard_Post_Send_Key);
                    }
                }
            } catch (Exception ex)
            {
                Log.Error(ex, "Post last Clipboard content");
            }
        }

        private void HotKeyFavoritesWindowPressed(OCMHotKey.HotKey e)
        {
            try
            {
                ShowFavoritesWindow();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "show/Hide Favorites Window");
            }
        }

        private void HotKey_HotKeyPressed(object sender, OCMHotKey.HotKey e)
        {
            try
            {
                if (e.UniqueName.ToLower().StartsWith(HotKey_Event_FavoritesItem_Prefix.ToLower()))
                {
                    Guid favId = new Guid(e.UniqueName.ToLower().Replace(HotKey_Event_FavoritesItem_Prefix.ToLower(), ""));
                    if (FavoriteItems.Any(x => x.Favorite.Id == favId))
                    {
                        var item = FavoriteItems.First(x => x.Favorite.Id == favId);
                        switch (item.Favorite.Type)
                        {
                            case DAL.Models.Favorite.ContentType.Text:
                                PostAndGet(item.FavoriteContentText.Content);
                                break;
                            case DAL.Models.Favorite.ContentType.Image:
                                PostAndGet(item.FavoriteContentImage.Content);
                                break;
                            case DAL.Models.Favorite.ContentType.File:
                                PostAndGet(item.FavoriteContentFile.GetListValue());
                                break;
                        }
                        if (!Settings.OnlySetClipboardOnPaste)
                            HotKey.SendKeys(Clipboard_Post_Send_Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Hotkey pressed event");
            }
        }
        #endregion

        #region Favorites Window
        private WeakReference<Favorites.FavoritesWindow> weakFavorites;

        public void ShowFavoritesWindow()
        {
            if (Helper.WindowCheck.IsWindowOpen<Favorites.FavoritesWindow>())
            {
                if (weakFavorites != null && weakFavorites.TryGetTarget(out Favorites.FavoritesWindow favorites))
                    favorites.Close();
            }
            else
            {
                var favoritesWindow = new Favorites.FavoritesWindow();
                weakFavorites = new WeakReference<Favorites.FavoritesWindow>(favoritesWindow);
                favoritesWindow.Show();
                favoritesWindow.Activate();
            }
        }

        private List<Favorites.FavoriteItemViewModel> _favoriteItems;
        public List<Favorites.FavoriteItemViewModel> FavoriteItems
        {
            get
            {
                if (_favoriteItems == null)
                    _favoriteItems = OnFavoritesRefresh();
                return _favoriteItems;
            }
        }

        private List<Favorites.FavoriteItemViewModel> OnFavoritesRefresh()
        {
            OnRemoveHotKeysFavoritesItem(_favoriteItems);
            var newFav = Task.Run(DBContext.Favorites).GetAwaiter().GetResult();
            OnSetupHotKeysFavoritesItem(newFav);
            return newFav;
        }

        public void RefreshFavorites()
        {
            _favoriteItems = OnFavoritesRefresh();
            if (weakFavorites != null && weakFavorites.TryGetTarget(out Favorites.FavoritesWindow favorites))
                favorites.Refresh();
        }
        #endregion

        #region Blacklist

        private bool AllowClip(OCMClip.ClipHandler.Entities.ClipData clipData)
        {
            if (BlacklistItems.Any(x => x.NormalizedValue == clipData.ApplicationName?.ToUpper()
            || x.NormalizedValue == clipData.ApplicationWindowTitle?.ToUpper()
            || x.NormalizedValue == clipData.ProcessName?.ToUpper()
            || x.NormalizedValue == clipData.ProcessId.ToString()))
                return false;
            return true;
        }

        private List<DAL.Models.Blacklist> _blacklistItems;
        public List<DAL.Models.Blacklist> BlacklistItems
        {
            get
            {
                if (_blacklistItems == null)
                    _blacklistItems = OnBlacklistRefresh();
                return _blacklistItems;
            }
        }

        public void RefreshBlacklist()
        {
            _blacklistItems = OnBlacklistRefresh();
        }

        private List<DAL.Models.Blacklist> OnBlacklistRefresh()
        {
            var newFav = Task.Run(DBContext.Blacklist).GetAwaiter().GetResult();
            return newFav;
        }
        #endregion
    }
}
