using Eugene.Core;
using Eugene.Core.GA;
using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Eugene
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var runApp = true;
            TestcaseBlockerDataset dataset = null;

            while (runApp)
            {
                PrintMenu();

                var choice = Console.ReadLine();

                switch (choice.ToLower())
                {
                    case "q":
                        runApp = false;
                        break;
                    case "1":
                        dataset = await GenerateDataset();
                        Console.ReadKey();
                        break;
                    case "2":
                        ShowAvailableDatasets();
                        Console.ReadKey();
                        break;
                    case "3":
                        dataset = await LoadDataset();
                        Console.ReadKey();
                        break;
                    case "4":
                        await SaveDataset(dataset);
                        Console.ReadKey();
                        break;
                    case "5":
                        await ExportDataset(dataset);
                        Console.ReadKey();
                        break;
                    case "6":
                        RunOptimization(dataset);
                        Console.ReadKey();
                        break;
                    default:
                        break;
                }                
            }
        }

        private static void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine("Hello, I am Eugene. These are your options:\n");
            Console.WriteLine("1: Generate random dataset");
            Console.WriteLine("2: Show available datasets");
            Console.WriteLine("3: Load dataset");
            Console.WriteLine("4: Save dataset");
            Console.WriteLine("5: Export dataset");
            Console.WriteLine("6: Run");
            Console.WriteLine("q: Quit");
            Console.Write("\nChoice: ");
        }

        private static void Optimizer_GenerationEvaluated(object sender, EventArgs e)
        {
            GenerationEvaluatedEventArgs args = e as GenerationEvaluatedEventArgs;

            Console.WriteLine($"++++++++++++++++++++  Generation {args.Generation} evaluated with fitness {args.Fitness} ++++++++++++++++++++");
        }

        private static void Optimizer_GenerationProgress(object sender, EventArgs e)
        {
            Console.Write(".");
        }

        private static void Optimizer_ChromosomeEvaluated(object sender, EventArgs e)
        {
            var args = e as ChromosomeEvaluatedEventArgs;
            Console.WriteLine($"\tGenes ({args.Genes.Count}): {string.Join(", ", args.Genes)} --> {args.Fitness}");
        }

        public static async Task<TestcaseBlockerDataset> GenerateDataset()
        {
            return await Task.Run(() =>
            {
                Console.Write("Number of Blockers: ");
                string strBlockers = Console.ReadLine();
                int numBlockers = 0;
                if(!int.TryParse(strBlockers, out numBlockers))
                {
                    Console.WriteLine("Input was not a valid number.");
                    return null;
                }
                if(numBlockers <= 0)
                {
                    Console.WriteLine("Number of Blockers must be greater than zero.");
                    return null;
                }

                Console.Write("Number of Testcases: ");
                string strTestcases = Console.ReadLine();
                int numTestcases = 0;
                if (!int.TryParse(strTestcases, out numTestcases))
                {
                    Console.WriteLine("Input was not a valid number.");
                    return null;
                }
                if (numTestcases <= 0)
                {
                    Console.WriteLine("Number of Testcases must be greater than zero.");
                    return null;
                }

                Console.WriteLine("Generating a random dataset...");

                var dataset = DataGenerator.GenerateRandomData(numBlockers, numTestcases);
                Console.WriteLine(dataset.ToString());
                return dataset;
            });
        }

        public static void ShowAvailableDatasets()
        {
            var path = GetDataPath();
            var files = Directory.GetFiles(path, "*.json").ToList();

            foreach(var file in files)
            {
                Console.WriteLine($"{file}");
            }
        }

        public static async Task<TestcaseBlockerDataset> LoadDataset()
        {
            Console.Write("Enter filename without extension: ");
            string filename = Console.ReadLine();

            var path = Path.Combine(GetDataPath(), filename + ".json");

            if (!File.Exists(path))
            {
                Console.WriteLine($"File not found at {path}");
                return null;
            }

            Console.WriteLine("Loading dataset...");

            var dataset = await TestcaseBlockerDataset.LoadAsync(path);
            Console.WriteLine(dataset.ToString());

            return dataset;
        }

        public static async Task SaveDataset(TestcaseBlockerDataset dataset)
        {
            if(dataset is null)
            {
                Console.WriteLine("Cannot save an empty dataset. First generate or load a dataset.");
                return;
            }

            Console.Write("Enter filename without extension: ");
            string filename = Console.ReadLine();

            var path = Path.Combine(GetDataPath(), filename + ".json");

            Console.WriteLine($"Saving dataset to {path}");

            await dataset.SaveAsync(path);
        }

        public static async Task ExportDataset(TestcaseBlockerDataset dataset)
        {
            if (dataset is null)
            {
                Console.WriteLine("Cannot export an empty dataset. First generate or load a dataset.");
                return;
            }

            Console.Write("Enter filename without extension: ");
            string filename = Console.ReadLine();

            var path = Path.Combine(GetDataPath(), filename + ".xlsx");

            Console.WriteLine($"Exporting dataset to {path}");

            await dataset.ExportAsXlsxAsync(path);
        }

        public static void RunOptimization(TestcaseBlockerDataset dataset)
        {
            if (dataset is null)
            {
                Console.WriteLine("Cannot run optimization for an empty dataset. First generate or load a dataset.");
                return;
            }

            var debugOutput = true;

            Console.Write("Suppress output information for each generation (y)? ");
            if(Console.ReadLine().ToLower() == "y")
            {
                debugOutput = false;
            }

            var watch = new Stopwatch();
            watch.Start();

            var optimizer = new Optimizer(dataset);
            if (debugOutput)
            {
                optimizer.ChromosomeEvaluated += Optimizer_ChromosomeEvaluated;
                optimizer.GenerationEvaluated += Optimizer_GenerationEvaluated;
            }
            else
            {
                optimizer.GenerationEvaluated += Optimizer_GenerationProgress;
            }
            var result = optimizer.Optimize();

            var duration = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Duration: {duration.ToString()} (including output)");
            Console.WriteLine($"Best solution fitness: {result.Fitness}");
            Console.WriteLine($"Resolved Blockers: ({result.NumberOfSolvedBlockers}/{result.Cost}) [{string.Join(",", result.Blockers.Select(x => "'" + x.Name + "'").ToList())}]");
            Console.WriteLine($"Resolved Testcases: ({result.NumberOfSolvedTestcases}/{result.Value}) [{string.Join(",", result.Testcases.Select(x => "'" + x.Name + "'").ToList())}]");
        }

        public static string GetDataPath()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path = Path.Combine(path, "data");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
