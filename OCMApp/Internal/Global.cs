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
        const string Log_File = "log.txt";
        const string DB_File = "ocm.db";
        const string Clipboard_Get_Send_Key = "^c";
        const string Clipboard_Post_Send_Key = "^v";

        public OCMClip.OCMClip Clip { get; private set; }
        public OCMHotKey.OCMHotKey HotKey { get; private set; }
        public Settings.Settings Settings { get; private set; } = new Settings.Settings();
        public Localize Localize { get; private set; }
        public DAL.DBContext DBContext { get; private set; }
        public DAL.Models.LastClip LastClip { get; private set; }
        public bool InitError { get; private set; } = false;
        
        private OCMHotKey.HotKey clipboardHotKeyGet;
        private OCMHotKey.HotKey clipboardHotKeyPost;
        private bool isInit = false;

        public string LogFile
        {
            get
            {
                return System.IO.Path.Combine(Helper.Folder.GetUserFolder(), Log_File);
            }
        }

        public void Init()
        {
            if (!isInit)
            {
                try
                {
                    isInit = true;

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
                    DBContext = new DAL.DBContext(System.IO.Path.Combine(Helper.Folder.GetUserFolder(), DB_File));

                    OnSettingsChange();
                } catch (Exception ex)
                {
                    Log.Error(ex, "Critical Application Error (restart needed)");
                    InitError = true;
                }
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
                Localize.SetLanguage();

                OnSetupClipGet();
                OnSetupClipPost();
                OnSetupAutostart();
            } catch (Exception ex)
            {
                Log.Error(ex, "Change Application Settings");
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
        #endregion

        public void PostAndGet(DAL.Models.ClipText textEntity)
        {
            try
            {
                Clip.Post(textEntity.Value, OCMClip.ClipHandler.Entities.Enums.TextDataFormat.Text);
                Clip.Get();
            } catch (Exception ex)
            {
                Log.Error(ex, "Post and Get Text Clipboard");
            }
        }

        public void PostAndGet(DAL.Models.ClipImage imageEntity)
        {
            try
            {
                Clip.Post(OCMClip.ClipHandler.ConvertImage.ByteArrayToImage(imageEntity.Value));
                Clip.Get();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post and Get Image Clipboard");
            }
        }

        public void PostAndGet(DAL.Models.ClipFile fileEntity)
        {
            try
            {
                Clip.Post(fileEntity.GetListValue());
                Clip.Get();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Post and Get File Clipboard");
            }
        }

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

        private void HotKey_HotKeyPressed(object sender, OCMHotKey.HotKey e)
        {
            
        }

        private void Clip_ClipboardTextChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataText e)
        {
            try
            {
                var entity = new DAL.Models.ClipText(e);
                LastClip.ClipText = entity;
                DBContext.InsertClipText(entity);
            } catch (Exception ex)
            {
                Log.Error(ex, "Clipboard text value save to DB");
            }
        }

        private void Clip_ClipboardImageChanged(object sender, OCMClip.ClipHandler.Entities.ClipDataImage e)
        {
            try
            {
                var entity = new DAL.Models.ClipImage(e);
                LastClip.ClipImage = entity;
                DBContext.InsertClipImage(new DAL.Models.ClipImage(e));
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
                var entity = new DAL.Models.ClipFile(e);
                LastClip.ClipFile = entity;
                DBContext.InsertClipFile(new DAL.Models.ClipFile(e));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Clipboard files value save to DB");
            }
        }
    }
}
