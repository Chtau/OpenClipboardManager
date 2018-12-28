using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp
{
    public class MainWindowViewModel : BaseViewModel
    {
        public List<DAL.Models.ClipText> ClipDataTexts { get; set; }

        public MainWindowViewModel()
        {
            ClipDataTexts = Internal.Global.Instance.DBContext.GetClipText().ToList();//new List<DAL.Models.ClipText>();
        }
    }
}
