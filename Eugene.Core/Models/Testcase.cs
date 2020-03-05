using System;
using System.Collections.Generic;

namespace Eugene.Core.Models
{
    public class Testcase : IEquatable<Testcase>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TestType { get; set; }
        public string ApplicationModule { get; set; }
        //public double Weight { get; set; } = 1;
        public IList<string> BlockerIds { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"Testcase '{Name}'";
        }
        public bool Equals(Testcase other)
        {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }
        public override bool Equals(object obj) => Equals(obj as Testcase);
        public static bool operator ==(Testcase obj1, Testcase obj2)
        {
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Testcase obj1, Testcase obj2)
        {
            return !(obj1 == obj2);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
