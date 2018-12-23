using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal class ClipText
    {
        private readonly ILogger logger;
        private string lastValue = null;

        public ClipText(ILogger _logger)
        {
            logger = _logger;
        }

        public void Handle(string textValue)
        {
            logger.Diagnostic("ClipText handle => " + textValue);

            if (!string.IsNullOrWhiteSpace(textValue) && lastValue != textValue)
            {
                if (OnGetEntity(textValue, OnGetCurrentTextFormat(), out Entities.ClipDataText entity))
                {
                    lastValue = textValue;
                    /*if (Internal.Global.Instance.Settings.ShowBallonFromTextClip)
                    {
                        string title = Internal.Global.Instance.Localize.GetText("Text") + " - " + DateTime.Now.ToString("HH:mm");
                        if (Internal.Global.Instance.Settings.ShowCustomBallonTipOnClip)
                            Controls.BallonHelper.ShowCustomBallonText(lastClipText, title);
                        if (Internal.Global.Instance.Settings.ShowSystemBallonTipOnClip)
                            Controls.BallonHelper.ShowSystemBallon(title, lastClipText);
                    }
                    Internal.Global.Instance.InvokeClipChanged(entity, entity != null ? entity.Id : -1);*/
                }
            }
        }

        private System.Windows.Forms.TextDataFormat OnGetCurrentTextFormat()
        {
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
            else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.UnicodeText))
            {
                currentFormat = System.Windows.Forms.TextDataFormat.UnicodeText;
            }
            else if (System.Windows.Forms.Clipboard.ContainsText(System.Windows.Forms.TextDataFormat.Text))
            {
                currentFormat = System.Windows.Forms.TextDataFormat.Text;
            }
            
            return currentFormat;
        }

        private bool OnGetEntity(string value, System.Windows.Forms.TextDataFormat format, out Entities.ClipDataText entity)
        {
            entity = null;
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
