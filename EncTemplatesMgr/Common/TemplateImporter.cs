using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EncTemplatesMgr.Helpers;
using EncTemplatesMgr.Interfaces;
using EncTemplatesMgr.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace EncTemplatesMgr.Common
{
    internal class TemplateImporter
    {
        /// <summary>
        /// Type of template being imported.
        /// </summary>
        private TemplateSettingsType _templateSettingsType;

        /// <summary>
        /// Only import templates that pass the filter.
        /// </summary>
        public Filter TemplateFilter { get; set; }

        /// <summary>
        /// Import templates of type TemplateSettingsType.
        /// </summary>
        /// <param name="templateSettingsType">Type of template(s) being imported.</param>
        public TemplateImporter(TemplateSettingsType templateSettingsType)
        {
            this._templateSettingsType = templateSettingsType;
        }

        /// <summary>
        /// Import templates of type specified in constructor.
        /// </summary>
        /// <param name="filePath">Full path of the json file containing templates to import.</param>
        public void ImportTemplates(string filePath)
        {
            try
            {
                var jsonString = File.ReadAllText(filePath);
                if (this._templateSettingsType == TemplateSettingsType.MiscData)
                {
                    var importTemplates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomDataTemplate>>(jsonString);
                    foreach (var customTemplate in importTemplates)
                    {
                        this.ImportCustomTemplate(customTemplate);
                    }
                }
                else if (this._templateSettingsType == TemplateSettingsType.ClosingCost) 
                {
                    var importTemplates = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomClosingCostTemplate>>(jsonString);
                    foreach (var customTemplate in importTemplates)
                    {
                        this.ImportCustomTemplate(customTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("TemplateImport", ex);
            }
        }

        private void ImportCustomTemplate(ICustomTemplate customTemplate)
        {
            if (this.TemplateFilter != null && !this.TemplateFilter.TemplateInFilter(customTemplate))
                return;

            var fileSystemEntryName = FileSystemEntry.AddRoot(customTemplate.FilePath.Replace(@"\\", @"\"));
            var fileSystemEntry = FileSystemEntry.Parse(fileSystemEntryName);
            ConfigManager.ConfigurationManager.CreateTemplateSettingsFolder(this._templateSettingsType, fileSystemEntry);
            FieldDataTemplate newTemplate = null;

            if (this._templateSettingsType == TemplateSettingsType.MiscData)
                newTemplate = ((CustomDataTemplate)customTemplate).ConvertToDataTemplate();

            if (this._templateSettingsType == TemplateSettingsType.ClosingCost)
                newTemplate = ((CustomClosingCostTemplate)customTemplate).ConvertToClosingCostTemplate();

            var newFileSystemEntry = FileSystemEntry.Parse(fileSystemEntryName + "\\" + newTemplate.TemplateName);
            ConfigManager.ConfigurationManager.SaveTemplateSettings(this._templateSettingsType, newFileSystemEntry, newTemplate);
        }
    }
}
