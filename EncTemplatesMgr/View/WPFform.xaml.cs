using EllieMae.EMLite.ClientServer;
using EncTemplatesMgr.Common;
using EncTemplatesMgr.Helpers;
using EncTemplatesMgr.ViewModel;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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
        private ObservableCollection<FieldData> FieldData = new ObservableCollection<FieldData>(new List<FieldData>());

        /// <summary>
        /// Filter View Model
        /// </summary>
        private FilterData FilterData = new FilterData();

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
            fieldsAndValuesGrid.DataContext = FieldData;
            FilterStackPanel.DataContext = FilterData;
            exportFilePath.Text = _defaultFilePath;
        }

        private void HyperlinkOpenDocumentation_Click(object sender, RoutedEventArgs e)
        {
            var userGuidePath = Path.Combine(Path.GetTempPath(), "EncTemplatesMgr");
            var userGuideFile = Path.Combine(userGuidePath, "UserGuide.htm");
            if (!File.Exists(userGuideFile))
            {
                Directory.CreateDirectory(userGuidePath);
                File.WriteAllText(userGuideFile, Properties.Resources.UserGuide);
            }

            Process.Start(userGuideFile);
        }

        private void ChkboxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            templateNameContains.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            templateNameMustMatch.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            filePathContains.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            filePathMustMatch.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            fieldValuesMustMatch.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            allFieldValuesMustMatch.IsEnabled = !(bool)selectAllTemplates.IsChecked;
            filterFieldsAndValuesGrid.IsEnabled = !(bool)selectAllTemplates.IsChecked;
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

            var exportPath = CheckFilePath(exportFilePath.Text);
            var templateExport = new TemplateExporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = FilterData.ToFilter()
            };

            StartProgressBar();
            await Task.Run(() => templateExport.ExportTemplates(exportPath));
            StopProgressBar();
        }

        private async void ButtonImportTemplates_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(templateType.Text))
                return;

            var importPath = exportFilePath.Text;
            if (string.IsNullOrEmpty(importPath) || !File.Exists(importPath))
            {
                MessageBox.Show("Import path is not valid.", "Encompass Templates Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var templateImport = new TemplateImporter((TemplateSettingsType)templateType.SelectedValue)
            {
                TemplateFilter = FilterData.ToFilter(),
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

            var templateUpdate = new TemplateUpdater(
                (TemplateSettingsType)templateType.SelectedValue,
                appendDescription.Text,
                TypeConverters.FieldDataCollectionToDictionary(FieldData))
            { TemplateFilter = FilterData.ToFilter() };

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
