using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EncTemplatesMgr.Helpers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace EncTemplatesMgr.Common
{
    internal class TemplateUpdater
    {
        /// <summary>
        /// Description to append to the top of the existing template description.
        /// </summary>
        private string _appendDescription;

        /// <summary>
        /// Field ID's and their values to update to.
        /// </summary>
        private Dictionary<string, string> _fieldsAndValues;

        /// <summary>
        /// Type of template(s) to update.
        /// </summary>
        private TemplateSettingsType _templateSettingsType;

        /// <summary>
        /// Filter Object used to determine if templates should be included.
        /// </summary>
        public Filter TemplateFilter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateUpdater"/> class.
        /// </summary>
        /// <param name="templateSettingsType">TemplateSettingsType, only supports MiscData and ClosingCost.</param>
        /// <param name="appendDescription">Description to add to the top of the current template description.</param>
        /// <param name="fieldsAndValues">Dictionary of fields and their values to update.</param>
        public TemplateUpdater(TemplateSettingsType templateSettingsType, string appendDescription, Dictionary<string, string> fieldsAndValues)
        {
            this._templateSettingsType = templateSettingsType;
            this._appendDescription = appendDescription;
            this._fieldsAndValues = fieldsAndValues;
        }

        /// <summary>
        /// Update templates with the initialized data.
        /// </summary>
        public void UpdateTemplates()
        {
            var fileSystemEntry = FileSystemEntry.Parse("Public:\\");
            var templates = ConfigManager.ConfigurationManager.GetTemplateDirEntries(this._templateSettingsType, fileSystemEntry);
            this.UpdateTemplateFields(templates);
        }

        private void UpdateTemplateFields(FileSystemEntry[] fileSystemEntries)
        {
            foreach (FileSystemEntry entry in fileSystemEntries)
            {
                if (entry.Type == FileSystemEntry.Types.Folder)
                {
                    var subFileSystemEntry = FileSystemEntry.Parse("Public:\\" + entry.Path);
                    var subTemplates = ConfigManager.ConfigurationManager.GetTemplateDirEntries(this._templateSettingsType, subFileSystemEntry);
                    this.UpdateTemplateFields(subTemplates);
                }
                else
                {
                    var thisTemplate = ConfigManager.ConfigurationManager.GetTemplateSettings(this._templateSettingsType, entry);
                    if (this._templateSettingsType == TemplateSettingsType.MiscData)
                    {
                        if (this.TemplateFilter == null || this.TemplateFilter.TemplateInFilter(entry, (DataTemplate)thisTemplate))
                            this.WriteToTemplate((DataTemplate)thisTemplate, entry); 
                    }
                    else if (this._templateSettingsType == TemplateSettingsType.ClosingCost)
                    {
                        if (this.TemplateFilter == null || this.TemplateFilter.TemplateInFilter(entry, (ClosingCost)thisTemplate))
                            this.WriteToTemplate((ClosingCost)thisTemplate, entry);
                    }
                    else if (this._templateSettingsType == TemplateSettingsType.LoanProgram)
                    {
                        if (this.TemplateFilter == null || this.TemplateFilter.TemplateInFilter(entry, (LoanProgram)thisTemplate))
                            this.WriteToTemplate((LoanProgram)thisTemplate, entry);
                    }
                    // ToDo: consider implementing this later. Need to track/grab alternate input data.
                    //else if (this.applyAlternate)
                    //{
                    //    this.WriteToTemplate(thisDataTemplate, entry);
                    //}
                }
            }
        }

        private void WriteToTemplate(FieldDataTemplate dataTemplate, FileSystemEntry fileSystemEntry)
        {
            var oldDescription = dataTemplate.Description;
            if (!string.IsNullOrEmpty(this._appendDescription))
                dataTemplate.Description = this._appendDescription + Environment.NewLine + oldDescription;

            foreach (var kvp in this._fieldsAndValues)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;

                try
                {
                    dataTemplate.SetField(kvp.Key, kvp.Value ?? string.Empty);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"FieldDataTemplate Error writing field: {kvp.Key}; value: {kvp.Value}; to {dataTemplate.TemplateName}. Exception: {ex}");
                }
            }

            ConfigManager.ConfigurationManager.SaveTemplateSettings(_templateSettingsType, fileSystemEntry, dataTemplate);
            Log.Logger.Verbose($"FieldDataTemplate {dataTemplate.TemplateName} updated.");
        }
    }
}
