using OCMClip;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public OCMClip.OCMClip Clip { get; private set; }
        public OCMHotKey.OCMHotKey HotKey { get; private set; }
        public Settings.Settings Settings { get; private set; } = new Settings.Settings();
        public Localize Localize { get; private set; }
        public DAL.DBContext DBContext { get; private set; }

        private OCMHotKey.HotKey clipboardHotKeyGet;
        private OCMHotKey.HotKey clipboardHotKeyPost;
        private bool isInit = false;
        public void Init()
        {
            if (!isInit)
            {
                isInit = true;
                
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(System.IO.Path.Combine(Helper.Folder.GetUserFolder(), "log.txt"),
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
                DBContext = new DAL.DBContext(System.IO.Path.Combine(Helper.Folder.GetUserFolder(), "ocm.db"));
                
                OnSettingsChange();
            }
        }

        public void Close()
        {
            Clip.Dispose();
            Log.CloseAndFlush();
        }

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
                string file = System.IO.Path.Combine(Helper.Folder.GetUserFolder(), GlobalValues.SettingsFile);
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
                string file = System.IO.Path.Combine(Helper.Folder.GetUserFolder(), GlobalValues.SettingsFile);
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
                    OnSaveSettings(Settings);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read Settings file");
            }
            return false;
        }

        private void OnSettingsChange()
        {
            Clip.Load(new Configuration(
                    new ConfigurationWatcher(Settings.ClipWatcherRefreshRateMilliseconds,
                        Settings.ClipWatcherRefreshRateSeconds,
                        Settings.ClipWatcherActiveText,
                        Settings.ClipWatcherActiveImage,
                        Settings.ClipWatcherActiveFile),
                    null, null, null,
                    Settings.ClipWatcherDefaultImageFormat
                    ));
            Localize.SetLanguage();

            OnSetupClipGet();
            OnSetupClipPost();
        }

        private void OnSetupClipGet()
        {
            if (Settings.UseWatcher)
            {
                if (clipboardHotKeyGet != null)
                    HotKey.Remove(clipboardHotKeyGet.Id);
                // use the Watcher to intercept the default Clipboard
                Clip.StartWatcher();
            } else
            {
                // use a defined Keyboard Shortcut only to retrive the Clipboard
                Clip.StopWatcher();
                if (clipboardHotKeyGet != null)
                    HotKey.Remove(clipboardHotKeyGet.Id);
                clipboardHotKeyGet = new OCMHotKey.HotKey(Settings.ClipKey, Settings.ClipKeyModifier, HotKeyGetClipboardPressed, HotKey_Event_ClipboardCopy);
                HotKey.Add(clipboardHotKeyGet);
            }
        }

        private void OnSetupClipPost()
        {
            if (clipboardHotKeyPost != null)
                HotKey.Remove(clipboardHotKeyPost.Id);
            clipboardHotKeyPost = new OCMHotKey.HotKey(Settings.ClipPostKey, Settings.ClipPostKeyModifier, HotKeyPostClipboardPressed, HotKey_Event_ClipboardPaste);
            HotKey.Add(clipboardHotKeyPost);
        }

        private void HotKeyGetClipboardPressed(OCMHotKey.HotKey e)
        {
            HotKey.SendKeys("^C");
            Clip.Get();
        }

        private void HotKeyPostClipboardPressed(OCMHotKey.HotKey e)
        {
            Clip.Post("TestPost", OCMClip.ClipHandler.Entities.Enums.TextDataFormat.Text);
            if (!Settings.OnlySetClipboardOnPaste)
                HotKey.SendKeys("^v");
        }

        private void HotKey_HotKeyPressed(object sender, OCMHotKey.HotKey e)
        {
            
        }

        private void Clip_ClipboardTextChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataText e)
        {
            System.Diagnostics.Debug.Print(e.Value);
            DBContext.InsertClipText(new DAL.Models.ClipText(e));
        }

        private void Clip_ClipboardImageChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataImage e)
        {
            
        }

        private void Clip_ClipboardFileChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataFile e)
        {
            
        }
    }
}
