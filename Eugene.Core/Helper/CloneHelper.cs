using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Eugene.Core.Helper
{
    public static class CloneHelper
    {
        public static object GetDeepClone(object obj)
        {
            var t = obj.GetType();
            string json = JsonSerializer.Serialize(obj);
            //var copy = JsonSerializer.Deserialize<T>(json);
            var copy = JsonSerializer.Deserialize(json, obj.GetType());
            return copy;
        }
    }
}
