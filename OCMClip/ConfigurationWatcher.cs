using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public class ConfigurationWatcher
    {
        public int RefreshRateMilliseconds { get; private set; }
        public int RefreshRateSeconds { get; private set; }
        public bool ActiveText { get; private set; }
        public bool ActiveImage { get; private set; }
        public bool ActiveAudio { get; private set; }
        public bool ActiveFileDropList { get; private set; }
    }
}
