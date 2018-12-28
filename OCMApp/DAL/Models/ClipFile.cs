using OCMClip.ClipHandler.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class ClipFile : Clip
    {
        public string Value { get; set; }

        public ClipFile()
        {

        }

        public ClipFile(ClipDataFile entity) : this()
        {
            if (entity.Value != null)
            {
                Value = "";
                for (int i = 0; i < entity.Value.Count; i++)
                {
                    Value += entity.Value[i];
                    if (i < (entity.Value.Count - 1))
                        Value += ";";
                }
            }
            base.Set(entity);
        }
    }
}
