using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OCMClip.ClipHandler.Entities;

namespace OCMClip.ClipHandler
{
    internal class ClipImage
    {
        private readonly ILogger logger;
        private readonly Configuration configuration;
        private Image lastValue = null;

        public ClipImage(Configuration _configuration, ILogger _logger, Image _lastValue)
        {
            configuration = _configuration;
            logger = _logger;
            lastValue = _lastValue;
        }

        public Entities.ClipDataImage Handle(Image image)
        {
            if (image != null && !Nativ.CompareMemCmp(image ,lastValue))
            {
                if (OnGetEntity(image, configuration.DefaultImageFormat, out Entities.ClipDataImage entity))
                {
                    lastValue = image;
                    return entity;
                }
            }
            return null;
        }

        private bool OnGetEntity(Image image, Enums.ImageFormatType formatType, out Entities.ClipDataImage entity)
        {
            entity = null;
            try
            {
                var nativData = Nativ.GetActiveWindow();
                entity = new Entities.ClipDataImage
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    Value = ConvertImage.ImageToByteArray(image, formatType),
                    FormatType = formatType,
                    ApplicationWindowTitle = nativData.Item1,
                    ProcessId = nativData.Item2.Id,
                    ProcessName = nativData.Item2.ProcessName,
                    ApplicationName = nativData.Item2.ProcessName
                };

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
