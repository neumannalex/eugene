using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core
{
    public class OptimizationResult
    {
        public double Fitness { get; private set; } = 0;
        public double Value { get; private set; } = 0;
        public double Cost { get; private set; } = 0;
        public List<Blocker> Blockers { get; private set; } = new List<Blocker>();
        public List<Testcase> Testcases { get; private set; } = new List<Testcase>();        

        public int NumberOfSolvedBlockers
        {
            get
            {
                if (Blockers is null)
                    return 0;

                return Blockers.Count;
            }
        }

        public int NumberOfSolvedTestcases
        {
            get
            {
                if (Testcases is null)
                    return 0;

                return Testcases.Count;
            }
        }

        public OptimizationResult(double fitness, double value, double cost, List<Blocker> solvedBlockers, List<Testcase> solvedTestcases)
        {
            Fitness = fitness;
            Value = value;
            Cost = cost;
            Blockers = solvedBlockers;
            Testcases = solvedTestcases;
        }
    }
}
