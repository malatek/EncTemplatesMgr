using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.DataEngine.Log;
using EncTemplatesMgr.Helpers;
using EncTemplatesMgr.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace EncTemplatesMgr.Models
{
    public class Filter
    {
        /// <summary>
        /// Include all templates.
        /// </summary>
        public bool IncludeAllTemplates { get; set; }

        /// <summary>
        /// Check if fileSystemEntry.Path contains this.
        /// </summary>
        public string FilterFilePath { get; set; }

        /// <summary>
        /// If true, file path must match.
        /// </summary>
        public bool FilePathRequired { get; set; }

        /// <summary>
        /// Check if fileSystemEntry.Name contains this.
        /// </summary>
        public string FilterTemplateName { get; set; }

        /// <summary>
        /// If true, template name must match.
        /// </summary>
        public bool TemplateNameRequired { get; set; }

        /// <summary>
        /// Check if template data contains a match for a field with the given value.
        /// </summary>
        public Dictionary<string, string> FilterFieldValues { get; set; }

        /// <summary>
        /// If true, filed value must return a match. If false, any criteria can match..
        /// </summary>
        public bool FieldValuesRequired { get; set; }

        /// <summary>
        /// If true, all field values must match. If false, any field values can match.
        /// </summary>
        public bool AllFieldValuesRequired { get; set; }

        /// <summary>
        /// Filter the provided objects.
        /// </summary>
        public Filter() { }

        /// <summary>
        /// Filter templates based on provided data.
        /// </summary>
        /// <param name="customTemplate">Custom Template to check data against.</param>
        /// <returns>Template match found.</returns>
        public bool TemplateInFilter(ICustomTemplate customTemplate)
        {
            var resultFound = false;
            if (this.IncludeAllTemplates)
                return true;

            if (this.FilterFilePath != string.Empty && customTemplate.FilePath.Contains(this.FilterFilePath))
            {
                resultFound = true;
            }
            else if (this.FilePathRequired)
            {
                return false;
            }

            if (this.FilterTemplateName != string.Empty && customTemplate.Name.Contains(this.FilterTemplateName))
            {
                resultFound = true;
            }
            else if (this.TemplateNameRequired)
            {
                return false;
            }

            if ((this.FilterFieldValues != null && this.FilterFieldValues.Count > 0) &&
                this.CheckTemplateData(customTemplate))
            {
                resultFound = true;
            }
            else if (this.FieldValuesRequired)
            {
                return false;
            }

            return resultFound;
        }

        /// <summary>
        /// Filter templates based on provided data.
        /// </summary>
        /// <param name="fileSystemEntry">File system entry to verify if data matches against.</param>
        /// <param name="fieldDataTemplate">FieldDataTemplate template to verify if data matches against.</param>
        /// <returns>True if a match is found.</returns>
        public bool TemplateInFilter(FileSystemEntry fileSystemEntry, FieldDataTemplate fieldDataTemplate)
        {
            var resultFound = false;
            if (this.IncludeAllTemplates)
                return true;

            if (this.FilterFilePath != string.Empty && fileSystemEntry.Path.Contains(this.FilterFilePath))
            {
                resultFound = true;
            }
            else if (this.FilePathRequired)
            {
                return false;
            }

            if (this.FilterTemplateName != string.Empty && fileSystemEntry.Name.Contains(this.FilterTemplateName))
            {
                resultFound = true;
            }
            else if (this.TemplateNameRequired)
            {
                return false;
            }

            if ((this.FilterFieldValues != null && this.FilterFieldValues.Count > 0) &&
                this.CheckTemplateData(fieldDataTemplate))
            {
                resultFound = true;
            }
            else if (this.FieldValuesRequired)
            {
                return false;
            }

            return resultFound;
        }

        private bool CheckTemplateData(ICustomTemplate customTemplate)
        {
            var valueFound = false;
            foreach (var fieldValue in this.FilterFieldValues)
            {
                try
                {
                    if (customTemplate.FieldIDsAndValues[fieldValue.Key] != fieldValue.Value)
                    {
                        if (this.AllFieldValuesRequired)
                            return false;
                    }
                    else
                    {
                        valueFound = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"CheckTemplateData Error comparing field: {fieldValue.Key}; value: {fieldValue.Value}; in {customTemplate.Name}. Exception: {ex}");
                    return false;
                }
            }

            return valueFound;
        }

        private bool CheckTemplateData(FieldDataTemplate fieldDataTemplate)
        {
            var valueFound = false;
            foreach (var kvp in this.FilterFieldValues)
            {
                try
                {
                    if (fieldDataTemplate.GetField(kvp.Key) != kvp.Value)
                    {
                        if (this.AllFieldValuesRequired)
                            return false;
                    }
                    else
                    {
                        valueFound = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"CheckTemplateData Error comparing field: {kvp.Key}; value: {kvp.Value}; in {fieldDataTemplate.TemplateName}. Exception: {ex}");
                    return false;
                }
            }

            return valueFound;
        }
    }
}
