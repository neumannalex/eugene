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

        public BlockerResolver(TestcaseBlockerDataset initialDataset)
        {
            _initialDataset = initialDataset;
        }

        public BlockerResolverResult Resolve(List<Blocker> blockers)
        {
            var localDataset = (TestcaseBlockerDataset)CloneHelper.GetDeepClone(_initialDataset);

            // Remove Blockers from Testcases
            foreach (var testcase in localDataset.Testcases)
            {
                foreach (var blocker in blockers)
                {
                    testcase.BlockerIds.Remove(blocker.Id);
                }
            }

            // Remove Blockers from Dataset
            var blockersToRemove = new List<Blocker>();
            foreach(var blocker in localDataset.Blockers)
            {
                var numBlockedTestcases = localDataset.Testcases.Where(x => x.BlockerIds.Contains(blocker.Id)).Count();
                if (numBlockedTestcases <= 0)
                    blockersToRemove.Add(blocker);
            }

            foreach (var blocker in blockersToRemove)
                localDataset.Blockers.Remove(blocker);

            var result = new BlockerResolverResult(_initialDataset, localDataset);

            return result;
        }
    }
}
