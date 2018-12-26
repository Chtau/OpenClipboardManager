using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp
{
    public class OCMClipLogger : OCMClip.ILogger
    {
        public void Diagnostic(string message)
        {
            Log.Debug(message);
        }

        public void Error(Exception ex)
        {
            Log.Error(ex.ToString());
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Info(string message)
        {
            Log.Information(message);
        }

        public void Warn(string message)
        {
            Log.Warning(message);
        }
    }
}
