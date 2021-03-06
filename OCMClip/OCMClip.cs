﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public class OCMClip : IDisposable
    {
        public event EventHandler<ClipHandler.Entities.ClipDataText> ClipboardTextChanged;
        public event EventHandler<ClipHandler.Entities.ClipDataImage> ClipboardImageChanged;
        public event EventHandler<ClipHandler.Entities.ClipDataFile> ClipboardFileChanged;

        internal static ILogger logger;
        private Configuration configuration;

        public OCMClip(ILogger _logger, bool createSTAThread = true)
        {
            logger = _logger;
            Watcher.Instance.ClipboardFileListRecived += Instance_ClipboardFileListRecived;
            Watcher.Instance.ClipboardImageRecived += Instance_ClipboardImageRecived;
            Watcher.Instance.ClipboardTextRecived += Instance_ClipboardTextRecived;
            Watcher.Instance.UseOwnThread = createSTAThread;
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

        public void StopWatcher()
        {
            Watcher.Instance.StopTimer();
        }

        public void Get()
        {
            if (configuration == null)
                throw new Exception("Load a configuration before you can query for clipboard content");
            Watcher.Instance.GetClipboard();
        }

        public void Post(string value, ClipHandler.Entities.Enums.TextDataFormat format)
        {
            if (value != null)
                Watcher.Instance.PostClipboard(value, format);
        }

        public void Post(Image value)
        {
            if (value != null)
                Watcher.Instance.PostClipboard(value);
        }

        public void Post(List<string> value)
        {
            if (value != null)
            {
                StringCollection col = new StringCollection();
                col.AddRange(value.ToArray());
                Watcher.Instance.PostClipboard(col);
            }
        }

        private void Instance_ClipboardTextRecived(object sender, string e)
        {
            using (var entity = ClipHandler.ClipHandle.Instance.Text(e))
            {
                if (entity != null)
                    ClipboardTextChanged?.Invoke(this, entity);
            }
        }

        private void Instance_ClipboardImageRecived(object sender, System.Drawing.Image e)
        {
            using (var entity = ClipHandler.ClipHandle.Instance.Image(e))
            {
                if (entity != null)
                    ClipboardImageChanged?.Invoke(this, entity);
            }
        }

        private void Instance_ClipboardFileListRecived(object sender, System.Collections.Specialized.StringCollection e)
        {
            using (var entity = ClipHandler.ClipHandle.Instance.File(e))
            {
                if (entity != null)
                    ClipboardFileChanged?.Invoke(this, entity);
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
                    Watcher.Instance.StopTimer();
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
