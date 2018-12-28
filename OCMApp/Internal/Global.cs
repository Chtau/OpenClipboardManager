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

        public OCMClip.OCMClip Clip { get; private set; }
        public OCMHotKey.OCMHotKey HotKey { get; private set; }
        public Settings.Settings Settings { get; private set; } = new Settings.Settings();
        public Localize Localize { get; private set; }

        private OCMHotKey.HotKey clipboardHotKey;
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
            SettingsChange();
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
                        SettingsChange();
                        return true;
                    }
                } else
                {
                    if (Settings == null)
                        Settings = new Settings.Settings();
                    SaveSettings(Settings);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read Settings file");
            }
            return false;
        }

        private void SettingsChange()
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

            OnSetupClip();
        }

        private void OnSetupClip()
        {
            if (Settings.UseWatcher)
            {
                if (clipboardHotKey != null)
                    HotKey.Remove(clipboardHotKey.Id);
                // use the Watcher to intercept the default Clipboard
                Clip.StartWatcher();
            } else
            {
                // use a defined Keyboard Shortcut only to retrive the Clipboard
                Clip.StopWatcher();
                if (clipboardHotKey != null)
                    HotKey.Remove(clipboardHotKey.Id);
                clipboardHotKey = new OCMHotKey.HotKey(Settings.ClipKey, Settings.ClipKeyModifier, HotKeyGetClipboardPressed, "getclipboard");
                HotKey.Add(clipboardHotKey);
            }
        }

        private void HotKeyGetClipboardPressed(OCMHotKey.HotKey e)
        {
            HotKey.SendKeys("^C");
            Clip.Get();
        }

        private void HotKey_HotKeyPressed(object sender, OCMHotKey.HotKey e)
        {
            
        }

        private void Clip_ClipboardTextChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataText e)
        {
            System.Diagnostics.Debug.Print(e.Value);
        }

        private void Clip_ClipboardImageChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataImage e)
        {
            
        }

        private void Clip_ClipboardFileChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataFile e)
        {
            
        }
    }
}
