using OCMClip.ClipHandler.Entities;
using Serilog;
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
            try
            {
                if (Value != null)
                    Preview = OCMClip.ClipHandler.ConvertImage.ImageToByteArray(OCMClip.ClipHandler.ConvertImage.ResizeImage(Value, 360, 360), Enums.ImageFormatType.Png);
            } catch (Exception ex)
            {
                Log.Error(ex, "Could not convert Image for Preview");
                Preview = Value;
            }
            FormatType = entity.FormatType;
            base.Set(entity);
        }
    }
}
