using Eugene.Core;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using Eugene.Desktop.Dialogs;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eugene.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private BusinessValueMap _businessValue = new BusinessValueMap();
        public BusinessValueMap BusinessValue
        {
            get { return _businessValue; }
            set
            {
                SetProperty(ref _businessValue, value);

                //if (Dataset != null)
                //{
                //    Dataset.BusinessValue = _businessValue;
                //    DatasetAsTable = ConvertDatasetToTable(Dataset);
                //}

                //if (ResolvedDataset != null)
                //{
                //    ResolvedDataset.BusinessValue = _businessValue;
                //    ResolvedDatasetAsTable = ConvertDatasetToTable(ResolvedDataset);
                //}

                //RaisePropertyChanged(nameof(Dataset));
                //RaisePropertyChanged(nameof(DatasetAsTable));
                //RaisePropertyChanged(nameof(ResolvedDataset));
                //RaisePropertyChanged(nameof(ResolvedDatasetAsTable));
            }
        }

        private TestcaseBlockerDataset _dataset = null;
        public TestcaseBlockerDataset Dataset
        {
            get { return _dataset; }
            set
            {
                SetProperty(ref _dataset, value);
                _dataset.BusinessValue = BusinessValue ?? new BusinessValueMap();

                DatasetAsTable = ConvertDatasetToTable(_dataset);

                SetStatusDataset();
            }
        }

        private TestcaseBlockerDataset _resolvedDataset = null;
        public TestcaseBlockerDataset ResolvedDataset
        {
            get { return _resolvedDataset; }
            set
            {
                SetProperty(ref _resolvedDataset, value);
                _resolvedDataset.BusinessValue = BusinessValue ?? new BusinessValueMap();

                ResolvedDatasetAsTable = ConvertDatasetToTable(_resolvedDataset);
            }
        }

        private OptimizationResult _optimizationResult = null;
        public OptimizationResult OptimizationResult
        {
            get { return _optimizationResult; }
            set { SetProperty(ref _optimizationResult, value); }
        }

        private Optimizer _optimizer;
        public Optimizer Optimizer
        {
            get { return _optimizer; }
            set { _optimizer = value; }
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        private DataView _datasetAsTable;
        public DataView DatasetAsTable
        {
            get { return _datasetAsTable; }
            set { SetProperty(ref _datasetAsTable, value); }
        }

        private DataView _resolvedDatasetAsTable;
        public DataView ResolvedDatasetAsTable
        {
            get { return _resolvedDatasetAsTable; }
            set { SetProperty(ref _resolvedDatasetAsTable, value); }
        }

        private BlockerResolverResult _resolution;
        public BlockerResolverResult Resolution
        {
            get { return _resolution; }
            set { SetProperty(ref _resolution, value); }
        }

        private ObservableCollection<Blocker> _selectedBlockers = new ObservableCollection<Blocker>();
        public ObservableCollection<Blocker> SelectedBlockers
        {
            get { return _selectedBlockers; }
            set { SetProperty(ref _selectedBlockers, value); }
        }

        public MainWindowViewModel()
        {
            BusinessValue.TestTypes.Add("Regression", 0.5);
            BusinessValue.TestTypes.Add("Level 2", 2);

            #region Initialize Commands
            ExitApplicationCommand = new DelegateCommand(ExitApplication, CanExitApplication);

            LoadDatasetCommand = new DelegateCommand(LoadDataset, CanLoadDataset)
                                                .ObservesProperty(() => IsRunning);

            SaveDatasetCommand = new DelegateCommand(SaveDataset, CanSaveDataset)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => Dataset);

            ImportDatasetCommand = new DelegateCommand(ImportDataset, CanImportDataset)
                                                .ObservesProperty(() => IsRunning);

            ExportDatasetCommand = new DelegateCommand(ExportDataset, CanExportDataset)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => Dataset);

            GenerateDatasetCommand = new DelegateCommand(GenerateDataset, CanGenerateDataset)
                                                .ObservesProperty(() => IsRunning);

            StartOptimizationCommand = new DelegateCommand(StartOptimization, CanStartOptimization)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => Dataset);

            SaveOptimizationResultCommand = new DelegateCommand(SaveOptimizationResult, CanSaveOptimizationResult)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => OptimizationResult);

            ExportResolvedDatasetCommand = new DelegateCommand(ExportResolvedDataset, CanExportResolvedDataset)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => ResolvedDataset);

            ResolveSelectedBlockersCommand = new DelegateCommand(ResolveSelectedBlockers, CanResolveSelectedBlockers)
                                                .ObservesProperty(() => IsRunning)
                                                .ObservesProperty(() => Dataset)
                                                .ObservesProperty(() => SelectedBlockers);

            NotifyBlockersSelectedCommand = new DelegateCommand(NotifyBlockersSelected, CanNotifyBlockersSelected);

            ApplyBusinessValueCommand = new DelegateCommand(ApplyBusinessValue, CanApplyBusinessValue);
            #endregion
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
            if (IsRunning)
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
            if (IsRunning)
                return false;

            if (Dataset == null)
                return false;

            return true;
        }

        public DelegateCommand ImportDatasetCommand { get; private set; }
        private void ImportDataset()
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
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {

                var optionsDlg = new ExcelImportDialog();
                if(optionsDlg.ShowDialog() == true)
                {
                    options = optionsDlg.ImportOptions;
                    if(options != null)
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
                    else
                    {
                        SetStatusMessage("Invalid import options given.");
                    }
                }
            }
        }
        private bool CanImportDataset()
        {
            if (IsRunning)
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
            if (IsRunning)
                return false;

            if (Dataset == null)
                return false;

            return true;
        }

        public DelegateCommand GenerateDatasetCommand { get; private set; }
        private void GenerateDataset()
        {
            Dataset = DataGenerator.GenerateRandomData(20, 50);
        }
        private bool CanGenerateDataset()
        {
            if (IsRunning)
                return false;

            return true;
        }

        public DelegateCommand SaveOptimizationResultCommand { get; private set; }
        private async void SaveOptimizationResult()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "json",
                Filter = "Json (*.json)|*.json"
            };

            if (dlg.ShowDialog() == true)
            {
                await IOOperations.SaveAsync<OptimizationResult>(dlg.FileName, OptimizationResult);
                SetStatusMessage("Result saved.");
            }
        }
        private bool CanSaveOptimizationResult()
        {
            if (IsRunning)
                return false;

            if (OptimizationResult == null)
                return false;

            return true;
        }

        public DelegateCommand ExportResolvedDatasetCommand { get; private set; }
        private async void ExportResolvedDataset()
        {
            var dlg = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            if (dlg.ShowDialog() == true)
            {
                await ResolvedDataset.ExportAsXlsxAsync(dlg.FileName);
                SetStatusMessage("Dataset saved.");
            }
        }
        private bool CanExportResolvedDataset()
        {
            if (IsRunning)
                return false;

            if (ResolvedDataset == null)
                return false;

            return true;
        }

        public DelegateCommand StartOptimizationCommand { get; private set; }
        private async void StartOptimization()
        {
            IsRunning = true;

            var watch = new Stopwatch();
            watch.Start();

            await Task.Run(() =>
            {
                Optimizer = new Optimizer(Dataset);
                OptimizationResult = Optimizer.Optimize();
            });

            var duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            watch.Stop();
            IsRunning = false;

            SelectedBlockers.Clear();
            SelectedBlockers.AddRange(OptimizationResult.ResolvedBlockers);

            SetStatusMessage($"Finished in {duration}.");

            ResolveSelectedBlockersCommand.Execute();
        }
        private bool CanStartOptimization()
        {
            if (IsRunning)
                return false;

            if (Dataset == null)
                return false;

            return true;
        }

        public DelegateCommand ResolveSelectedBlockersCommand { get; private set; }
        private async void ResolveSelectedBlockers()
        {
            IsRunning = true;
            await Task.Run(() =>
            {
                var resolver = new BlockerResolver(Dataset);
                //Resolution = resolver.GetResolution(SelectedBlockers.Select(x => x.Id).ToList());
                Resolution = resolver.Resolve(SelectedBlockers.ToList());
                ResolvedDataset = Resolution.ResolvedDataset;
            });
            IsRunning = false;

            //SetStatusMessage($"{SelectedBlockers.Count} Blockers resolved.");
        }
        private bool CanResolveSelectedBlockers()
        {
            if (IsRunning)
                return false;

            if (Dataset == null)
                return false;

            //if (SelectedBlockers.Count <= 0)
            //    return false;

            return true;
        }

        public DelegateCommand NotifyBlockersSelectedCommand { get; private set; }
        private void NotifyBlockersSelected()
        {
            ResolveSelectedBlockersCommand.RaiseCanExecuteChanged();
        }
        private bool CanNotifyBlockersSelected()
        {
            return true;
        }

        public DelegateCommand ApplyBusinessValueCommand { get; private set; }
        private void ApplyBusinessValue()
        {
            if (Dataset != null)
            {
                Dataset.BusinessValue = BusinessValue;
                DatasetAsTable = ConvertDatasetToTable(Dataset);
                RaisePropertyChanged(nameof(Dataset));
            }

            if (ResolvedDataset != null)
            {
                ResolvedDataset.BusinessValue = BusinessValue;
                ResolvedDatasetAsTable = ConvertDatasetToTable(ResolvedDataset);
                RaisePropertyChanged(nameof(ResolvedDataset));
            }
        }
        private bool CanApplyBusinessValue()
        {
            return true;
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
        private DataView ConvertDatasetToTable(TestcaseBlockerDataset dataset)
        {
            if (dataset == null)
                return null;

            var table = new DataTable();

            // Spalten
            table.Columns.Add(new DataColumn("#", typeof(int)));
            table.Columns.Add(new DataColumn("Testcase Id", typeof(string)));
            table.Columns.Add(new DataColumn("Testcase Name", typeof(string)));
            table.Columns.Add(new DataColumn("Test Type", typeof(string)));
            table.Columns.Add(new DataColumn("App. Module", typeof(string)));
            table.Columns.Add(new DataColumn("Value", typeof(string)));


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
                newRow[3] = testcase.TestType;
                newRow[4] = testcase.ApplicationModule;
                newRow[5] = dataset.GetValueForTestcases(new List<Testcase> { testcase });

                for (int i = 0; i < dataset.Blockers.Count; i++)
                {
                    var blockerId = dataset.Blockers[i].Id;
                    if(testcase.BlockerIds.Contains(blockerId))
                    {
                        newRow[i + 6] = "x";
                        sumOfBlockedTestcases[blockerId]++;
                    }
                }

                newRow[dataset.Blockers.Count + 6] = testcase.BlockerIds.Count;

                table.Rows.Add(newRow);
            }

            // Ergebniszeile
            var resultRow = table.NewRow();
            for(int i = 0; i < dataset.Blockers.Count; i++)
            {
                var blockerId = dataset.Blockers[i].Id;
                var blocked = sumOfBlockedTestcases[blockerId];
                resultRow[i + 6] = blocked;
            }
            table.Rows.Add(resultRow);


            return table.DefaultView;
        }
    }
}
