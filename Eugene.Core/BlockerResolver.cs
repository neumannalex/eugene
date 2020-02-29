using Eugene.Core.Helper;
using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core
{
    public class BlockerResolver
    {
        private readonly List<Blocker> _originalBlockers = new List<Blocker>();
        private readonly List<Testcase> _originalTestcases = new List<Testcase>();

        public BlockerResolver(List<Blocker> blockers, List<Testcase> testcases)
        {
            _originalBlockers = blockers;
            _originalTestcases = testcases;
        }

        public double GetValueOfTestcases(List<string> testcaseIds)
        {
            var value = 0.0;

            for (int i = 0; i < testcaseIds.Count; i++)
                value += GetCostOfTestcase(testcaseIds[i]);

            return value;
        }

        public double GetValueOfTestcases(List<Testcase> testcases)
        {
            return GetValueOfTestcases(testcases.Select(x => x.Id).ToList());
        }

        public double GetCostOfTestcase(string testcaseId)
        {
            var existingTestcase = _originalTestcases.Where(x => x.Id == testcaseId).FirstOrDefault();
            if(existingTestcase != null)
            {
                return existingTestcase.Weight;
            }
            else
            {
                return 0;
            }
        }

        public double GetCostOfTestcase(Testcase testcase)
        {
            return GetCostOfTestcase(testcase.Id);
        }

        public double GetCostOfBlockers(List<string> blockerIds)
        {
            var cost = 0.0;

            for (int i = 0; i < blockerIds.Count; i++)
                cost += GetCostOfBlocker(blockerIds[i], i);

            return cost;
        }

        public double GetCostOfBlockers(List<Blocker> blockers)
        {
            return GetCostOfBlockers(blockers.Select(x => x.Id).ToList());
        }

        public double GetCostOfBlocker(string blockerId, int rank = 0)
        {
            var blocker = _originalBlockers.Where(x => x.Id == blockerId).FirstOrDefault();
            if(blocker != null)
            {
                return blocker.Cost;
            }
            else
            {
                return 0;
            }
        }

        public double GetCostOfBlocker(Blocker blocker, int rank = 0)
        {
            return GetCostOfBlocker(blocker.Id, rank);
        }

        public List<Testcase> GetTestcasesResolvedByBlockers(List<string> blockerIds)
        {
            throw new NotImplementedException();
        }

        public List<Testcase> GetUnblockedTestcases()
        {
            var unblockedTestcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.Where(x => x.BlockerIds.Count <= 0).ToList());
            return unblockedTestcases;
        }

        public List<string> GetUnblockedTestcaseIds()
        {
            var unblockedTestcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.Where(x => x.BlockerIds.Count <= 0).ToList());
            return unblockedTestcases.Select(x => x.Id).ToList();
        }

        public List<Testcase> GetBlockedTestcases()
        {
            var blockedTestcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.Where(x => x.BlockerIds.Count > 0).ToList());
            return blockedTestcases;
        }

        public List<string> GetBlockedTestcaseIds()
        {
            var blockedTestcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.Where(x => x.BlockerIds.Count > 0).ToList());
            return blockedTestcases.Select(x => x.Id).ToList();
        }

        public BlockerResolutionResult GetResolution(List<string> blockerIdsToResolve)
        {
            // Kopien der Originale erstellen
            var allBlockers = DeepClone.GetDeepClone<List<Blocker>>(_originalBlockers.ToList());
            var allTestcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.ToList());


            foreach (var testcase in allTestcases)
            {
                foreach (var blockerId in blockerIdsToResolve)
                {
                    testcase.BlockerIds.Remove(blockerId);
                }
            }

            TestcaseBlockerDataset resolvedDataset = new TestcaseBlockerDataset
            {
                Blockers = allBlockers,
                Testcases = allTestcases
            };

            return new BlockerResolutionResult(
                new TestcaseBlockerDataset { 
                    Blockers = allBlockers,
                    Testcases = DeepClone.GetDeepClone<List<Testcase>>(_originalTestcases.ToList())
                },
                new TestcaseBlockerDataset {
                    Blockers = allBlockers,
                    Testcases = allTestcases
                });
        }
    }
}
