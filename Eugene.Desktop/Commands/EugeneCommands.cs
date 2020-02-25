using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Eugene.Desktop.Commands
{
    public static class EugeneCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "ExitCommand",
            typeof(EugeneCommands),
            new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) });

        public static readonly RoutedUICommand LoadDataset = new RoutedUICommand(
            "Load Dataset",
            "LoadDatasetCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand SaveDataset = new RoutedUICommand(
            "Save Dataset",
            "SaveDatasetCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand ImportDataset = new RoutedUICommand(
            "Import Dataset",
            "ImportDatasetCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand ExportDataset = new RoutedUICommand(
            "Export Dataset",
            "ExportDatasetCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand GenerateDataset = new RoutedUICommand(
            "Generate Dataset",
            "GenerateDatasetCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand StartOptimization = new RoutedUICommand(
            "Start Optimization",
            "StartOptimizationCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand SaveResult = new RoutedUICommand(
            "Save Result",
            "SaveResultCommand",
            typeof(EugeneCommands));

        public static readonly RoutedUICommand ExportResult = new RoutedUICommand(
            "Export Result",
            "ExportResultCommand",
            typeof(EugeneCommands));
    }
}
