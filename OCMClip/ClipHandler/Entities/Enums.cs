using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler.Entities
{
    public class Enums
    {
        public enum TextDataFormat
        {
            //
            // Summary:
            //     Specifies the standard ANSI text format.
            Text = 0,
            //
            // Summary:
            //     Specifies the standard Windows Unicode text format.
            UnicodeText = 1,
            //
            // Summary:
            //     Specifies text consisting of rich text format (RTF) data.
            Rtf = 2,
            //
            // Summary:
            //     Specifies text consisting of HTML data.
            Html = 3,
            //
            // Summary:
            //     Specifies a comma-separated value (CSV) format, which is a common interchange
            //     format used by spreadsheets.
            CommaSeparatedValue = 4
        }

        public enum ImageFormatType
        {
            Png = 0,
            Bmp = 1,
            Emf = 2,
            Exif = 3,
            Gif = 4,
            Icon = 5,
            Jpeg = 6,
            Tiff = 7,
            Wmf = 8
        }
    }
}
