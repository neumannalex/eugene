using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eugene.Core.Helper
{
    public static class IOOperations
    {
        public static async Task SaveAsync<T>(string filename, T obj)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            using (FileStream fs = File.Create(filename))
            {
                await JsonSerializer.SerializeAsync(fs, obj, jsonOptions);
            }
        }

        public static async Task<T> LoadAsync<T>(string filename)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                var obj = await JsonSerializer.DeserializeAsync<T>(fs);
                return obj;
            }
        }
    }
}
