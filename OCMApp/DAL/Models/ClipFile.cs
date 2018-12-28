using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp.DAL.Models
{
    public class ClipFile : Clip
    {
        public List<string> Value { get; set; }
    }
}
