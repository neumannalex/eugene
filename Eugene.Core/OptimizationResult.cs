using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eugene.Core
{
    public class OptimizationResult
    {
        public double Fitness { get; private set; } = 0;
        public double Value { get; private set; } = 0;
        public double ValueIncludingUnblocked { get; private set; } = 0;
        public double Cost { get; private set; } = 0;
        public List<Blocker> ResolvedBlockers { get; private set; } = new List<Blocker>();
        public List<Testcase> ResolvedTestcases { get; private set; } = new List<Testcase>();
        public List<Testcase> ResolvedTestcasesIncludingUnblocked { get; private set; } = new List<Testcase>();

        public int NumberOfResolvedBlockers
        {
            get
            {
                if (ResolvedBlockers is null)
                    return 0;

                return ResolvedBlockers.Count;
            }
        }

        public int NumberOfResolvedTestcases
        {
            get
            {
                if (ResolvedTestcases is null)
                    return 0;

                return ResolvedTestcases.Count;
            }
        }

        public int NumberOfResolvedTestcasesIncludingUnblocked
        {
            get
            {
                if (ResolvedTestcasesIncludingUnblocked is null)
                    return 0;

                return ResolvedTestcasesIncludingUnblocked.Count;
            }
        }

        public OptimizationResult(double fitness, double value, double valueIncludingUnblocked,  double cost, List<Blocker> solvedBlockers, List<Testcase> solvedTestcases, List<Testcase> solvedTestcasesIncludingUnblocked)
        {
            Fitness = fitness;
            Value = value;
            ValueIncludingUnblocked = valueIncludingUnblocked;
            Cost = cost;
            ResolvedBlockers = solvedBlockers;
            ResolvedTestcases = solvedTestcases;
            ResolvedTestcasesIncludingUnblocked = solvedTestcasesIncludingUnblocked;
        }

        public async Task SaveAsync(string filename)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using (FileStream fs = File.Create(filename))
            {
                await JsonSerializer.SerializeAsync(fs, this, jsonOptions);
            }
        }
    }
}
