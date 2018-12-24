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
        public ConfigurationWatcher ConfigurationWatcher { get; private set; }

        public Configuration(ConfigurationWatcher configurationWatcher, 
            ClipHandler.Entities.Enums.ImageFormatType defaultImageFormatType = ClipHandler.Entities.Enums.ImageFormatType.Png)
        {
            ConfigurationWatcher = configurationWatcher;
            DefaultImageFormat = defaultImageFormatType;
        }
    }
}
