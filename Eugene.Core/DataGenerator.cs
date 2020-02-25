using Eugene.Core.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
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

        public static TestcaseBlockerDataset Import(string filename)
        {
            var worksheetName = "Rohdaten_19.02.2020";

            var columnNumberTestcaseId = 2;
            var columnNumberTestcaseName = 4;

            var columnNumberBlockerNames = 1;
            var blockerSeparator = ',';

            var firstDataRow = 2;
            var lastDataRow = firstDataRow;

            var blockers = new List<Blocker>();
            var testcases = new List<Testcase>();

            FileInfo existingFile = new FileInfo(filename);
            using (var excel = new ExcelPackage(existingFile))
            {
                var ws = excel.Workbook.Worksheets[worksheetName];
                lastDataRow = ws.Dimension.End.Row;

                for(int row = firstDataRow; row <= lastDataRow; row++)
                {
                    var testcaseId = (Convert.ToString(ws.Cells[row, columnNumberTestcaseId].Value)).Trim();
                    var testcaseName = (Convert.ToString(ws.Cells[row, columnNumberTestcaseName].Value)).Trim();

                    var blockerNamesString = (Convert.ToString(ws.Cells[row, columnNumberBlockerNames].Value)).Trim();
                    var blockerNames = new List<string>(blockerNamesString.Split(blockerSeparator));

                    var testcase = new Testcase
                    {
                        Id = testcaseId,
                        Name = testcaseName
                    };

                    foreach(var blockerName in blockerNames)
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
    }
}
