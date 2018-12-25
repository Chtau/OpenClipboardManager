using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler
{
    internal class ClipFile
    {
        private readonly ILogger logger;
        private readonly Configuration configuration;
        private List<string> lastValue = null;

        public ClipFile(Configuration _configuration, ILogger _logger)
        {
            configuration = _configuration;
            logger = _logger;
            lastValue = configuration.LastClipboardFileValue;
        }

        public Entities.ClipDataFile Handle(StringCollection textValue)
        {
            logger.Diagnostic("ClipFile handle => " + textValue.Count);

            List<string> list = textValue.Cast<string>().ToList();
            if (list != null && list.Count() > 0 && !OnCompareEqualsListValues(list, lastValue))
            {
                if (OnGetEntity(list, out Entities.ClipDataFile entity))
                {
                    lastValue = list;
                    return entity;
                }
            }
            return null;
        }

        private bool OnCompareEqualsListValues(List<string> l1, List<string> l2)
        {
            if (l2 == null || l2.Count() == 0)
                return false;
            return l1.Except(l2).Count() == 0;
        }

        private bool OnGetEntity(List<string> value, out Entities.ClipDataFile entity)
        {
            entity = null;
            try
            {
                var nativData = Nativ.GetActiveWindow();
                entity = new Entities.ClipDataFile
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    Value = value,
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
