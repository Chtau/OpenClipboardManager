using OCMClip;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp
{
    public class StartUp
    {
        #region Singleton
        private static volatile StartUp instance;
        private static readonly object syncRoot = new Object();

        private StartUp()
        {
        }

        public static StartUp Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new StartUp();
                    }
                }

                return instance;
            }
        }
        #endregion

        public OCMClip.OCMClip Clip { get; private set; }

        private bool isInit = false;
        public void Init()
        {
            if (!isInit)
            {
                isInit = true;
                
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

                Clip = new OCMClip.OCMClip(new OCMClipLogger());
                Clip.Load(new Configuration(
                    new ConfigurationWatcher(50, 0, true, true, true),
                    null, null, null,
                    OCMClip.ClipHandler.Entities.Enums.ImageFormatType.Png
                    ));
            }
        }

        public void Close()
        {
            Clip.Dispose();
            Log.CloseAndFlush();
        }
    }
}
