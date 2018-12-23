using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public interface ILogger
    {
        void Error(Exception ex);
        void Error(string message);
        void Warn(string message);
        void Info(string message);
        void Diagnostic(string message);
    }
}
