using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.GA
{
    public class BlockedTestcasesChromosome : ChromosomeBase
    {
        public BlockedTestcasesChromosome(int numberOfBlockers) : base(numberOfBlockers)
        {
            CreateGenes();
        }

        public override IChromosome CreateNew()
        {
            return new BlockedTestcasesChromosome(Length);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            //var maxNumberOfUsedBlockers = 3;
            //var threshold = (double)maxNumberOfUsedBlockers / (double)this.Length;
            //var threshold = Math.Sqrt((double)maxNumberOfUsedBlockers / (double)this.Length);

            var threshold = 0.5;

            var p = RandomizationProvider.Current.GetDouble(0, 1);
            return new Gene(p < threshold ? true : false);
        }
    }
}
