using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMClip.ClipHandler.Entities
{
    public abstract class ClipData
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationWindowTitle { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
    }
}
