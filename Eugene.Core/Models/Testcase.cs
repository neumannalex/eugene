using System;
using System.Collections.Generic;

namespace Eugene.Core.Models
{
    public class Testcase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; } = 1;
        public IList<string> BlockerIds { get; set; } = new List<string>();
    }
}
