using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal sealed class ClipHandle
    {
        #region Singleton
        private static volatile ClipHandle instance;
        private static readonly object syncRoot = new Object();

        private ClipHandle() { }

        public static ClipHandle Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ClipHandle();
                    }
                }

                return instance;
            }
        }
        #endregion

        private ClipHandler.ClipText clipText;
        private ClipHandler.ClipImage clipImage;
        private ClipHandler.ClipFile clipFile;
        private Configuration configuration;

        public void Init(ILogger logger, Configuration _configuration)
        {
            configuration = _configuration;
            clipText = new ClipHandler.ClipText(configuration, logger, null);
            clipImage = new ClipHandler.ClipImage(configuration, logger, null);
            clipFile = new ClipHandler.ClipFile(configuration, logger, null);
        }

        public Entities.ClipDataText Text(string value)
        {
            return clipText.Handle(value);
        }

        public Entities.ClipDataImage Image(System.Drawing.Image value)
        {
            return clipImage.Handle(value);
        }

        public Entities.ClipDataFile File(System.Collections.Specialized.StringCollection value)
        {
            return clipFile.Handle(value);
        }
    }
}
