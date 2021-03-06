﻿using Eugene.Core.GA;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eugene.Core
{
    public class Optimizer
    {
        private int _generationCounter = 0;
        private readonly TestcaseBlockerDataset _dataset;
        private BlockerResolver _resolver;

        private GeneticAlgorithm _ga;
        private WeightedMaximizeSolvedTestcasesFitness _fitness;
        private IChromosome _chromosome;
        private IPopulation _population;
        private ISelection _selection;
        private ICrossover _crossover;
        private IMutation _mutation;

        public object DeepCopy { get; private set; }

        public event EventHandler ChromosomeEvaluated;
        protected virtual void OnChromosomeEvaluated(ChromosomeEvaluatedEventArgs e)
        {
            EventHandler handler = ChromosomeEvaluated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler GenerationEvaluated;
        protected virtual void OnGenerationEvaluated(GenerationEvaluatedEventArgs e)
        {
            EventHandler handler = GenerationEvaluated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Optimizer(TestcaseBlockerDataset dataset)
        {
            _dataset = dataset;
            _resolver = new BlockerResolver(_dataset);
        }

        public OptimizationResult Optimize()
        {
            _selection = new EliteSelection();
            _crossover = new UniformCrossover();
            _mutation = new TworsMutation();

            _chromosome = new BlockedTestcasesChromosome(_dataset.Blockers.Count);
            _fitness = new WeightedMaximizeSolvedTestcasesFitness(_resolver);

            _population = new Population(20, 40, _chromosome);

            _ga = new GeneticAlgorithm(_population, _fitness, _selection, _crossover, _mutation);
            _ga.Termination = new OrTermination( new FitnessStagnationTermination(100),
                                                new GenerationNumberTermination(10000),
                                                new TimeEvolvingTermination(TimeSpan.FromMinutes(5)));

            //_ga.GenerationRan += Ga_GenerationRan;
            //_fitness.Evaluated += Fitness_Evaluated;

            _ga.Start();

            var resolvedBlockers = GetBlockersToResolve(_ga.BestChromosome);            
            var resolution = _resolver.Resolve(resolvedBlockers);

            var resolvedTestcases = resolution.ResolvedTestcases;
            var resolvedTestcasesIncludingUnblocked = resolution.ResolvedDataset.Testcases.Where(x => x.BlockerIds.Count <= 0).ToList();// GetResolvedTestcasesIncludingUnblocked(resolvedBlockers);

            var cost = _dataset.GetCostForBlockers(resolvedBlockers);
            var value = _dataset.GetValueForTestcases(resolvedTestcases);
            var valueIncludingUnblocked = _dataset.GetValueForTestcases(resolvedTestcasesIncludingUnblocked);

            var optimizationResult = new OptimizationResult(_ga.BestChromosome.Fitness.Value, value, valueIncludingUnblocked, cost, resolvedBlockers, resolvedTestcases, resolvedTestcasesIncludingUnblocked);
            return optimizationResult;
        }

        private List<Blocker> GetBlockersToResolve(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            var blockersToResolve = new List<Blocker>();
            for (int i = 0; i < genes.Length; i++)
            {
                if ((bool)genes[i].Value == true)
                {
                    blockersToResolve.Add(_dataset.Blockers[i]);
                }
            }

            return blockersToResolve;
        }

        private void Fitness_Evaluated(object sender, EventArgs e)
        {
            var args = e as ChromosomeEvaluatedEventArgs;
            OnChromosomeEvaluated(args);
        }

        private void Ga_GenerationRan(object sender, EventArgs e)
        {
            var genes = _ga.BestChromosome.GetGenes().Select(x => (bool)x.Value).ToList();

            var resolvedBlockers = GetBlockersToResolve(_ga.BestChromosome);
            var resolution = _resolver.Resolve(resolvedBlockers);
            var resolvedTestcases = resolution.ResolvedTestcases;

            var cost = _dataset.GetCostForBlockers(resolvedBlockers);
            var value = _dataset.GetValueForTestcases(resolvedTestcases);

            OnGenerationEvaluated(new GenerationEvaluatedEventArgs(
                _generationCounter++,
                _ga.BestChromosome.Fitness.Value,
                value,
                cost,
                resolvedBlockers,
                resolvedTestcases,
                genes));
        }
    }
}
