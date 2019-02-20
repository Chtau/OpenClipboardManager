using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class Blacklist
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Value { get; set; }
        [Ignore]
        public string NormalizedValue
        {
            get
            {
                return Value?.ToUpper();
            }
        }

        public Blacklist()
        {
            Id = Guid.NewGuid();
        }

        public Blacklist(string value) : this()
        {
            Value = value;
        }
    }
}
