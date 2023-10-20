using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.ViewModel
{
    internal class FilterData : INotifyPropertyChanged
    {
        //todo: replace the filter properties in WPFform.xaml.cs with this

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
