using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnakeMultiplayer.Services.Adapter
{
    public class JsonFileAdaptee
    {
        private readonly JsonSerializerOptions _options;

        public JsonFileAdaptee(bool prettyPrint = false)
        {
            _options = new JsonSerializerOptions { WriteIndented = prettyPrint };
        }

        public async Task AppendJsonDataAsync(string path, Dictionary<string, string> newData)
        {
            Dictionary<string, string> existingData;

            if (File.Exists(path) && new FileInfo(path).Length > 0)
            {
                // Read and deserialize existing JSON data
                var existingJson = await File.ReadAllTextAsync(path);
                existingData = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson)
                               ?? new Dictionary<string, string>();
            }
            else
            {
                existingData = new Dictionary<string, string>();
            }

            foreach (var entry in newData)
            {
                existingData[entry.Key] = entry.Value; // Update or add new entries
            }

            var jsonData = JsonSerializer.Serialize(existingData, _options);
            await File.WriteAllTextAsync(path, jsonData);
        }

        public Dictionary<string, string> ConvertToDictionary(object data)
        {
            Dictionary<string, string> jsonData = new();

            // Convert input data into a dictionary format
            switch (data)
            {
                case KeyValuePair<string, string> kvp:
                    jsonData[kvp.Key] = kvp.Value;
                    break;
                case string entry when entry.Contains(":"):
                    var parts = entry.Split(':');
                    jsonData[parts[0]] = parts[1];
                    break;
                default:
                    throw new InvalidOperationException("Unsupported data format for JsonFileAdapter.");
            }

            return jsonData;
        }

        public async Task<string[]> ReadJsonDataAsync(string path)
        {
            var lines = await File.ReadAllTextAsync(path);
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, string>>(lines);
            var nameHashArray = new List<string>();
            foreach (var kvp in jsonData)
            {
                nameHashArray.Add($"{kvp.Key}:{kvp.Value}");
            }

            return nameHashArray.ToArray();
        }
    }
}
