using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public Settings Settings { get; set; }
        public List<KeyValuePair<int, string>> ClipWatcherImageFormatTypeEnum { get; set; }
        public int ClipWatcherImageFormatTypeEnumSelected { get; set; }

        public SettingsViewModel()
        {
            Settings = Internal.Global.Instance.Settings;
            ClipWatcherImageFormatTypeEnumSelected = (int)Settings.ClipWatcherDefaultImageFormat;
            ClipWatcherImageFormatTypeEnum = Helper.ComboBoxBindingModelBuilder.FromEnum(typeof(OCMClip.ClipHandler.Entities.Enums.ImageFormatType));
        }
    }
}
