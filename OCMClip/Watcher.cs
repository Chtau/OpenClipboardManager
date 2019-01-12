using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCMClip
{
    internal sealed class Watcher
    {
        #region Singleton
        private static volatile Watcher instance;
        private static readonly object syncRoot = new Object();

        private Watcher()
        {

        }

        public static Watcher Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Watcher();
                    }
                }

                return instance;
            }
        }
        #endregion

        public event EventHandler<string> ClipboardTextRecived;
        public event EventHandler<System.Drawing.Image> ClipboardImageRecived;
        public event EventHandler<System.Collections.Specialized.StringCollection> ClipboardFileListRecived;
        public bool UseOwnThread { get; set; } = true;

        private System.Timers.Timer dispatcherTimer;
        private bool isRestarting = false;
        private ConfigurationWatcher configuration;
        private STATaskScheduler sta = new STATaskScheduler(1);

        private void OnStartTimer(int refreshRateMilliseconds, int refreshRateSeconds)
        {
            try
            {
                dispatcherTimer = new System.Timers.Timer();
                dispatcherTimer.Elapsed += DispatcherTimer_Elapsed;
                dispatcherTimer.Interval = (int)(new TimeSpan(0, 0, 0, refreshRateSeconds, refreshRateMilliseconds).TotalMilliseconds);
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        private void OnStopTimer()
        {
            try
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Elapsed -= DispatcherTimer_Elapsed;
                dispatcherTimer = null;
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }
        
        private void DispatcherTimer_Elapsed(object sender, EventArgs e)
        {
            isRestarting = true;
            if (dispatcherTimer != null)
                dispatcherTimer.Enabled = false;
            try
            {
                OnGetClipboardContent();
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
            finally
            {
                if (dispatcherTimer != null)
                {
                    dispatcherTimer.Enabled = true;
                    isRestarting = false;
                }
            }
        }

        private void OnGetClipboardContent()
        {
            InvokeSTATThread(() =>
            {
                try
                {
                    if (configuration.ActiveText && System.Windows.Forms.Clipboard.ContainsText())
                    {
                        ClipboardTextRecived?.Invoke(this, System.Windows.Forms.Clipboard.GetText());
                    }
                    else if (configuration.ActiveImage && System.Windows.Forms.Clipboard.ContainsImage())
                    {
                        ClipboardImageRecived?.Invoke(this, System.Windows.Forms.Clipboard.GetImage());
                    }
                    else if (configuration.ActiveFileDropList && System.Windows.Forms.Clipboard.ContainsFileDropList())
                    {
                        ClipboardFileListRecived?.Invoke(this, System.Windows.Forms.Clipboard.GetFileDropList());
                    }
                } catch (System.Runtime.InteropServices.ExternalException eex)
                {
                    OCMClip.logger.Diagnostic(eex.ToString());
                } catch (Exception ex)
                {
                    OCMClip.logger.Error(ex);
                }
            });
        }

        private void OnPostClipboardContentString(string value, ClipHandler.Entities.Enums.TextDataFormat format)
        {
            if (configuration.ActiveText)
            {
                InvokeSTATThread(() =>
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetText(value, (System.Windows.Forms.TextDataFormat)(int)format);
                    }
                    catch (System.Runtime.InteropServices.ExternalException eex)
                    {
                        OCMClip.logger.Diagnostic(eex.ToString());
                    }
                    catch (Exception ex)
                    {
                        OCMClip.logger.Error(ex);
                    }
                });
            }
        }

        private void OnPostClipboardContentImage(Image value)
        {
            if (configuration.ActiveImage)
            {
                InvokeSTATThread(() =>
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetImage(value);
                    }
                    catch (System.Runtime.InteropServices.ExternalException eex)
                    {
                        OCMClip.logger.Diagnostic(eex.ToString());
                    }
                    catch (Exception ex)
                    {
                        OCMClip.logger.Error(ex);
                    }
                });
            }
        }

        private void OnPostClipboardContentFile(StringCollection value)
        {
            if (configuration.ActiveFileDropList)
            {
                InvokeSTATThread(() =>
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetFileDropList(value);
                    }
                    catch (System.Runtime.InteropServices.ExternalException eex)
                    {
                        OCMClip.logger.Diagnostic(eex.ToString());
                    }
                    catch (Exception ex)
                    {
                        OCMClip.logger.Error(ex);
                    }
                });
            }
        }

        private void InvokeSTATThread(Action action)
        {
            if (UseOwnThread)
            {
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, sta).GetAwaiter().GetResult();
            }
            else
                action();
        }

        public void StartTimer()
        {
            try
            {
                int refreshRateMilliseconds = configuration.RefreshRateMilliseconds;
                int refreshRateSeconds = configuration.RefreshRateSeconds;

                StopTimer();
                OnStartTimer(refreshRateMilliseconds, refreshRateSeconds);
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
            finally
            {
                isRestarting = false;
            }
        }

        public void StopTimer()
        {
            try
            {
                isRestarting = true;
                if (dispatcherTimer != null)
                {
                    OnStopTimer();
                }
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        public bool IsAlive()
        {
            try
            {
                if (dispatcherTimer == null || !dispatcherTimer.Enabled)
                {
                    return isRestarting ? true : false;
                }
                return true;
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
                return false;
            }
        }

        public void GetClipboard()
        {
            try
            {
                OnGetClipboardContent();
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        public void PostClipboard(string value, ClipHandler.Entities.Enums.TextDataFormat format)
        {
            try
            {
                OnPostClipboardContentString(value, format);
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        public void PostClipboard(Image value)
        {
            try
            {
                OnPostClipboardContentImage(value);
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        public void PostClipboard(StringCollection value)
        {
            try
            {
                OnPostClipboardContentFile(value);
            }
            catch (Exception ex)
            {
                OCMClip.logger.Error(ex);
            }
        }

        /// <summary>
        /// Change the current configuration for the Watcher
        /// If the Timer was started when the configuration change happend and the new refresh rates are different the Watcher will be restarted
        /// </summary>
        /// <param name="_configuration"></param>
        public void ConfigurationChange(ConfigurationWatcher _configuration)
        {
            bool requieredTimerRestart = false;
            if (_configuration != null && configuration != null
                && (_configuration.RefreshRateMilliseconds != configuration.RefreshRateMilliseconds
                || _configuration.RefreshRateSeconds != configuration.RefreshRateSeconds)
                && IsAlive())
            {
                requieredTimerRestart = true;
            }
            configuration = _configuration;
            if (requieredTimerRestart)
            {
                StartTimer();
            }
        }
    }
}
