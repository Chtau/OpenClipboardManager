using OCMClip.ClipHandler.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class FavoriteContentFile
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public FavoriteContentFile()
        {
            Id = Guid.NewGuid();
        }

        public FavoriteContentFile(List<string> content) : this()
        {
            if (content != null)
            {
                Content = "";
                for (int i = 0; i < content.Count; i++)
                {
                    Content += content[i];
                    if (i < (content.Count - 1))
                        Content += ";";
                }
            }
        }

        public List<string> GetListValue()
        {
            List<string> retVal = new List<string>();
            if (!string.IsNullOrWhiteSpace(Content))
            {
                foreach (var item in Content.Split(';'))
                {
                    retVal.Add(item.Trim());
                }
            }
            return retVal;
        }
    }
}
