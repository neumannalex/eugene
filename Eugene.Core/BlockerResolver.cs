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
        private TestcaseBlockerDataset _initialDataset;
        public TestcaseBlockerDataset InitialDataset
        {
            get
            {
                return _initialDataset;
            }
        }

        public BlockerResolver(TestcaseBlockerDataset initialDataset)
        {
            _initialDataset = initialDataset;
        }

        public BlockerResolverResult Resolve2(List<Blocker> blockers)
        {
            var remainingBlockers = new List<Blocker>();
            foreach(var blocker in InitialDataset.Blockers)
            {
                // Anzahl der durch diesen Blocker blockierten Testfälle
                var numBlockedTestcases = InitialDataset.Testcases.Where(x => x.BlockerIds.Contains(blocker.Id)).Count();
                
                // Blocker kann beibehalten werden, wenn er Testfälle blockiert
                if (numBlockedTestcases > 0)
                {
                    // zu lösende Blocker enthalten den aktuellen Blocker nicht --> aktueller Blocker wird beibehalten
                    if (blockers.Where(x => x.Id == blocker.Id).Count() <= 0)
                    {
                        remainingBlockers.Add(blocker);
                    }
                }
            }

            var test = InitialDataset.Blockers.Except(blockers);

            var resolvedTestcases = new List<Testcase>();
            // Remove Blockers from Testcases
            foreach (var testcase in InitialDataset.Testcases)
            {
                var blockerIds = new List<string>();
                foreach(var remainingBlocker in remainingBlockers)
                {
                    if(testcase.BlockerIds.Contains(remainingBlocker.Id))
                    {
                        blockerIds.Add(remainingBlocker.Id);
                    }
                }

                resolvedTestcases.Add(new Testcase
                {
                    Id = testcase.Id,
                    Name = testcase.Name,
                    Weight = testcase.Weight,
                    BlockerIds = blockerIds
                });
            }

            var localDataset = new TestcaseBlockerDataset
            {
                Blockers = remainingBlockers,
                Testcases = resolvedTestcases
            };

            var result = new BlockerResolverResult(_initialDataset, localDataset);

            return result;
        }

        public BlockerResolverResult Resolve(List<Blocker> blockers)
        {
            var blockerIdsToResolve = blockers.Select(x => x.Id);
            //var remainingBlockers = InitialDataset.Blockers.Except(blockers);
            var resolvedTestcases = new List<Testcase>();

            // Remove Blockers from Testcases
            foreach (var testcase in InitialDataset.Testcases)
            {
                var testcaseBlockerIds = new List<string>(testcase.BlockerIds.Where(x => !blockerIdsToResolve.Contains(x)));

                resolvedTestcases.Add(new Testcase
                {
                    Id = testcase.Id,
                    Name = testcase.Name,
                    Weight = testcase.Weight,
                    BlockerIds = testcaseBlockerIds
                });
            }

            var unresolvedBlockerIds = resolvedTestcases.SelectMany(x => x.BlockerIds).Distinct();
            var unresolvedBlockers = new List<Blocker>();
            foreach(var blocker in InitialDataset.Blockers)
            {
                if (unresolvedBlockerIds.Contains(blocker.Id))
                    unresolvedBlockers.Add(blocker);
            }

            var localDataset = new TestcaseBlockerDataset
            {
                Blockers = unresolvedBlockers,
                Testcases = resolvedTestcases
            };

            var result = new BlockerResolverResult(_initialDataset, localDataset);

            return result;
        }
    }
}
