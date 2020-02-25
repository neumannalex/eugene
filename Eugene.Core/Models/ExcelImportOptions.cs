using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.Models
{
    public class ExcelImportOptions
    {
        public string WorksheetName { get; set; }

        public int ColumnNumberTestcaseId { get; set; }
        public int ColumnNumberTestcaseName { get; set; }
        public int ColumnNumberBlockerNames { get; set; }
        public string BlockerSeparator { get; set; } = ",";

        public int FirstDataRow { get; set; } = 1;
        public int? LastDataRow { get; set; } = null;
    }
}
