using EllieMae.EMLite.DataEngine;
using EncTemplatesMgr.Helpers;
using System;
using System.Collections.Generic;

namespace EncTemplatesMgr.Models
{
    public class CustomClosingCostTemplate : Interfaces.ICustomTemplate
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
        /// File Path where template would reside in Encompass.
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
        /// Converts ClosingCost Template to CustomClosingCostTemplate. FilePath will not be added to the custom template.
        /// </summary>
        /// <param name="closingCost">Encompass Closing Cost Template.</param>
        public static explicit operator CustomClosingCostTemplate(ClosingCost closingCost)
        {
            var customClosingCostTemplate = new CustomClosingCostTemplate()
            {
                Name = closingCost.TemplateName,
                Description = closingCost.Description
            };

            var fieldsAndValues = new Dictionary<string, string>();

            foreach (var field in closingCost.GetAssignedFieldIDs())
            {
                var fieldValue = closingCost.GetField(field);
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    fieldsAndValues.Add(field, fieldValue);
                }
            }

            customClosingCostTemplate.FieldIDsAndValues = fieldsAndValues;

            return customClosingCostTemplate;
        }

        /// <summary>
        /// Convert this to an Encompass ClosingCost Template.
        /// </summary>
        /// <returns>New ClosingCost Template.</returns>
        public ClosingCost ConvertToClosingCostTemplate()
        {
            var closingCost = new ClosingCost()
            {
                TemplateName = this.Name,
                Description = this.Description
            };

            foreach (var kvp in this._fieldIDsAndValues)
            {
                try
                {
                    closingCost.SetField(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error("TemplateImport", ex);
                }
            }

            return closingCost;
        }
    }
}
