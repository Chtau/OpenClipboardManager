using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.Settings
{
    public class WindowState
    {
        public enum State
        {
            WindowsDefault,
            Remember
        }

        public State CurrentState { get; set; } = State.WindowsDefault;
        public double? Top { get; set; } = null;
        public double? Left { get; set; } = null;
        public double? Height { get; set; } = null;
        public double? Width { get; set; } = null;

        public bool HasRememberState()
        {
            if (CurrentState == State.Remember && Top != null
                && Left != null)
                return true;
            return false;
        }
    }
}
