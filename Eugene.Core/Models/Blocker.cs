using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.Models
{
    public class Blocker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; } = 1;
    }
}
