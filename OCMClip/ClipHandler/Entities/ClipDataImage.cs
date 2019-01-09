using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCMClip.ClipHandler.Entities;

namespace OCMClip.ClipHandler.Entities
{
    public class ClipDataImage : ClipData, IDisposable
    {
        public byte[] Value { get; set; }
        public Enums.ImageFormatType FormatType { get; set; }

        public new void Dispose()
        {
            Value = null;
            base.Dispose();
        }
    }
}
