using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip
{
    public class Manager
    {
        internal static ILogger logger;

        public Manager(ILogger _logger)
        {
            logger = _logger;
        }
    }
}
