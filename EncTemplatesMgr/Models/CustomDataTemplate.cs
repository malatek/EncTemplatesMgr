using EllieMae.EMLite.DataEngine;
using EncTemplatesMgr.Helpers;
using System;
using System.Collections.Generic;

namespace EncTemplatesMgr.Models
{
    public class CustomDataTemplate : Interfaces.ICustomTemplate
    {
        /// <summary>
        /// Template Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Template Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// File path where template would reside in Encompass.
        /// </summary>
        public string FilePath { get; set; }

        private Dictionary<string, string> _fieldIDsAndValues;

        /// <summary>
        /// Field IDs and their associated values.
        /// </summary>
        public Dictionary<string, string> FieldIDsAndValues 
        {
            get
            {
                if (this._fieldIDsAndValues == null)
                {
                    this._fieldIDsAndValues = new Dictionary<string, string>();
                }
                return this._fieldIDsAndValues;
            }

            set { this._fieldIDsAndValues = value; }
        }

        /// <summary>
        /// Respa Form version.
        /// </summary>
        public string RespaTilaFormVersion { get; set; }

        /// <summary>
        /// Template Ignores business rules.
        /// </summary>
        public bool IgnoreBusinessRules { get; set; }

        /// <summary>
        /// Converts DataTemplate to CustomDataTemplate. FilePath will not be converted.
        /// </summary>
        /// <param name="dataTemplate">Encompass Data Template.</param>
        public static explicit operator CustomDataTemplate(DataTemplate dataTemplate)
        {
            var customDataTemplate = new CustomDataTemplate()
            {
                Name = dataTemplate.TemplateName,
                Description = dataTemplate.Description,
                RespaTilaFormVersion = dataTemplate.RESPAVersion,
                IgnoreBusinessRules = dataTemplate.IgnoreBusinessRules
            };

            var fieldsAndValues = new Dictionary<string, string>();

            foreach (var field in dataTemplate.GetAssignedFieldIDs())
            {
                var fieldValue = dataTemplate.GetField(field);
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    fieldsAndValues.Add(field, fieldValue);
                }
            }

            customDataTemplate.FieldIDsAndValues = fieldsAndValues;

            return customDataTemplate;
        }

        /// <summary>
        /// Convert this to an Encompass DataTemplate.
        /// </summary>
        /// <returns>New DataTemplate.</returns>
        public DataTemplate ToDataTemplate()
        {
            DataTemplate newDataTemplate = new DataTemplate
            {
                TemplateName = this.Name,
                RESPAVersion = this.RespaTilaFormVersion,
                Description = this.Description,
                IgnoreBusinessRules = this.IgnoreBusinessRules
            };

            foreach (var kvp in this.FieldIDsAndValues)
            {
                try
                {
                    newDataTemplate.SetField(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("TemplateImport", ex);
                }
            }

            return newDataTemplate;
        }
    }
}
