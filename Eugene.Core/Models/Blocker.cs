using System;
using System.Collections.Generic;
using System.Text;

namespace Eugene.Core.Models
{
    public class Blocker : IEquatable<Blocker>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; } = 1;

        public override string ToString()
        {
            return $"Blocker '{Name}'";
        }
        public bool Equals(Blocker other)
        {
            if (other is null)
                return false;

            return this.Id == other.Id;
        }
        public override bool Equals(object obj) => Equals(obj as Blocker);
        public static bool operator ==(Blocker obj1, Blocker obj2)
        {
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Blocker obj1, Blocker obj2)
        {
            return !(obj1 == obj2);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
