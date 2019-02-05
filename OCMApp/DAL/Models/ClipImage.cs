using OCMClip.ClipHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class ClipImage : Clip
    {
        public byte[] Preview { get; set; }
        public byte[] Value { get; set; }
        public Enums.ImageFormatType FormatType { get; set; }

        public ClipImage()
        {

        }

        public ClipImage(ClipDataImage entity) : this()
        {
            Value = entity.Value;
            Preview = Value;
            FormatType = entity.FormatType;
            base.Set(entity);
        }
    }
}
