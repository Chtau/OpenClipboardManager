﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal class ClipText
    {
        private readonly ILogger logger;
        private readonly Configuration configuration;
        private string lastValue = null;

        public ClipText(Configuration _configuration, ILogger _logger)
        {
            configuration = _configuration;
            logger = _logger;
            lastValue = configuration.LastClipboardTextValue;
        }

        /// <summary>
        /// creates a Entity for the Clipboard text value is it is a new value
        /// </summary>
        /// <param name="textValue"></param>
        /// <returns></returns>
        public Entities.ClipDataText Handle(string textValue)
        {
            if (!string.IsNullOrWhiteSpace(textValue) && lastValue != textValue)
            {
                if (OnGetEntity(textValue, OnGetCurrentTextFormat(), out Entities.ClipDataText entity))
                {
                    lastValue = textValue;
                    return entity;
                }
            }
            return null;
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
                var nativData = Nativ.GetActiveWindow();
                entity = new Entities.ClipDataText
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    SourceTextFormat = (ClipHandler.Entities.Enums.TextDataFormat)(int)format,
                    Value = value,
                    ApplicationWindowTitle = nativData.Item1,
                    ProcessId = nativData.Item2.Id,
                    ProcessName = nativData.Item2.ProcessName,
                    ApplicationName = nativData.Item2.ProcessName
                };

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
