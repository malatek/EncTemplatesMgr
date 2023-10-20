using EncTemplatesMgr.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncTemplatesMgr.Helpers
{
    internal static class TypeConverters
    {
        /// <summary>
        /// Converts an obserable collection to a dictionary.
        /// </summary>
        /// <param name="fieldDataCollection"></param>
        /// <returns>Dictionary of string, string.</returns>
        public static Dictionary<string, string> FieldDataCollectionToDictionary(ObservableCollection<FieldData> fieldDataCollection)
        {
            var dictionary = new Dictionary<string, string>();

            if (fieldDataCollection == null)
                return dictionary;

            foreach (var item in fieldDataCollection)
            {
                if (string.IsNullOrEmpty(item.FieldId))
                    continue;

                dictionary.Add(item.FieldId, item.FieldValue);
            }

            return dictionary;
        }
    }
}
