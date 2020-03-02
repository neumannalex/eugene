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

        public WeightedMaximizeSolvedTestcasesFitness(BlockerResolver resolver)
        {
            _resolver = resolver;
        }

        public double Evaluate(IChromosome chromosome)
        {
            var myChromosome = chromosome as BlockedTestcasesChromosome;
            var genes = myChromosome.GetGenes().Select(x => (bool)x.Value).ToList();

            var blockersToResolve = GetBlockersToResolveFromChromosome(myChromosome, _resolver.InitialDataset.Blockers.ToList());

            var resolution = _resolver.Resolve(blockersToResolve);

            var cost = _resolver.InitialDataset.GetCostForBlockers(resolution.ResolvedBlockers);
            var value = _resolver.InitialDataset.GetValueForTestcases(resolution.ResolvedTestcases);
            value /= _resolver.InitialDataset.TotalValue;

            double fitness = value / cost;

            return fitness;
        }

        private List<Blocker> GetBlockersToResolveFromChromosome(BlockedTestcasesChromosome chromosome, List<Blocker> blockers)
        {
            var selectedBlockers = new List<Blocker>();

            var genes = chromosome.GetGenes();
            for (int i = 0; i < genes.Length; i++)
            {
                if ((bool)genes[i].Value == true)
                {
                    selectedBlockers.Add(blockers[i]);
                }
            }

            return selectedBlockers;
        }
    }
}
