using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EncTemplatesMgr.Helpers;
using EncTemplatesMgr.Interfaces;
using System;
using System.Collections.Generic;

namespace EncTemplatesMgr.Common
{
    public class Filter
    {
        /// <summary>
        /// Check if fileSystemEntry.Path contains this.
        /// </summary>
        public string FilterFilePath { get; set; }

        /// <summary>
        /// Check if fileSystemEntry.Name contains this.
        /// </summary>
        public string FilterTemplateName { get; set; }

        /// <summary>
        /// Check if template data contains a match for a field with the given value.
        /// </summary>
        public Dictionary<string, string> FilterFieldValues { get; set; }

        /// <summary>
        /// Filter the provided objects.
        /// </summary>
        public Filter() { }

        /// <summary>
        /// Filter templates based on provided data.
        /// </summary>
        /// <param name="customTemplate">Custom Template to check data against.</param>
        /// <returns></returns>
        public bool TemplateInFilter(ICustomTemplate customTemplate)
        {
            if (this.FilterFilePath == "*" || this.FilterTemplateName == "*")
                return true;

            if (this.FilterFilePath != string.Empty && customTemplate.FilePath.Contains(this.FilterFilePath))
                return true;

            if (this.FilterTemplateName != string.Empty && customTemplate.Name.Contains(this.FilterTemplateName))
                return true;

            if (this.FilterFieldValues == null || this.FilterFieldValues.Count < 1)
                return false;

            return CheckTemplateData(customTemplate);
        }

        /// <summary>
        /// Filter templates based on provided data.
        /// </summary>
        /// <param name="fileSystemEntry">File system entry to verify if data matches against.</param>
        /// <param name="fieldDataTemplate">FieldDataTemplate template to verify if data matches against.</param>
        /// <returns>True if a match is found.</returns>
        public bool TemplateInFilter(FileSystemEntry fileSystemEntry, FieldDataTemplate fieldDataTemplate)
        {
            if (this.FilterFilePath == "*" || this.FilterTemplateName == "*")
                return true;

            if (this.FilterFilePath != string.Empty && fileSystemEntry.Path.Contains(this.FilterFilePath))
                return true;

            if (this.FilterTemplateName != string.Empty && fileSystemEntry.Name.Contains(this.FilterTemplateName))
                return true;

            if (this.FilterFieldValues == null || FilterFieldValues.Count < 1)
                return false;

            return CheckTemplateData(fieldDataTemplate);
    
        }

        private bool CheckTemplateData(ICustomTemplate customTemplate)
        {
            foreach (var fieldValue in this.FilterFieldValues)
            {
                try
                {
                    if (customTemplate.FieldIDsAndValues[fieldValue.Key] != fieldValue.Value)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Verbose("CheckTemplateDetails " + ex.ToString());
                    return false;
                }
            }

            return true;
        }

        private bool CheckTemplateData(FieldDataTemplate fieldDataTemplate)
        {
            // ToDo: this functions as an 'and' filter, consider implementing an 'or' filter as well.
            foreach (var kvp in this.FilterFieldValues)
            {
                try
                {
                    if (fieldDataTemplate.GetField(kvp.Key) != kvp.Value)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"CheckTemplateData Error comparing field: {kvp.Key}; value: {kvp.Value}; in {fieldDataTemplate.TemplateName}. Exception: {ex}");
                    return false;
                }
            }

            return true;
        }
    }
}
