using Eugene.Core.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Eugene.Core
{
    public static class DataGenerator
    {
        public static TestcaseBlockerDataset GenerateRandomData(int numberOfBlockers, int numberOfTestcases)
        {
            var blockers = new List<Blocker>();
            var testcases = new List<Testcase>();

            var rnd = new Random();

            // Generate Blockers
            for (int i = 1; i <= numberOfBlockers; i++)
            {
                blockers.Add(new Blocker
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Blocker {i}"
                });
            }

            // Generate Testcases
            for (int i = 1; i <= numberOfTestcases; i++)
            {
                var testcase = new Testcase
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Testcase {i}"
                };

                var numberOfBlockersForTestcase = rnd.Next(0, (int)Math.Ceiling(0.5 * numberOfBlockers));
                while (testcase.BlockerIds.Count < numberOfBlockersForTestcase)
                {
                    var blockerId = blockers[rnd.Next(0, numberOfBlockers)].Id;
                    if (!testcase.BlockerIds.Contains(blockerId))
                        testcase.BlockerIds.Add(blockerId);
                }

                testcases.Add(testcase);
            }

            TestcaseBlockerDataset configuration = new TestcaseBlockerDataset
            {
                Blockers = blockers,
                Testcases = testcases
            };

            return configuration;
        }

        public static TestcaseBlockerDataset ImportFromExcel(string filename, ExcelImportOptions options)
        {
            if (string.IsNullOrEmpty(options.WorksheetName))
                throw new ArgumentException("WorksheetName is empty");

            if (string.IsNullOrEmpty(options.BlockerSeparator))
                throw new ArgumentException("BlockerSeparator is empty");

            if (options.ColumnNumberTestcaseId <= 0)
                throw new ArgumentException("ColumnNumberTestcaseId must be greater than zero.");

            if (options.ColumnNumberTestcaseName <= 0)
                throw new ArgumentException("ColumnNumberTestcaseName must be greater than zero.");

            if (options.ColumnNumberBlockerNames <= 0)
                throw new ArgumentException("ColumnNumberBlockerNames must be greater than zero.");

            if (options.FirstDataRow <= 0)
                throw new ArgumentException("FirstDataRow must be greater than zero.");

            try
            {
                var blockers = new List<Blocker>();
                var testcases = new List<Testcase>();

                FileInfo existingFile = new FileInfo(filename);
                using (var excel = new ExcelPackage(existingFile))
                {
                    var ws = excel.Workbook.Worksheets[options.WorksheetName];
                    if (!options.LastDataRow.HasValue)
                        options.LastDataRow = ws.Dimension.End.Row;

                    for (int row = options.FirstDataRow; row <= options.LastDataRow.Value; row++)
                    {
                        var testcaseId = (Convert.ToString(ws.Cells[row, options.ColumnNumberTestcaseId].Value)).Trim();
                        var testcaseName = (Convert.ToString(ws.Cells[row, options.ColumnNumberTestcaseName].Value)).Trim();

                        var blockerNamesString = (Convert.ToString(ws.Cells[row, options.ColumnNumberBlockerNames].Value)).Trim();
                        var blockerNames = new List<string>(blockerNamesString.Split(options.BlockerSeparator.ToCharArray()));

                        var testtype = options.ColumnNumberTestType.HasValue ? (Convert.ToString(ws.Cells[row, options.ColumnNumberTestType.Value].Value)).Trim() : string.Empty;
                        var applicationModule = options.ColumnNumberApplicationModule.HasValue ? (Convert.ToString(ws.Cells[row, options.ColumnNumberApplicationModule.Value].Value)).Trim(): string.Empty;

                        var testcase = new Testcase
                        {
                            Id = testcaseId,
                            Name = testcaseName,
                            TestType = testtype,
                            ApplicationModule = applicationModule
                        };

                        foreach (var blockerName in blockerNames)
                        {
                            var trimmedBlockerName = blockerName.Trim().ToLower();

                            if (!string.IsNullOrEmpty(trimmedBlockerName))
                            {
                                var existingBlocker = blockers.Where(x => x.Name == trimmedBlockerName).FirstOrDefault();

                                if (existingBlocker == null)
                                {
                                    existingBlocker = new Blocker
                                    {
                                        Id = Guid.NewGuid().ToString().Replace("-", ""),
                                        Name = trimmedBlockerName,
                                        Cost = 1
                                    };

                                    blockers.Add(existingBlocker);
                                }

                                if (!testcase.BlockerIds.Contains(existingBlocker.Id))
                                    testcase.BlockerIds.Add(existingBlocker.Id);
                            }
                        }

                        testcases.Add(testcase);
                    }
                }

                var dataset = new TestcaseBlockerDataset
                {
                    Blockers = blockers,
                    Testcases = testcases
                };

                return dataset;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw; 
            }
        }
    }
}
