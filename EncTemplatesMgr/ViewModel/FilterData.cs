using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.ViewModel
{
    internal class FilterData : INotifyPropertyChanged
    {
        //todo: replace the filter properties in WPFform.xaml.cs with this
        public bool IncludeAllTemplates { get; set; }
        public string FilterFilePath { get; set; }
        public bool FilePathRequired { get; set; }
        public string FilterTemplateName { get; set; }
        public bool TemplateNameRequired { get; set; }
        private ObservableCollection<FieldData> _filterFieldData = new ObservableCollection<FieldData>(new List<FieldData>());
        public Dictionary<string, string> FilterFieldValues 
        { 
            get
            {
                return Helpers.TypeConverters.FieldDataCollectionToDictionary(_filterFieldData);
            }
        }
        public bool FieldValuesRequired { get; set; }
        public bool AllFieldValuesRequired { get; set; }

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
