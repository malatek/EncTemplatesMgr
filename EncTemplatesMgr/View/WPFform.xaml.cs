using EllieMae.EMLite.ClientServer;
using EncTemplatesMgr.Common;
using EncTemplatesMgr.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncTemplatesMgr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Fields to update and data to update them with.
        /// </summary>
        private ObservableCollection<FieldData> _fieldData = new ObservableCollection<FieldData>(new List<FieldData>());

        /// <summary>
        /// Only update the template if it has the given fields with the given data.
        /// </summary>
        private ObservableCollection<FieldData> _filterFieldData = new ObservableCollection<FieldData>(new List<FieldData>());

        /// <summary>
        /// Default import/export path.
        /// </summary>
        private readonly string _defaultFilePath = "C:\\temp\\export.json";

        public MainWindow()
        {
            InitializeComponent();
            this.PopulateTemplateTypeCombobox();
            this.fieldsAndValuesGrid.DataContext = _fieldData;
            this.filterFieldsAndValuesGrid.DataContext = _filterFieldData;
            this.exportFilePath.Text = this._defaultFilePath;
        }

        private void PopulateTemplateTypeCombobox()
        {
            this.templateType.DisplayMemberPath = "Key";
            this.templateType.SelectedValuePath = "Value";
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Loan Program", TemplateSettingsType.LoanProgram));
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Data Template", TemplateSettingsType.MiscData));
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Closing Cost", TemplateSettingsType.ClosingCost));
        }

        private void HyperlinkOpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            var tempFolder = System.IO.Path.GetTempPath();
            var userGuidePath = System.IO.Path.Combine(tempFolder, "EncTemplatesMgr");
            var userGuideFile = System.IO.Path.Combine(userGuidePath, "UserGuide.htm");
            if (!File.Exists(userGuideFile))
            {
                Directory.CreateDirectory(userGuidePath);
                File.WriteAllText(userGuideFile, Properties.Resources.UserGuide);
            }

            Process.Start(userGuideFile);
        }

        private void ExportFilePath_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.exportFilePath.Text == this._defaultFilePath)
                this.exportFilePath.Text = string.Empty;
        }

        private void ButtonFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
                this.exportFilePath.Text = openFileDialog.FileName;
        }

        private void ButtonExportTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.templateType.Text))
                return;

            var filter = new Filter()
            {
                FilterFilePath = this.filePathContains.Text,
                FilterTemplateName = this.templateNameContains.Text,
                FilterFieldValues = this.FieldDataCollectionToDictionary(this._filterFieldData)
            };

            var exportPath = this.CheckFilePath(this.exportFilePath.Text);
            var templateExport = new TemplateExporter((TemplateSettingsType)this.templateType.SelectedValue)
            {
                TemplateFilter = filter
            };

            templateExport.ExportTemplates(exportPath);
            MessageBox.Show("Template export complete.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonImportTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.templateType.Text))
                return;

            var filter = new Filter()
            {
                FilterFilePath = this.filePathContains.Text,
                FilterTemplateName = this.templateNameContains.Text,
                FilterFieldValues = this.FieldDataCollectionToDictionary(this._filterFieldData)
            };

            var importPath = this.exportFilePath.Text;
            if (string.IsNullOrEmpty(importPath) || !File.Exists(importPath))
            {
                MessageBox.Show("Import path is not valid.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var templateImport = new TemplateImporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = filter,
                OverwriteExisting = (bool)this.OverwriteExisting.IsChecked
            };

            templateImport.ImportTemplates(importPath);
            MessageBox.Show("Template import complete.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonUpdateTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.templateType.Text))
                return;

            var filter = new Filter()
            {
                FilterFilePath = this.filePathContains.Text,
                FilterTemplateName = this.templateNameContains.Text,
                FilterFieldValues = this.FieldDataCollectionToDictionary(this._filterFieldData)
            };

            var templateUpdate = new TemplateUpdater(
                (TemplateSettingsType)this.templateType.SelectedValue,
                this.appendDescription.Text,
                this.FieldDataCollectionToDictionary(this._fieldData))
            { TemplateFilter = filter };

            templateUpdate.UpdateTemplates();
            MessageBox.Show("Template updates complete.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Dictionary<string, string> FieldDataCollectionToDictionary(ObservableCollection<FieldData> fieldDataCollection)
        {
            var dictionary = new Dictionary<string, string>();

            if (fieldDataCollection == null)
                return dictionary;

            foreach (var item in fieldDataCollection)
            {
                if(string.IsNullOrEmpty(item.FieldId))
                    continue;

                dictionary.Add(item.FieldId, item.FieldValue);
            }
            
            return dictionary;
        }

        private string CheckFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = _defaultFilePath;
                this.exportFilePath.Text = path;
            }

            return path;
        }

        private void ResetUIData()
        {
            this.filePathContains.Text = string.Empty;
            this.templateNameContains.Text = string.Empty;
            this.appendDescription.Text = string.Empty;
            this.exportFilePath.Text = string.Empty;
            this.OverwriteExisting.IsChecked = false;
        }

        private void filterFieldsAndValuesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void fieldsAndValuesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
