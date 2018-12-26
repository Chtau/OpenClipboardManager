using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Models
{
    public class Settings
    {
        public string Culture { get; set; } = "en-EN";
        public bool AutoStart { get; set; } = false;

        #region OCMClip
        public int ClipWatcherRefreshRateMilliseconds { get; set; } = 50;
        public int ClipWatcherRefreshRateSeconds { get; set; } = 0;
        public bool ClipWatcherActiveText { get; set; } = true;
        public bool ClipWatcherActiveImage { get; set; } = true;
        public bool ClipWatcherActiveFile { get; set; } = true;

        public OCMClip.ClipHandler.Entities.Enums.ImageFormatType ClipWatcherDefaultImageFormat { get; set; } = OCMClip.ClipHandler.Entities.Enums.ImageFormatType.Png;

        #endregion


    }
}
