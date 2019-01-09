using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler.Entities
{
    public class ClipDataText : ClipData, IDisposable
    {
        public string Value { get; set; }
        public Enums.TextDataFormat SourceTextFormat { get; set; }

        public new void Dispose()
        {
            Value = null;
            base.Dispose();
        }
    }
}
