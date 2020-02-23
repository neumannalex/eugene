using Eugene.Core.Helper;
using Eugene.Core.Models;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core.GA
{
    public class WeightedMaximizeSolvedTestcasesFitness : IFitness
    {
        private readonly TestcaseBlockerDataset _dataset;

        private double MaximumCost
        {
            get
            {
                // Lineare Kosten
                //return _dataset.Blockers.Sum(x => x.Cost);

                var cost = 0.0;

                for(int i = 0; i < _dataset.Blockers.Count; i++)
                    cost += GetBlockerCost(_dataset.Blockers[i], i);

                return cost;
            }
        }

        private double MaximumValue
        {
            get
            {
                return _dataset.Testcases.Sum(x => x.Weight);
            }
        }

        public event EventHandler Evaluated;
        protected virtual void OnEvaluated(ChromosomeEvaluatedEventArgs e)
        {
            EventHandler handler = Evaluated;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        public WeightedMaximizeSolvedTestcasesFitness(TestcaseBlockerDataset configuration)
        {
            _dataset = configuration;
        }

        public double Evaluate(IChromosome chromosome)
        {
            var myChromosome = chromosome as BlockedTestcasesChromosome;
            var genes = myChromosome.GetGenes().Select(x => (bool)x.Value).ToList();

            var blockersToResolve = GetBlockersToResolveFromChromosome(myChromosome);
            var resolvedTestcases = GetTestcasesForResolvedBlockers(blockersToResolve);

            if (blockersToResolve.Count <= 0 || resolvedTestcases.Count <= 0)
                return 0;

            var cost = GetCostForResolvedBlockers(blockersToResolve);
            var value = GetValueForResolvedTestcases(resolvedTestcases);

            double fitness = value / cost;

            OnEvaluated(new ChromosomeEvaluatedEventArgs(fitness, value, cost, blockersToResolve, resolvedTestcases, genes));

            return fitness;
        }

        private List<Blocker> GetBlockersToResolveFromChromosome(BlockedTestcasesChromosome chromosome)
        {
            var allBlockers = DeepClone.GetDeepClone<List<Blocker>>(_dataset.Blockers.ToList());
            var blockers = new List<Blocker>();

            var genes = chromosome.GetGenes();
            for (int i = 0; i < genes.Length; i++)
            {
                if ((bool)genes[i].Value == true)
                {
                    blockers.Add(allBlockers[i]);
                }
            }

            return blockers;
        }

        private List<Testcase> GetTestcasesForResolvedBlockers(List<Blocker> resolvedBlockers)
        {
            var allTestcases = DeepClone.GetDeepClone<List<Testcase>>(_dataset.Testcases.Where(x => x.BlockerIds.Count > 0).ToList());
            var resolvedTestcases = new List<Testcase>();

            foreach (var testcase in allTestcases)
            {
                foreach (var blocker in resolvedBlockers)
                {
                    if (testcase.BlockerIds.Contains(blocker.Id))
                    {
                        testcase.BlockerIds.Remove(blocker.Id);
                    }
                }

                if (testcase.BlockerIds.Count <= 0)
                {
                    resolvedTestcases.Add(testcase);
                }
            }

            return resolvedTestcases;
        }

        public double GetValueForResolvedTestcases(List<Testcase> resolvedTestcases)
        {
            // Lineare Kosten
            return resolvedTestcases.Sum(x => x.Weight);
        }

        public double GetCostForResolvedBlockers(List<Blocker> resolvedBlockers)
        {
            // Lineare Kosten
            //return resolvedBlockers.Sum(x => x.Cost);

            var cost = 0.0;

            for (int i = 0; i < resolvedBlockers.Count; i++)
                cost += GetBlockerCost(resolvedBlockers[i], i);

            return cost;
        }

        private double GetBlockerCost(Blocker blocker, int rank = 0)
        {
            return Math.Pow(2, rank) * blocker.Cost;
        }
    }
}
