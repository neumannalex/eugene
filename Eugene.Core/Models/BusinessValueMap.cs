using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eugene.Core.Models
{
    public class BusinessValueMap
    {
        public Dictionary<string, double> TestTypes { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> ApplicationModules { get; set; } = new Dictionary<string, double>();

        public double GetTestTypeValue(string testtype)
        {
            var types = testtype.Split(',');
            var values = new List<double>();

            foreach(var type in types)
            {
                var trimmedType = type.Trim();
                var value = TestTypes.ContainsKey(trimmedType) ? TestTypes[trimmedType] : 1.0;
                values.Add(value);
            }

            if (values.Count > 0)
                return values.Max();
            else
                return 1.0;
        }

        public double GetApplicationModuleValue(string applicationmodule)
        {
            var modules = applicationmodule.Split(',');
            var values = new List<double>();

            foreach (var module in modules)
            {
                var trimmedModule = module.Trim();
                var value = ApplicationModules.ContainsKey(trimmedModule) ? ApplicationModules[trimmedModule] : 1.0;
                values.Add(value);
            }

            if (values.Count > 0)
                return values.Max();
            else
                return 1.0;
        }
    }
}
