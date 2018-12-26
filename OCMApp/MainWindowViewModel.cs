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
        public List<OCMClip.ClipHandler.Entities.ClipDataText> ClipDataTexts { get; set; }

        public MainWindowViewModel()
        {
            ClipDataTexts = new List<OCMClip.ClipHandler.Entities.ClipDataText>();
        }
    }
}
