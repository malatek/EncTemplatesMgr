using System.ComponentModel;

namespace EncTemplatesMgr.ViewModel
{
    public class FieldData : INotifyPropertyChanged
    {
        /// <summary>
        /// Encompass Field ID.
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// Value of the field.
        /// </summary>
        public string FieldValue { get; set; }

        public FieldData()
        {

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
