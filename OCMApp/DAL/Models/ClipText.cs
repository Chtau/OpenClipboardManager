using OCMClip.ClipHandler.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class ClipText : Clip
    {
        public string Preview { get; set; }
        public string Value { get; set; }
        public Enums.TextDataFormat SourceTextFormat { get; set; }

        public ClipText()
        {

        }

        public ClipText(ClipDataText entity) : this()
        {
            Value = entity.Value;
            if (Value.Length > 55)
                Preview = Value.Substring(0, 55);
            else
                Preview = Value;
            SourceTextFormat = entity.SourceTextFormat;
            base.Set(entity);
        }
    }
}
