using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler.Entities
{
    public class ClipDataText : ClipData
    {
        public string Value { get; set; }
        public System.Windows.Forms.TextDataFormat SourceTextFormat { get; set; }
    }
}
