using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class FavoriteContentText
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Content { get; set; }

        public FavoriteContentText()
        {

        }

        public FavoriteContentText(string content) : this()
        {
            Content = content;
        }
    }
}
