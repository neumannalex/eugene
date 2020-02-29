using Eugene.Core;
using Eugene.Core.Helper;
using Eugene.Core.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NUnitTestProject1
{
    public class Tests
    {
        private TestcaseBlockerDataset GetKnownDataset()
        {
            //            Blockers
            //      1 | 2 | 3 | 4 | 5 | 
            // T 1  x |   | x |   |   | 2
            // e 2    | x |   |   |   | 1
            // s 3    |   |   |   | x | 1
            // t 4    |   |   |   |   | 0
            // s 5  x |   | x |   | x | 3
            //   6    |   | x |   |   | 1
            //   7    | x |   |   |   | 1
            //   8    |   | x |   | x | 2
            //   9    |   | x |   |   | 1
            //     ----------------------
            //      2 | 2 | 5 | 0 | 3 |

            //            Blockers
            //      1 | 2 | 3 | 4 | 5 | 
            // T 1  o |   | x |   |   | 1
            // e 2    | o |   |   |   | 0
            // s 3    |   |   |   | x | 1
            // t 4    |   |   |   |   | 0
            // s 5  o |   | x |   | x | 2
            //   6    |   | x |   |   | 1
            //   7    | o |   |   |   | 0
            //   8    |   | x |   | x | 2
            //   9    |   | x |   |   | 1
            //     ----------------------
            //      0 | 0 | 5 | 0 | 3 |

            var blockers = new List<Blocker>
            {
                new Blocker {Id = "1", Name = "Blocker 1", Cost = 1},
                new Blocker {Id = "2", Name = "Blocker 2", Cost = 1},
                new Blocker {Id = "3", Name = "Blocker 3", Cost = 1},
                new Blocker {Id = "4", Name = "Blocker 4", Cost = 1},
                new Blocker {Id = "5", Name = "Blocker 5", Cost = 1}
            };

            var testcases = new List<Testcase>
            {
                new Testcase {Id = "1", Name = "Testcase 1", Weight = 1, BlockerIds = new List<string>{"1", "3"} },
                new Testcase {Id = "2", Name = "Testcase 2", Weight = 1, BlockerIds = new List<string>{"2"} },
                new Testcase {Id = "3", Name = "Testcase 3", Weight = 1, BlockerIds = new List<string>{"5"} },
                new Testcase {Id = "4", Name = "Testcase 4", Weight = 1, BlockerIds = new List<string>() },
                new Testcase {Id = "5", Name = "Testcase 5", Weight = 1, BlockerIds = new List<string>{"1", "3", "5"} },
                new Testcase {Id = "6", Name = "Testcase 6", Weight = 1, BlockerIds = new List<string>{"3"} },
                new Testcase {Id = "7", Name = "Testcase 7", Weight = 1, BlockerIds = new List<string>{"2"} },
                new Testcase {Id = "8", Name = "Testcase 8", Weight = 1, BlockerIds = new List<string>{"3", "5"} },
                new Testcase {Id = "9", Name = "Testcase 9", Weight = 1, BlockerIds = new List<string>{"3"} },
            };

            return new TestcaseBlockerDataset
            {
                Blockers = blockers,
                Testcases = testcases
            };
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EnsureDeepClonedDatasetIsEqualToOriginal()
        {
            var numBlockers = 10;
            var numTestcases = 50;

            TestcaseBlockerDataset originalDataset = DataGenerator.GenerateRandomData(numBlockers, numTestcases);

            var clonedDataset = (TestcaseBlockerDataset)CloneHelper.GetDeepClone(originalDataset);

            Assert.IsInstanceOf<TestcaseBlockerDataset>(clonedDataset);
            Assert.IsTrue(clonedDataset.Blockers.Count == numBlockers);
            Assert.IsTrue(clonedDataset.Testcases.Count == numTestcases);
        }

        [Test]
        public void EnsureDeepClonedDatasetIsDifferentInstanceThanOriginalDataset()
        {
            var numBlockers = 10;
            var numTestcases = 50;

            TestcaseBlockerDataset originalDataset = DataGenerator.GenerateRandomData(numBlockers, numTestcases);
            var clonedDataset = (TestcaseBlockerDataset)CloneHelper.GetDeepClone(originalDataset);

            var originalTestcase = originalDataset.Testcases.Where(x => x.BlockerIds.Count > 0).FirstOrDefault();
            var clonedTestcase = clonedDataset.Testcases.Where(x => x.Id == originalTestcase.Id).FirstOrDefault();

            Assert.IsTrue(originalTestcase.Name == clonedTestcase.Name);
            Assert.IsTrue(originalTestcase.BlockerIds.Count == clonedTestcase.BlockerIds.Count);

            clonedTestcase.BlockerIds.Clear();
            Assert.IsFalse(originalTestcase.BlockerIds.Count == clonedTestcase.BlockerIds.Count);
        }

        [Test]
        public void EnsureCopiedDatasetIsSameInstanceAsOriginalDataset()
        {
            var numBlockers = 10;
            var numTestcases = 50;

            TestcaseBlockerDataset originalDataset = DataGenerator.GenerateRandomData(numBlockers, numTestcases);
            var copiedDataset = originalDataset;

            var originalTestcase = originalDataset.Testcases.Where(x => x.BlockerIds.Count > 0).FirstOrDefault();
            var copiedTestcase = copiedDataset.Testcases.Where(x => x.Id == originalTestcase.Id).FirstOrDefault();

            Assert.IsTrue(originalTestcase.Name == copiedTestcase.Name);
            Assert.IsTrue(originalTestcase.BlockerIds.Count == copiedTestcase.BlockerIds.Count);

            copiedTestcase.BlockerIds.Clear();
            Assert.IsTrue(originalTestcase.BlockerIds.Count == copiedTestcase.BlockerIds.Count);
        }

        [Test]
        public void EnsureDatasetIsCorrectlyResolved()
        {
            var initialDataset = GetKnownDataset();
            var blockerIdsToResolve = new List<string> {"1", "2"};
            var blockersToResolve = initialDataset.Blockers.Where(x => blockerIdsToResolve.Contains(x.Id)).ToList();

            var resolver = new BlockerResolver(initialDataset);

            var resolvedDataset = resolver.Resolve(blockersToResolve);

            var expectedResolvedBlockerIds = new List<string> {"1", "2", "4"};
            var expectedUnresolvedBlockerIds = new List<string> {"3", "5"};

            Assert.IsTrue(resolvedDataset.ResolvedBlockers.Count == expectedResolvedBlockerIds.Count);
            Assert.IsTrue(resolvedDataset.UnresolvedBlockers.Count == expectedUnresolvedBlockerIds.Count);

            Assert.IsTrue(expectedResolvedBlockerIds.Except(resolvedDataset.ResolvedBlockers.Select(x => x.Id)).Count() == 0);
            Assert.IsTrue(expectedUnresolvedBlockerIds.Except(resolvedDataset.UnresolvedBlockers.Select(x => x.Id)).Count() == 0);
        }

        [Test]
        public void BlockerEquality()
        {
            var originalBlocker = new Blocker { Id = "1", Name = "Blocker 1", Cost = 1 };

            var referencedBlocker = originalBlocker;
            var clonedBlocker = (Blocker)CloneHelper.GetDeepClone(originalBlocker);

            Assert.AreEqual(originalBlocker, referencedBlocker);
            Assert.AreEqual(originalBlocker, clonedBlocker);
        }
    }
}