using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public class Manager : IDisposable
    {
        public event EventHandler<ClipHandler.Entities.ClipDataText> ClipboardTextChanged;
        public event EventHandler<ClipHandler.Entities.ClipDataImage> ClipboardImageChanged;
        public event EventHandler<ClipHandler.Entities.ClipDataFile> ClipboardFileChanged;

        internal static ILogger logger;
        private Configuration configuration;

        public Manager(ILogger _logger)
        {
            logger = _logger;
            Watcher.Instance.ClipboardFileListRecived += Instance_ClipboardFileListRecived;
            Watcher.Instance.ClipboardImageRecived += Instance_ClipboardImageRecived;
            Watcher.Instance.ClipboardTextRecived += Instance_ClipboardTextRecived;
        }

        public void Load(Configuration _configuration)
        {
            configuration = _configuration;
            
            ClipHandler.ClipHandle.Instance.Init(logger, configuration);
            Watcher.Instance.ConfigurationChange(configuration.ConfigurationWatcher);
        }

        public void StartWatcher()
        {
            if (configuration == null)
                throw new Exception("Load a configuration before you start the clipboard watcher");
            Watcher.Instance.StartTimer();
        }

        public void Query()
        {
            if (configuration == null)
                throw new Exception("Load a configuration before you can query for clipboard content");
            Watcher.Instance.QueryClipboard();
        }

        private void Instance_ClipboardTextRecived(object sender, string e)
        {
            if (ClipboardTextChanged != null)
            {
                ClipHandler.Entities.ClipDataText entity = ClipHandler.ClipHandle.Instance.Text(e);
                if (entity != null)
                {
                    ClipboardTextChanged?.Invoke(this, entity);
                }
            }
        }

        private void Instance_ClipboardImageRecived(object sender, System.Drawing.Image e)
        {
            if (ClipboardImageChanged != null)
            {
                ClipHandler.Entities.ClipDataImage entity = ClipHandler.ClipHandle.Instance.Image(e);
                if (entity != null)
                {
                    ClipboardImageChanged?.Invoke(this, entity);
                }
            }
        }

        private void Instance_ClipboardFileListRecived(object sender, System.Collections.Specialized.StringCollection e)
        {
            if (ClipboardFileChanged != null)
            {
                ClipHandler.Entities.ClipDataFile entity = ClipHandler.ClipHandle.Instance.File(e);
                if (entity != null)
                {
                    ClipboardFileChanged?.Invoke(this, entity);
                }
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
                    Watcher.Instance.ClipboardFileListRecived -= Instance_ClipboardFileListRecived;
                    Watcher.Instance.ClipboardImageRecived -= Instance_ClipboardImageRecived;
                    Watcher.Instance.ClipboardTextRecived -= Instance_ClipboardTextRecived;
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
