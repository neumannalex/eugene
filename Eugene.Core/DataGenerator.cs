using Eugene.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core
{
    public static class DataGenerator
    {
        public static TestcaseBlockerDataset GenerateRandomData(int numberOfBlockers, int numberOfTestcases)
        {
            var blockers = new List<Blocker>();
            var testcases = new List<Testcase>();

            var rnd = new Random();

            // Generate Blockers
            for (int i = 1; i <= numberOfBlockers; i++)
            {
                blockers.Add(new Blocker
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Blocker {i}"
                });
            }

            // Generate Testcases
            for (int i = 1; i <= numberOfTestcases; i++)
            {
                var testcase = new Testcase
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Testcase {i}"
                };

                var numberOfBlockersForTestcase = rnd.Next(0, (int)Math.Ceiling(0.5 * numberOfBlockers));
                while (testcase.BlockerIds.Count < numberOfBlockersForTestcase)
                {
                    var blockerId = blockers[rnd.Next(0, numberOfBlockers)].Id;
                    if (!testcase.BlockerIds.Contains(blockerId))
                        testcase.BlockerIds.Add(blockerId);
                }

                testcases.Add(testcase);
            }

            TestcaseBlockerDataset configuration = new TestcaseBlockerDataset
            {
                Blockers = blockers,
                Testcases = testcases
            };

            return configuration;
        }
    }
}
