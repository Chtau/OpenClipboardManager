using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class LastClip
    {
        public ClipText ClipText { get; set; }
        public ClipImage ClipImage { get; set; }
        public ClipFile ClipFile { get; set; }

        public LastClip(ClipText clipText, ClipImage clipImage, ClipFile clipFile)
        {
            ClipText = clipText;
            ClipImage = clipImage;
            ClipFile = clipFile;
        }
    }
}
