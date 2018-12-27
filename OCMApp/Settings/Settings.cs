﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Settings
{
    public class Settings
    {
        public Internal.Localize.Language Culture { get; set; } = Internal.Localize.Language.English;
        public bool AutoStart { get; set; } = false;
        public string DataPath { get; set; } = null;

        #region OCMClip
        public bool UseWatcher { get; set; } = false;

        public int ClipWatcherRefreshRateMilliseconds { get; set; } = 50;
        public int ClipWatcherRefreshRateSeconds { get; set; } = 0;
        public bool ClipWatcherActiveText { get; set; } = true;
        public bool ClipWatcherActiveImage { get; set; } = true;
        public bool ClipWatcherActiveFile { get; set; } = true;

        public OCMClip.ClipHandler.Entities.Enums.ImageFormatType ClipWatcherDefaultImageFormat { get; set; } = OCMClip.ClipHandler.Entities.Enums.ImageFormatType.Png;

        #endregion

        #region OCMHotKey

        public OCMHotKey.Enums.Key ClipKey { get; set; } = OCMHotKey.Enums.Key.C;
        public OCMHotKey.Enums.KeyModifier ClipKeyModifier { get; set; } = OCMHotKey.Enums.KeyModifier.Ctrl | OCMHotKey.Enums.KeyModifier.Shift;

        #endregion
    }
}
