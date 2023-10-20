using EllieMae.EMLite.ClientServer;
using EncTemplatesMgr.Common;
using EncTemplatesMgr.ViewModel;
using EncTemplatesMgr.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            StopProgressBar();
            PopulateTemplateTypeCombobox();

            lblStatus.Visibility = Visibility.Hidden;
            fieldsAndValuesGrid.DataContext = _fieldData;
            filterFieldsAndValuesGrid.DataContext = _filterFieldData;
            exportFilePath.Text = _defaultFilePath;
        }

        private void HyperlinkOpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            var userGuidePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "EncTemplatesMgr");
            var userGuideFile = System.IO.Path.Combine(userGuidePath, "UserGuide.htm");
            if (!File.Exists(userGuideFile))
            {
                Directory.CreateDirectory(userGuidePath);
                File.WriteAllText(userGuideFile, Properties.Resources.UserGuide);
            }

            Process.Start(userGuideFile);
        }

        private void ChkboxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var isEnabled = true;
            if ((bool)selectAllTemplates.IsChecked)
            {
                isEnabled = false;
            }
            else
            {
                isEnabled = true;
            }

            templateNameContains.IsEnabled = isEnabled;
            templateNameMustMatch.IsEnabled = isEnabled;
            filePathContains.IsEnabled = isEnabled;
            filePathMustMatch.IsEnabled = isEnabled;
            fieldValuesMustMatch.IsEnabled = isEnabled;
            allFieldValuesMustMatch.IsEnabled = isEnabled;
            filterFieldsAndValuesGrid.IsEnabled = isEnabled;
        }

        private void ExportFilePath_GotFocus(object sender, RoutedEventArgs e)
        {
            if (exportFilePath.Text == _defaultFilePath)
                exportFilePath.Text = string.Empty;
        }

        private void ButtonFilePicker_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
                exportFilePath.Text = openFileDialog.FileName;
        }

        private async void ButtonExportTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(templateType.Text))
                return;

            var filter = new Filter()
            {
                IncludeAllTemplates = (bool)selectAllTemplates.IsChecked,
                FilterFilePath = filePathContains.Text,
                FilePathRequired = (bool)filePathMustMatch.IsChecked,
                FilterTemplateName = templateNameContains.Text,
                TemplateNameRequired = (bool)templateNameMustMatch.IsChecked,
                FilterFieldValues = TypeConverters.FieldDataCollectionToDictionary(_filterFieldData),
                FieldValuesRequired = (bool)fieldValuesMustMatch.IsChecked,
                AllFieldValuesRequired = (bool)allFieldValuesMustMatch.IsChecked
            };

            var exportPath = CheckFilePath(exportFilePath.Text);
            var templateExport = new TemplateExporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = filter
            };

            StartProgressBar();
            await Task.Run(() => templateExport.ExportTemplates(exportPath));
            StopProgressBar();
        }

        private async void ButtonImportTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(templateType.Text))
                return;

            var filter = new Filter()
            {
                IncludeAllTemplates = (bool)selectAllTemplates.IsChecked,
                FilterFilePath = filePathContains.Text,
                FilePathRequired = (bool)filePathMustMatch.IsChecked,
                FilterTemplateName = templateNameContains.Text,
                TemplateNameRequired = (bool)templateNameMustMatch.IsChecked,
                FilterFieldValues = TypeConverters.FieldDataCollectionToDictionary(_filterFieldData),
                FieldValuesRequired = (bool)fieldValuesMustMatch.IsChecked,
                AllFieldValuesRequired = (bool)allFieldValuesMustMatch.IsChecked
            };

            var importPath = exportFilePath.Text;
            if (string.IsNullOrEmpty(importPath) || !File.Exists(importPath))
            {
                MessageBox.Show("Import path is not valid.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var templateImport = new TemplateImporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = filter,
                OverwriteExisting = (bool)OverwriteExisting.IsChecked
            };

            StartProgressBar();
            await Task.Run(() => templateImport.ImportTemplates(importPath));
            StopProgressBar();
        }

        private async void ButtonUpdateTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(templateType.Text))
                return;

            var filter = new Filter()
            {
                IncludeAllTemplates = (bool)selectAllTemplates.IsChecked,
                FilterFilePath = filePathContains.Text,
                FilePathRequired = (bool)filePathMustMatch.IsChecked,
                FilterTemplateName = templateNameContains.Text,
                TemplateNameRequired = (bool)templateNameMustMatch.IsChecked,
                FilterFieldValues = TypeConverters.FieldDataCollectionToDictionary(_filterFieldData),
                FieldValuesRequired = (bool)fieldValuesMustMatch.IsChecked,
                AllFieldValuesRequired = (bool)allFieldValuesMustMatch.IsChecked
            };

            var templateUpdate = new TemplateUpdater(
                (TemplateSettingsType)templateType.SelectedValue,
                appendDescription.Text,
                TypeConverters.FieldDataCollectionToDictionary(_fieldData))
            { TemplateFilter = filter };

            StartProgressBar();
            await Task.Run(() => templateUpdate.UpdateTemplates());
            StopProgressBar();
        }

        private void PopulateTemplateTypeCombobox()
        {
            templateType.DisplayMemberPath = "Key";
            templateType.SelectedValuePath = "Value";
            templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Loan Program", TemplateSettingsType.LoanProgram));
            templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Data Template", TemplateSettingsType.MiscData));
            templateType.Items.Add(new KeyValuePair<string, TemplateSettingsType?>("Closing Cost", TemplateSettingsType.ClosingCost));
        }

        private void StopProgressBar()
        {
            lblStatus.Content = "Operation Complete";
            pbStatus.Visibility = Visibility.Hidden;
            this.IsEnabled = true;
        }

        private void StartProgressBar()
        {
            this.IsEnabled = false;
            lblStatus.Content = "In Progress... Please Wait";
            lblStatus.Visibility = Visibility.Visible;
            pbStatus.Visibility = Visibility.Visible;
        }

        private string CheckFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = _defaultFilePath;
                exportFilePath.Text = path;
            }

            if (!path.EndsWith(".json"))
                path += ".json";

            return path;
        }
    }
}
