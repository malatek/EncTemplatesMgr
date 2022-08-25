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
    internal class TemplateExporter
    {
        /// <summary>
        /// List of CustomTemplates that will be exported.
        /// </summary>
        private List<ICustomTemplate> _customTemplateList = new List<ICustomTemplate>();

        /// <summary>
        /// TemplateSettingsType to export.
        /// </summary>
        private TemplateSettingsType _templateSettingsType;

        /// <summary>
        /// Filter object to filter templates to be exported.
        /// </summary>
        public Filter TemplateFilter { get; set; }

        /// <summary>
        /// Export templates of type TemplateSettingsType.
        /// </summary>
        /// <param name="templateSettingsType"></param>
        public TemplateExporter(TemplateSettingsType templateSettingsType)
        {
            this._templateSettingsType = templateSettingsType;
        }

        /// <summary>
        /// Export all templates to json file.
        /// </summary>
        /// <param name="filePath">Full file path (including file name) to export to.</param>
        public void ExportTemplates(string filePath)
        {
            try
            {
                var fileSystemEntry = FileSystemEntry.Parse("Public:\\");
                var dataTemplates = ConfigManager.ConfigurationManager.GetTemplateDirEntries(this._templateSettingsType, fileSystemEntry);
                this.ExtractTemplateData(dataTemplates);

                if (this._customTemplateList.Count < 1)
                    return;

                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(this._customTemplateList);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("TemplateExport", ex);
            }
        }

        private void ExtractTemplateData(FileSystemEntry[] templates)
        {
            foreach (var template in templates)
            {
                if (template.Type == FileSystemEntry.Types.Folder)
                {
                    var subFileSystemEntry = FileSystemEntry.Parse("Public:\\" + template.Path);
                    var subDataTemplates = ConfigManager.ConfigurationManager.GetTemplateDirEntries(this._templateSettingsType, subFileSystemEntry);
                    this.ExtractTemplateData(subDataTemplates);
                }
                else
                {
                    var thisTemplate = ConfigManager.ConfigurationManager.GetTemplateSettings(this._templateSettingsType, template);
                    ICustomTemplate customTemplate = null;

                    if (this._templateSettingsType == TemplateSettingsType.MiscData)
                    {
                        if (this.TemplateFilter != null && !this.TemplateFilter.TemplateInFilter(template, (DataTemplate)thisTemplate))
                            continue;

                        customTemplate = (CustomDataTemplate)(DataTemplate)thisTemplate;
                        if (template.Path.EndsWith(template.Name))
                            customTemplate.FilePath = template.Path.Remove(template.Path.Length - template.Name.Length);
                        else
                            customTemplate.FilePath = template.Path;
                    }
                    else if (this._templateSettingsType == TemplateSettingsType.ClosingCost)
                    {
                        if (this.TemplateFilter != null && !this.TemplateFilter.TemplateInFilter(template, (ClosingCost)thisTemplate))
                            continue;

                        customTemplate = (CustomClosingCostTemplate)(ClosingCost)thisTemplate;
                        if (template.Path.EndsWith(template.Name))
                            customTemplate.FilePath = template.Path.Remove(template.Path.Length - template.Name.Length);
                        else
                            customTemplate.FilePath = template.Path;
                    }

                    if (customTemplate != null)
                        this._customTemplateList.Add(customTemplate);
                }
            }
        }
    }
}
