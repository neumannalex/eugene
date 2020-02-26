using Eugene.Core;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eugene.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private TestcaseBlockerDataset _dataset = null;
        public TestcaseBlockerDataset Dataset
        {
            get { return _dataset; }
            set
            {
                SetProperty(ref _dataset, value);

                DatasetAsTable = ConvertDatasetToTable(_dataset);

                SetStatusDataset();
            }
        }

        private OptimizationResult _result = null;
        public OptimizationResult Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
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
            get { return _isOptimizing; }
            set { SetProperty(ref _isOptimizing, value); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        private DataTable _datasetAsTable;
        public DataTable DatasetAsTable
        {
            get { return _datasetAsTable; }
            set { SetProperty(ref _datasetAsTable, value); }
        }

        private ObservableCollection<Blocker> _selectedBlockers = new ObservableCollection<Blocker>();
        public ObservableCollection<Blocker> SelectedBlockers
        {
            get { return _selectedBlockers; }
            set { SetProperty(ref _selectedBlockers, value); }
        }


        public MainWindowViewModel()
        {
            ExitApplicationCommand = new DelegateCommand(ExitApplication, CanExitApplication);

            LoadDatasetCommand = new DelegateCommand(LoadDataset, CanLoadDataset)
                                                .ObservesProperty(() => IsOptimizing);

            SaveDatasetCommand = new DelegateCommand(SaveDataset, CanSaveDataset)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Dataset);

            ImportDatasetCommand = new DelegateCommand(ImportDataset, CanImportDataset)
                                                .ObservesProperty(() => IsOptimizing);

            ExportDatasetCommand = new DelegateCommand(ExportDataset, CanExportDataset)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Dataset);

            GenerateDatasetCommand = new DelegateCommand(GenerateDataset, CanGenerateDataset)
                                                .ObservesProperty(() => IsOptimizing);

            StartOptimizationCommand = new DelegateCommand(StartOptimization, CanStartOptimization)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Dataset);

            SaveResultCommand = new DelegateCommand(SaveResult, CanSaveResult)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Result);

            ExportResultCommand = new DelegateCommand(ExportResult, CanExportResult)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Result);

        }

        #region Commands
        public DelegateCommand ExitApplicationCommand { get; private set; }
        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
        private bool CanExitApplication()
        {
            return true;
        }

        public DelegateCommand LoadDatasetCommand { get; private set; }
        private async void LoadDataset()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Json (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
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
        private bool CanLoadDataset()
        {
            if (IsOptimizing)
                return false;

            return true;
        }

        public DelegateCommand SaveDatasetCommand { get; private set; }
        private async void SaveDataset()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "json",
                Filter = "Json (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                await IOOperations.SaveAsync<TestcaseBlockerDataset>(dlg.FileName, Dataset);
                SetStatusMessage("Dataset saved.");
            }
        }
        private bool CanSaveDataset()
        {
            if (IsOptimizing)
                return false;

            if (Dataset == null)
                return false;

            return true;
        }

        public DelegateCommand ImportDatasetCommand { get; private set; }
        private async void ImportDataset()
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
        private bool CanImportDataset()
        {
            if (IsOptimizing)
                return false;

            return true;
        }

        public DelegateCommand ExportDatasetCommand { get; private set; }
        private async void ExportDataset()
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
        private bool CanExportDataset()
        {
            if (IsOptimizing)
                return false;

            if (Dataset == null)
                return false;

            return true;
        }

        public DelegateCommand GenerateDatasetCommand { get; private set; }
        private async void GenerateDataset()
        {
            Dataset = DataGenerator.GenerateRandomData(20, 50);
        }
        private bool CanGenerateDataset()
        {
            if (IsOptimizing)
                return false;

            return true;
        }

        public DelegateCommand SaveResultCommand { get; private set; }
        private async void SaveResult()
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
        private bool CanSaveResult()
        {
            if (IsOptimizing)
                return false;

            if (Result == null)
                return false;

            return true;
        }

        public DelegateCommand ExportResultCommand { get; private set; }
        private async void ExportResult()
        {
            MessageBox.Show("Not yet implemented");
        }
        private bool CanExportResult()
        {
            if (IsOptimizing)
                return false;

            if (Result == null)
                return false;

            return true;
        }


        public DelegateCommand StartOptimizationCommand { get; private set; }
        private async void StartOptimization()
        {
            IsOptimizing = true;

            var watch = new Stopwatch();
            watch.Start();

            await Task.Run(() =>
            {
                Optimizer = new Optimizer(Dataset);
                Result = Optimizer.Optimize();
            });

            var duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            watch.Stop();
            IsOptimizing = false;

            SelectedBlockers.Clear();
            SelectedBlockers.AddRange(Result.ResolvedBlockers);

            SetStatusMessage($"Finished in {duration}.");
        }
        private bool CanStartOptimization()
        {
            if (Dataset != null)
                return !IsOptimizing;

            return false;
        }
        #endregion

        private void SetStatusMessage(string message)
        {
            StatusMessage = message;
            //MessageBox.Show(message);
        }
        private void SetStatusDataset()
        {
            //lblStatusDataset.Text = Dataset != null ? "Dataset: ok" : "No Dataset";
        }
        private DataTable ConvertDatasetToTable(TestcaseBlockerDataset dataset)
        {
            if (dataset == null)
                return null;

            var table = new DataTable();

            // Spalten
            table.Columns.Add(new DataColumn("#", typeof(int)));
            table.Columns.Add(new DataColumn("Testcase Id", typeof(string)));
            table.Columns.Add(new DataColumn("Testcase Name", typeof(string)));


            Dictionary<string, int> sumOfBlockedTestcases = new Dictionary<string, int>();
            foreach (var blocker in dataset.Blockers)
            {
                table.Columns.Add(new DataColumn(blocker.Name, typeof(string)));
                sumOfBlockedTestcases.Add(blocker.Id, 0);
            }

            table.Columns.Add(new DataColumn("Number of Blockers", typeof(int)));

            // Zeilen
            
            for (var testcaseIndex = 0; testcaseIndex < dataset.Testcases.Count; testcaseIndex++)
            {
                var testcase = dataset.Testcases[testcaseIndex];
                var newRow = table.NewRow();
                newRow[0] = testcaseIndex + 1;
                newRow[1] = testcase.Id;
                newRow[2] = testcase.Name;

                for(int i = 0; i < dataset.Blockers.Count; i++)
                {
                    var blockerId = dataset.Blockers[i].Id;
                    if(testcase.BlockerIds.Contains(blockerId))
                    {
                        newRow[i + 3] = "x";
                        sumOfBlockedTestcases[blockerId]++;
                    }
                }

                newRow[dataset.Blockers.Count + 3] = testcase.BlockerIds.Count;

                table.Rows.Add(newRow);
            }

            // Ergebniszeile
            var resultRow = table.NewRow();
            for(int i = 0; i < dataset.Blockers.Count; i++)
            {
                var blockerId = dataset.Blockers[i].Id;
                var blocked = sumOfBlockedTestcases[blockerId];
                resultRow[i + 3] = blocked;
            }
            table.Rows.Add(resultRow);


            return table;
        }
    }
}
