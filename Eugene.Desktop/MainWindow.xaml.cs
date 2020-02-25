using Eugene.Core;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Eugene.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TestcaseBlockerDataset _dataset = null;
        public TestcaseBlockerDataset Dataset
        {
            get { return _dataset; }
            set
            {
                _dataset = value;
                SetStatusDataset();
            }
        }

        private OptimizationResult _result = null;
        public OptimizationResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private Optimizer _optimizer;
        public Optimizer Optimizer
        {
            get { return _optimizer; }
            set { _optimizer = value; }
        }

        private bool _isOptimizing = false;
        public bool IsOptimizing
        {
            get { return _isOptimizing = false; }
            set
            {
                _isOptimizing = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LoadDatasetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Json (*.json)|*.json"
            };

            if(dlg.ShowDialog() == true)
            {
                try
                {
                    Dataset = await IOOperations.LoadAsync<TestcaseBlockerDataset>(dlg.FileName);
                    SetStatusMessage("Dataset loaded.");
                }
                catch
                {
                    Dataset = null;
                    SetStatusMessage("Loading dataset failed. Please check the data format.");
                }
            }
        }
        private void LoadDatasetCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void SaveDatasetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { 
                DefaultExt = "json",
                Filter = "Json (*.json)|*.json"
            };

            if(dlg.ShowDialog() == true)
            {
                await IOOperations.SaveAsync<TestcaseBlockerDataset>(dlg.FileName, Dataset);
                SetStatusMessage("Dataset saved.");
            }
        }
        private void SaveDatasetCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Dataset != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ImportDatasetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var options = new ExcelImportOptions
            {
                WorksheetName = "Rohdaten_19.02.2020",
                FirstDataRow = 2,
                ColumnNumberTestcaseId = 2,
                ColumnNumberTestcaseName = 4,
                ColumnNumberBlockerNames = 1,
                BlockerSeparator = ","
            };

            var dlg = new OpenFileDialog
            {
                Filter = "Json (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    Dataset = DataGenerator.ImportFromExcel(dlg.FileName, options);
                    SetStatusMessage("Dataset imported.");
                }
                catch
                {
                    Dataset = null;
                    SetStatusMessage("Importing dataset failed. Please check the data format and options.");
                }
            }
        }
        private void ImportDatasetCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void ExportDatasetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                await Dataset.ExportAsXlsxAsync(dlg.FileName);
                SetStatusMessage("Dataset saved.");
            }
        }
        private void ExportDatasetCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Dataset != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void GenerateDatasetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Dataset = DataGenerator.GenerateRandomData(20, 50);
        }
        private void GenerateDatasetCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void StartOptimizationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var watch = new Stopwatch();
            watch.Start();
            SetProgressbar(true);

            await Task.Run(() =>
            {
                Optimizer = new Optimizer(Dataset);
                Result = Optimizer.Optimize();
                
            });

            var duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            watch.Stop();
            SetProgressbar(false);

            SetStatusMessage($"Finished in {duration}.");
        }
        private void StartOptimizationCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Dataset != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private async void SaveResultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "json",
                Filter = "Json (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                await IOOperations.SaveAsync<OptimizationResult>(dlg.FileName, Result);
                SetStatusMessage("Result saved.");
            }
        }
        private void SaveResultCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Result != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void ExportResultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }
        private void ExportResultCommand_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Result != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }



        private void SetStatusMessage(string message)
        {
            lblStatusMessage.Text = message;
        }
        private void SetStatusDataset()
        {
            lblStatusDataset.Text = Dataset != null ? "Dataset: ok" : "No Dataset";
        }
        private void SetProgressbar(bool active)
        {
            pbProgress.IsIndeterminate = active;
        }
    }
}
