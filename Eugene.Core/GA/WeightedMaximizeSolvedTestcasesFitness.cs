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
        private BlockerResolver _resolver;

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
            _resolver = new BlockerResolver(_dataset);
        }

        public double Evaluate(IChromosome chromosome)
        {
            var myChromosome = chromosome as BlockedTestcasesChromosome;
            var genes = myChromosome.GetGenes().Select(x => (bool)x.Value).ToList();

            var blockersToResolve = GetBlockersToResolveFromChromosome(myChromosome);

            var resolution = _resolver.Resolve(blockersToResolve);

            var cost = _dataset.GetCostForBlockers(resolution.ResolvedBlockers);
            var value = _dataset.GetValueForTestcases(resolution.ResolvedTestcases);
            value /= _dataset.TotalValue;

            double fitness = value / cost;

            return fitness;
        }

        private List<Blocker> GetBlockersToResolveFromChromosome(BlockedTestcasesChromosome chromosome)
        {
            var blockers = new List<Blocker>();

            var genes = chromosome.GetGenes();
            for (int i = 0; i < genes.Length; i++)
            {
                if ((bool)genes[i].Value == true)
                {
                    blockers.Add(_dataset.Blockers[i]);
                }
            }

            return blockers;
        }
    }
}
