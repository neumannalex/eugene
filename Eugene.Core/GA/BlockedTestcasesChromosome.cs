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
            var p = RandomizationProvider.Current.GetDouble(0, 1);
            return new Gene(p >= 0.5 ? true : false);
        }
    }
}
