using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    internal sealed class Watcher
    {
        #region Singleton
        private static volatile Watcher instance;
        private static readonly object syncRoot = new Object();

        private Watcher() { }

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

        private System.Timers.Timer dispatcherTimer;
        /*private string lastClipText = null;
        private byte[] lastClipImage = null;
        private static LastImageClip LastImageClipboardValue = null;*/
        private bool isRestarting = false;
        private ConfigurationWatcher configuration;
        private readonly ClipHandler.ClipText clipText = new ClipHandler.ClipText(Manager.logger);

        private void OnStartTimer(int refreshRateMilliseconds, int refreshRateSeconds)
        {
            try
            {
                dispatcherTimer = new System.Timers.Timer();
                dispatcherTimer.Elapsed += DispatcherTimer_Elapsed;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, refreshRateSeconds, refreshRateMilliseconds).TotalMilliseconds;
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                Manager.logger.Error(ex);
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
                Manager.logger.Error(ex);
            }
        }
        
        private void DispatcherTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            isRestarting = true;
            dispatcherTimer.Enabled = false;
            try
            {
                if (configuration.ActiveText && System.Windows.Forms.Clipboard.ContainsText())
                {
                    /*if (!ApplicationFilter.AllowToUse())
                            return;*/
                    clipText.Handle(System.Windows.Forms.Clipboard.GetText());
                    /*string clip = System.Windows.Forms.Clipboard.GetText();
                    if (!string.IsNullOrWhiteSpace(clip) && lastClipText != clip)
                    {
                        bool handleClip = false;
                        System.Windows.Forms.TextDataFormat currentFormat = System.Windows.Forms.TextDataFormat.Text;
                        if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.CommaSeparatedValue))
                        {
                            currentFormat = System.Windows.Forms.TextDataFormat.CommaSeparatedValue;
                        }
                        else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.Html))
                        {
                            currentFormat = System.Windows.Forms.TextDataFormat.Html;
                        }
                        else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.Rtf))
                        {
                            currentFormat = System.Windows.Forms.TextDataFormat.Rtf;
                        }
                        else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.Text))
                        {
                            currentFormat = System.Windows.Forms.TextDataFormat.Text;
                        }
                        else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.UnicodeText))
                        {
                            currentFormat = System.Windows.Forms.TextDataFormat.UnicodeText;
                        }
                        handleClip = Clip.CopyClipText(clip, currentFormat, out Entities.ClipDataText entity);

                        if (handleClip)
                        {
                            lastClipText = clip;
                            if (Internal.Global.Instance.Settings.ShowBallonFromTextClip)
                            {
                                string title = Internal.Global.Instance.Localize.GetText("Text") + " - " + DateTime.Now.ToString("HH:mm");
                                if (Internal.Global.Instance.Settings.ShowCustomBallonTipOnClip)
                                    Controls.BallonHelper.ShowCustomBallonText(lastClipText, title);
                                if (Internal.Global.Instance.Settings.ShowSystemBallonTipOnClip)
                                    Controls.BallonHelper.ShowSystemBallon(title, lastClipText);
                            }
                            Internal.Global.Instance.InvokeClipChanged(entity, entity != null ? entity.Id : -1);
                        }
                    }*/
                }
                else if (configuration.ActiveImage && System.Windows.Forms.Clipboard.ContainsImage())
                {
                    System.Drawing.Image image = System.Windows.Forms.Clipboard.GetImage();
                    /*if (!ApplicationFilter.AllowToUse())
                        return;

                    System.Drawing.Image image = System.Windows.Forms.Clipboard.GetImage();
                    if (image == null)
                        return;

                    LastImageClip curValue = new LastImageClip()
                    {
                        Width = image.Width,
                        Height = image.Height,
                        Flags = image.Flags,
                        HResolution = image.HorizontalResolution,
                    };

                    if (LastImageClipboardValue != null)
                    {
                        if (LastImageClipboardValue.Width == curValue.Width &&
                            LastImageClipboardValue.Height == curValue.Height &&
                            LastImageClipboardValue.Flags == curValue.Flags &&
                            LastImageClipboardValue.HResolution == curValue.HResolution)
                            return;
                    }
                    LastImageClipboardValue = curValue;

                    var newByteArray = DataConverter.ImageToByteArray(image);
                    if (lastClipImage == null || !lastClipImage.SequenceEqual(newByteArray))
                    {
                        bool handleClip = false;
                        handleClip = Clip.CopyClipImage(image, out Entities.ClipDataImage entity);
                        if (handleClip)
                        {
                            lastClipImage = newByteArray;
                            if (Internal.Global.Instance.Settings.ShowBallonFromTextClip)
                            {
                                string title = Internal.Global.Instance.Localize.GetText("Image") + " - " + DateTime.Now.ToString("HH:mm");
                                if (Internal.Global.Instance.Settings.ShowCustomBallonTipOnClip)
                                    Controls.BallonHelper.ShowCustomBallonImage(image, title);
                                if (Internal.Global.Instance.Settings.ShowSystemBallonTipOnClip)
                                    Controls.BallonHelper.ShowSystemBallon(title);
                            }
                            Internal.Global.Instance.InvokeClipChanged(entity, entity != null ? entity.Id : -1);
                        }
                    }*/
                }
                else if (configuration.ActiveAudio && System.Windows.Forms.Clipboard.ContainsAudio())
                {
                    //no implementation
                    var data = System.Windows.Forms.Clipboard.GetAudioStream();
                }
                else if (configuration.ActiveFileDropList && System.Windows.Forms.Clipboard.ContainsFileDropList())
                {
                    //no implementation
                    var data = System.Windows.Forms.Clipboard.GetFileDropList();
                }
            }
            catch (Exception ex)
            {
                Manager.logger.Error(ex);
            }
            finally
            {
                dispatcherTimer.Enabled = true;
                isRestarting = false;
            }
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
                Manager.logger.Error(ex);
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
                Manager.logger.Error(ex);
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
                Manager.logger.Error(ex);
                return false;
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
