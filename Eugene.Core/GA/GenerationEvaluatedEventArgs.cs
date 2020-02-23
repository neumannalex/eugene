using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.GA
{
    public class GenerationEvaluatedEventArgs : EventArgs
    {
        public int Generation { get; private set; }
        public double Fitness { get; private set; }
        public double Value { get; private set; }
        public double Cost { get; private set; }
        public List<Blocker> Blockers { get; private set; } = new List<Blocker>();
        public List<Testcase> Testcases { get; private set; } = new List<Testcase>();
        public List<bool> Genes { get; private set; } = new List<bool>();

        public GenerationEvaluatedEventArgs(int generation, double fitness, double value, double cost, List<Blocker> blockers, List<Testcase> testcases, List<bool> genes)
        {
            Generation = generation;
            Fitness = fitness;
            Value = value;
            Cost = cost;
            Blockers = blockers;
            Testcases = testcases;
            Genes = genes;
        }
    }
}
