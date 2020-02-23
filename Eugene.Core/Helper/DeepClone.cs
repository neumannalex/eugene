using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Eugene.Core.Helper
{
    public static class DeepClone
    {
        public static T GetDeepClone<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            var copy = JsonSerializer.Deserialize<T>(json);
            return copy;
        }
    }
}
