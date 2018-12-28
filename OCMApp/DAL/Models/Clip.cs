using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public abstract class Clip
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationWindowTitle { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }

        protected void Set(OCMClip.ClipHandler.Entities.ClipData entity)
        {
            Id = entity.Id;
            DateCreated = entity.DateCreated;
            ApplicationName = entity.ApplicationName;
            ApplicationWindowTitle = entity.ApplicationWindowTitle;
            ProcessName = entity.ProcessName;
            ProcessId = entity.ProcessId;
        }
    }
}
