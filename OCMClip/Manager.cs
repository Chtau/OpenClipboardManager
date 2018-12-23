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

        internal static ILogger logger;
        private Configuration configuration;
        private ClipHandler.ClipText clipText;

        public Manager(ILogger _logger)
        {
            logger = _logger;
            Watcher.Instance.ClipboardAudioRecived += Instance_ClipboardAudioRecived;
            Watcher.Instance.ClipboardFileListRecived += Instance_ClipboardFileListRecived;
            Watcher.Instance.ClipboardImageRecived += Instance_ClipboardImageRecived;
            Watcher.Instance.ClipboardTextRecived += Instance_ClipboardTextRecived;
        }

        public void Load(Configuration _configuration)
        {
            configuration = _configuration;
            clipText = new ClipHandler.ClipText(logger, null);

            Watcher.Instance.ConfigurationChange(configuration.ConfigurationWatcher);
        }

        public void StartWatcher()
        {
            if (configuration == null)
                throw new Exception("Load a configuration before you start the clipboard watcher");
            Watcher.Instance.StartTimer();
        }


        private void Instance_ClipboardTextRecived(object sender, string e)
        {
            ClipHandler.Entities.ClipDataText entity = clipText.Handle(e);
            if (entity != null)
            {
                ClipboardTextChanged?.Invoke(this, entity);
            }
        }

        private void Instance_ClipboardImageRecived(object sender, System.Drawing.Image e)
        {
            throw new NotImplementedException();
        }

        private void Instance_ClipboardFileListRecived(object sender, System.Collections.Specialized.StringCollection e)
        {
            throw new NotImplementedException();
        }

        private void Instance_ClipboardAudioRecived(object sender, System.IO.Stream e)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Watcher.Instance.ClipboardAudioRecived -= Instance_ClipboardAudioRecived;
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
