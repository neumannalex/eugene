using Eugene.Core;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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


        public MainWindowViewModel()
        {
            StartOptimizationCommand = new DelegateCommand(StartOptimization, CanStartOptimization)
                                                .ObservesProperty(() => IsOptimizing)
                                                .ObservesProperty(() => Dataset);

            LoadDatasetCommand = new DelegateCommand(LoadDataset, CanLoadDataset);
        }

        public DelegateCommand LoadDatasetCommand { get; private set; }
        async void LoadDataset()
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
            return true;
        }

        public DelegateCommand StartOptimizationCommand { get; private set; }
        async void StartOptimization()
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

            SetStatusMessage($"Finished in {duration}.");
        }
        private bool CanStartOptimization()
        {
            if (Dataset != null)
                return !IsOptimizing;

            return false;
        }


        private void SetStatusMessage(string message)
        {
            StatusMessage = message;
            //MessageBox.Show(message);
        }
        private void SetStatusDataset()
        {
            //lblStatusDataset.Text = Dataset != null ? "Dataset: ok" : "No Dataset";
        }
    }
}
