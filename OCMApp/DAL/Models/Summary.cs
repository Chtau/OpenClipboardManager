using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class Summary
    {
        public int Total { get; set; }
        public int Text { get; set; }
        public int Image { get; set; }
        public int File { get; set; }
        public string Application { get; set; }
    }
}
