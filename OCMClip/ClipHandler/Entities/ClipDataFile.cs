using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler.Entities
{
    public class ClipDataFile : ClipData, IDisposable
    {
        public List<string> Value { get; set; }

        public new void Dispose()
        {
            Value = null;
            base.Dispose();
        }
    }
}
