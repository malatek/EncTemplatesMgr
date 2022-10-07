using EllieMae.EMLite.ClientServer;
using EncTemplatesMgr.Common;
using EncTemplatesMgr.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        /// What to show the user when there isn't a template type selected.
        /// </summary>
        private readonly string _msgTypeNotSelected = "A Template Type must be selected. No action has been taken.";

        /// <summary>
        /// What to show the user when run is finished.
        /// </summary>
        private readonly string _msgFinished = "Operation Complete.";

        public MainWindow()
        {
            InitializeComponent();
            //this.templateType.DataContext = new List<TemplateSettingsType?>() { null, TemplateSettingsType.LoanProgram, TemplateSettingsType.MiscData, TemplateSettingsType.ClosingCost };
            this.PopulateTemplateTypeCombobox();
            this.fieldsAndValuesGrid.DataContext = _fieldData;
            this.filterFieldsAndValuesGrid.DataContext = _filterFieldData;
        }

        private void PopulateTemplateTypeCombobox()
        {
            this.templateType.DisplayMemberPath = "Key";
            this.templateType.SelectedValuePath = "Value";
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Loan Program", TemplateSettingsType.LoanProgram));
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Data Template", TemplateSettingsType.MiscData));
            this.templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Closing Cost", TemplateSettingsType.ClosingCost));
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
            this.ResetUIData();
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
            if (string.IsNullOrEmpty(importPath))
            {
                MessageBox.Show("Import path must be specified.");
                return;
            }

            var templateImport = new TemplateImporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = filter,
                OverwriteExisting = (bool)this.OverwriteExisting.IsChecked
            };

            templateImport.ImportTemplates(importPath);
            this.ResetUIData();
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
            this.ResetUIData();
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
                path = System.IO.Path.GetTempPath() + "export.json";
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
