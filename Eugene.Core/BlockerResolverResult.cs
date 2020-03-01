using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core
{
    public class BlockerResolverResult
    {
        private TestcaseBlockerDataset _initialDataset;
        public TestcaseBlockerDataset InitialDataset
        {
            get { return _initialDataset; }
            set { _initialDataset = value; }
        }

        private TestcaseBlockerDataset _resolvedDataset;
        public TestcaseBlockerDataset ResolvedDataset
        {
            get { return _resolvedDataset; }
            set { _resolvedDataset = value; }
        }

        public BlockerResolverResult(TestcaseBlockerDataset initialDataset, TestcaseBlockerDataset resolvedDataset)
        {
            _initialDataset = initialDataset;
            _resolvedDataset = resolvedDataset;
        }

        public List<Blocker> UnresolvedBlockers
        {
            get
            {
                if (ResolvedDataset == null)
                    return new List<Blocker>();

                return ResolvedDataset.Blockers.ToList();
            }
        }

        public List<Blocker> ResolvedBlockers
        {
            get
            {
                if (InitialDataset == null)
                    return new List<Blocker>();

                if (ResolvedDataset == null)
                    return new List<Blocker>();

                var initialDatasetBlockers = InitialDataset.Blockers;
                var resolvedDatasetBlockers = ResolvedDataset.Blockers;

                var resolvedBlockers = initialDatasetBlockers.Except(resolvedDatasetBlockers);

                return resolvedBlockers.ToList();
            }
        }

        public List<Testcase> UnresolvedTestcases
        {
            get
            {
                if (ResolvedDataset == null)
                    return new List<Testcase>();

                return ResolvedDataset.BlockedTestcases.ToList();
            }
        }

        public List<Testcase> ResolvedTestcases
        {
            get
            {
                if (InitialDataset == null)
                    return new List<Testcase>();

                if (ResolvedDataset == null)
                    return new List<Testcase>();

                var initialDatasetTestcases= InitialDataset.UnblockedTestcases;
                var resolvedDatasetTestcases = ResolvedDataset.UnblockedTestcases;

                var resolvedTestcases = resolvedDatasetTestcases.Except(initialDatasetTestcases);

                return resolvedTestcases.ToList();
            }
        }
    }
}
