using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public class Configuration
    {
        public ClipHandler.Entities.Enums.ImageFormatType DefaultImageFormat { get; private set; }
        public string LastClipboardTextValue { get; private set; }
        public System.Drawing.Image LastClipboardImageValue { get; private set; }
        public List<string> LastClipboardFileValue { get; private set; }

        public ConfigurationWatcher ConfigurationWatcher { get; private set; }

        public Configuration(ConfigurationWatcher configurationWatcher,
            string lastClipboardTextValue = null,
            System.Drawing.Image lastClipboardImageValue = null,
            List<string> lastClipboardFileValue = null,
            ClipHandler.Entities.Enums.ImageFormatType defaultImageFormatType = ClipHandler.Entities.Enums.ImageFormatType.Png)
        {
            ConfigurationWatcher = configurationWatcher;
            DefaultImageFormat = defaultImageFormatType;
            LastClipboardFileValue = lastClipboardFileValue;
            LastClipboardImageValue = lastClipboardImageValue;
            LastClipboardTextValue = lastClipboardTextValue;
        }
    }
}
