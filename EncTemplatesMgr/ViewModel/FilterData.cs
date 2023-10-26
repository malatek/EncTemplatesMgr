using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.ViewModel
{
    public class FilterData : INotifyPropertyChanged
    {
        //todo: replace the filter properties in WPFform.xaml.cs with this
        /// <summary>
        /// Include all templates in filter regardless of other filter settings.
        /// </summary>
        public bool IncludeAllTemplates { get; set; }

        /// <summary>
        /// File path to filter on.
        /// </summary>
        public string FilterFilePath { get; set; }

        /// <summary>
        /// File path must match to be included in results.
        /// </summary>
        public bool FilePathRequired { get; set; }

        /// <summary>
        /// Template name to filter on.
        /// </summary>
        public string FilterTemplateName { get; set; }

        /// <summary>
        /// Template name must match to be included in results.
        /// </summary>
        public bool TemplateNameRequired { get; set; }

        /// <summary>
        /// Field values must have a match to be included in results.
        /// </summary>
        public bool FieldValuesRequired { get; set; }

        /// <summary>
        /// All field values must have a match to be included in results.
        /// </summary>
        public bool AllFieldValuesRequired { get; set; }

        /// <summary>
        /// Field IDs and values to filter on.
        /// </summary>
        public ObservableCollection<FieldData> FilterFieldData = new ObservableCollection<FieldData>(new List<FieldData>());

        /// <summary>
        /// Get FilterFieldData as a Dictionary.
        /// </summary>
        public Dictionary<string, string> FilterFieldValues
        {
            get
            {
                return Helpers.TypeConverters.FieldDataCollectionToDictionary(FilterFieldData);
            }
        }

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
