﻿using EncTemplatesMgr.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.ViewModel
{
    public class FilterData : INotifyPropertyChanged
    {
        public bool _includeAllTemplates;
        /// <summary>
        /// Include all templates in filter regardless of other filter settings.
        /// </summary>
        public bool IncludeAllTemplates
        {
            get => _includeAllTemplates;
            set
            {
                _includeAllTemplates = value;
                NotifyPropertyChanged();
            }
        }

        private string _filterFilePath;
        /// <summary>
        /// File path to filter on.
        /// </summary>
        public string FilterFilePath 
        { 
            get => _filterFilePath; 
            set
            {
                _filterFilePath = value;
                NotifyPropertyChanged();
            }
        }

        private bool _filePathRequired;
        /// <summary>
        /// File path must match to be included in results.
        /// </summary>
        public bool FilePathRequired 
        { 
            get => _filePathRequired;
            set
            {
                _filePathRequired = value;
                NotifyPropertyChanged();
            }
        }

        private string _filterTemplateName;
        /// <summary>
        /// Template name to filter on.
        /// </summary>
        public string FilterTemplateName 
        { 
            get => _filterTemplateName; 
            set
            {
                _filterTemplateName = value;
                NotifyPropertyChanged();
            }
        }

        private bool _templateNameRequired;
        /// <summary>
        /// Template name must match to be included in results.
        /// </summary>
        public bool TemplateNameRequired 
        { 
            get => _templateNameRequired; 
            set
            {
                _templateNameRequired = value;
                NotifyPropertyChanged();
            }
        }

        private bool _fieldValuesRequired;
        /// <summary>
        /// Field values must have a match to be included in results.
        /// </summary>
        public bool FieldValuesRequired 
        { 
            get => _fieldValuesRequired; 
            set
            {
                _fieldValuesRequired = value;
                NotifyPropertyChanged();
            }
        }

        private bool _allFieldValuesRequired;
        /// <summary>
        /// All field values must have a match to be included in results.
        /// </summary>
        public bool AllFieldValuesRequired 
        { 
            get => _allFieldValuesRequired; 
            set
            {
                _allFieldValuesRequired = value;
                NotifyPropertyChanged();
            }
        }

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

        public void NotifyPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Create a template data filter based on UI filter fields.
        /// </summary>
        /// <returns>Filter built from UI data.</returns>
        public Filter ToFilter()
        {
            return new Filter
            {
                IncludeAllTemplates = IncludeAllTemplates,
                FilterFilePath = FilterFilePath,
                FilePathRequired = FilePathRequired,
                FilterTemplateName = FilterTemplateName,
                TemplateNameRequired = TemplateNameRequired,
                FieldValuesRequired = FieldValuesRequired,
                AllFieldValuesRequired = AllFieldValuesRequired,
                FilterFieldValues = FilterFieldValues
            };
        }
    }
}
