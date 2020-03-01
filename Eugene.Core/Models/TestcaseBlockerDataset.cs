using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eugene.Core.Models
{
    public class TestcaseBlockerDataset
    {
        public IList<Blocker> Blockers { get; set; } = new List<Blocker>();
        public IList<Testcase> Testcases { get; set; } = new List<Testcase>();

        public IList<Testcase> BlockedTestcases
        {
            get
            {
                return Testcases.Where(x => x.BlockerIds.Count > 0).ToList();
            }
        }
        public int NumberOfBlockedTestcases
        {
            get
            {
                return BlockedTestcases.Count;
            }
        }

        public IList<Testcase> UnblockedTestcases
        {
            get
            {
                return Testcases.Where(x => x.BlockerIds.Count <= 0).ToList();
            }
        }
        public int NumberOfUnblockedTestcases
        {
            get
            {
                return UnblockedTestcases.Count;
            }
        }

        public IList<Blocker> UnresolvedBlockers
        {
            get
            {
                Dictionary<Blocker, int> blockedTestcasesCount = new Dictionary<Blocker, int>();
                foreach(var blocker in Blockers)
                {
                    var count = 0;
                    foreach(var testcase in Testcases)
                    {
                        if (testcase.BlockerIds.Contains(blocker.Id))
                            count++;
                    }
                    blockedTestcasesCount.Add(blocker, count);
                }

                return blockedTestcasesCount.Where(x => x.Value > 0).Select(x => x.Key).ToList();
            }
        }
        public int NumberOfUnresolvedBlockers
        {
            get
            {
                return UnresolvedBlockers.Count;
            }
        }

        public double BlockedValue
        {
            get
            {
                return GetValueForTestcases(BlockedTestcases);
            }
        }

        public double UnblockedValue
        {
            get
            {
                return GetValueForTestcases(UnblockedTestcases);
            }
        }

        public double TotalValue
        {
            get
            {
                return GetValueForTestcases(Testcases);
            }
        }

        public double TotalCost
        {
            get
            {
                return GetCostForBlockers(Blockers);
            }
        }

        public double GetCostForBlockers(IEnumerable<Blocker> blockers)
        {
            return blockers.Sum(x => x.Cost);
        }

        public double GetValueForTestcases(IEnumerable<Testcase> testcases)
        {
            return testcases.Sum(x => x.Weight);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Blockers:");
            foreach (var blocker in Blockers)
                sb.AppendLine($"\tId: {blocker.Id}, Name: {blocker.Name}");

            sb.AppendLine("\nTestcases:");
            foreach (var testcase in Testcases)
            {
                var ids = string.Join(",", testcase.BlockerIds);
                sb.AppendLine($"\tId: {testcase.Id}, Name: {testcase.Name}, Blockers: {ids}");
            }

            return sb.ToString();
        }

        public async Task ExportAsXlsxAsync(string filename)
        {
            await Task.Run(() =>
            {
                using (var xls = new ExcelPackage())
                {
                    var ws = xls.Workbook.Worksheets.Add("Data");

                    // Blocker
                    for (int i = 0; i < Blockers.Count; i++)
                    {
                        ws.Cells[1, i + 3].Value = Blockers[i].Name;
                        ws.Cells[1, i + 3].Style.TextRotation = 90;
                        ws.Cells[1, i + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        ws.Cells[1, i + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells[Testcases.Count + 2, i + 3].FormulaR1C1 = $"=SUBTOTAL(3,R[-{Testcases.Count}]C:R[-1]C)";
                        ws.Cells[Testcases.Count + 2, i + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[Testcases.Count + 2, i + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    // Anzahl Blocker
                    ws.Cells[1, Blockers.Count + 3].Value = "Anzahl Blocker";
                    ws.Cells[1, Blockers.Count + 3].Style.TextRotation = 90;
                    ws.Cells[1, Blockers.Count + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    ws.Cells[1, Blockers.Count + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Testfälle
                    for (int i = 0; i < Testcases.Count; i++)
                    {
                        ws.Cells[i + 2, 1].Value = Testcases[i].Id;
                        ws.Cells[i + 2, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        ws.Cells[i + 2, 2].Value = Testcases[i].Name;
                        ws.Cells[i + 2, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i + 2, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        ws.Cells[i + 2, Blockers.Count + 3].FormulaR1C1 = $"=COUNTIF(RC[-{Blockers.Count}]:RC[-1],\"=x\")";
                        ws.Cells[i + 2, Blockers.Count + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Cells[i + 2, Blockers.Count + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    // Anzahl geblockte Testfälle
                    ws.Cells[Testcases.Count + 2, 1].Value = "Geblockte Testfälle";
                    ws.Cells[Testcases.Count + 2, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[Testcases.Count + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    // Adjazenz
                    for (int row = 0; row < Testcases.Count; row++)
                    {
                        var test = Testcases[row];
                        for (int col = 0; col < Blockers.Count; col++)
                        {
                            var blocked = test.BlockerIds.Contains(Blockers[col].Id);

                            if (blocked)
                            {
                                ws.Cells[row + 2, col + 3].Value = "x";
                            }

                            ws.Cells[row + 2, col + 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            ws.Cells[row + 2, col + 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                    }

                    // Globale Formate
                    for(int col = 3; col <= Blockers.Count + 3; col++) // funktioniert nicht
                        ws.Column(col).AutoFit();

                    ws.Cells[1, 3, 1, Blockers.Count + 3].AutoFilter = true;

                    // Speichern
                    xls.SaveAs(new FileInfo(filename));
                }
            });
        }
    }
}
