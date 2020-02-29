using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.Models
{
    public class BlockerResolutionResult
    {
        public TestcaseBlockerDataset OriginalDataset { get; private set; }
        public TestcaseBlockerDataset ResolvedDataset { get; private set; }

        public BlockerResolutionResult(TestcaseBlockerDataset originalDataset, TestcaseBlockerDataset resolvedDataset)
        {
            OriginalDataset = originalDataset;
            ResolvedDataset = resolvedDataset;
        }

        public int NumberOfResolvedBlockersDifference
        {
            get
            {
                if (OriginalDataset == null || ResolvedDataset == null)
                    return 0;

                return ResolvedDataset.NumberOfResolvedBlockers - OriginalDataset.NumberOfResolvedBlockers;
            }
        }

        public int NumberOfUnresolvedBlockersDifference
        {
            get
            {
                if (OriginalDataset == null || ResolvedDataset == null)
                    return 0;

                return ResolvedDataset.NumberOfUnresolvedBlockers - OriginalDataset.NumberOfUnresolvedBlockers;
            }
        }

        public int NumberOfUnblockedTestcasesDifference
        {
            get
            {
                if (OriginalDataset == null || ResolvedDataset == null)
                    return 0;

                return ResolvedDataset.NumberOfUnblockedTestcases - OriginalDataset.NumberOfUnblockedTestcases;
            }
        }

        public int NumberOfBlockedTestcasesDifference
        {
            get
            {
                if (OriginalDataset == null || ResolvedDataset == null)
                    return 0;

                return ResolvedDataset.NumberOfBlockedTestcases - OriginalDataset.NumberOfBlockedTestcases;
            }
        }
    }
}
