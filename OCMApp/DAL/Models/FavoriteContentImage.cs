using OCMClip.ClipHandler.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class FavoriteContentImage
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public Enums.ImageFormatType FormatType { get; set; }

        public FavoriteContentImage()
        {

        }

        public FavoriteContentImage(byte[] content, Enums.ImageFormatType formatType) : this()
        {
            Content = content;
            FormatType = formatType;
        }
    }
}
