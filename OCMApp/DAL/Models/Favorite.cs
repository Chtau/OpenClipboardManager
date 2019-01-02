using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class Favorite
    {
        public enum ContentType
        {
            Text,
            Image,
            File
        }

        [PrimaryKey]
        public Guid Id { get; set; }
        public int Key { get; set; }
        public int Modifier1 { get; set; }
        public int Modifier2 { get; set; }
        public Guid FavoriteContentId { get; set; }
        public ContentType Type { get; set; }
    }
}
