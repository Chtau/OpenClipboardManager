using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OCMClip.ClipHandler
{
    internal class ClipImage
    {
        private readonly ILogger logger;
        private Image lastValue = null;

        public ClipImage(ILogger _logger, Image _lastValue)
        {
            logger = _logger;
            lastValue = _lastValue;
        }

        public Entities.ClipDataImage Handle(Image image)
        {
            if (image != null && !Nativ.CompareMemCmp(image ,lastValue))
            {
                if (OnGetEntity(image, out Entities.ClipDataImage entity))
                {
                    lastValue = image;
                    return entity;
                }
            }
            return null;
        }

        private bool OnGetEntity(Image image, out Entities.ClipDataImage entity)
        {
            entity = null;
            try
            {
                var nativData = Nativ.GetActiveWindow();
                entity = new Entities.ClipDataImage
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    Value = ConvertImage.ImageToByteArray(image, ConvertImage.ImageFormatType.Png),
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
