using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCMApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public List<OCMClip.ClipHandler.Entities.ClipDataText> ClipDataTexts { get; set; }

        public MainWindowViewModel()
        {
            ClipDataTexts = new List<OCMClip.ClipHandler.Entities.ClipDataText>();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
