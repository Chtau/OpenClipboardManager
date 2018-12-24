using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCMClip.ClipHandler.Entities;

namespace OCMClip.ClipHandler.Entities
{
    public class ClipDataImage : ClipData
    {
        public byte[] Value { get; set; }
        public Enums.ImageFormatType FormatType { get; set; }
    }
}
