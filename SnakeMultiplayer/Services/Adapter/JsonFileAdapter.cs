using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnakeMultiplayer.Services.Adapter
{
    public class JsonFileAdapter : IFileStorage
    {
        private readonly JsonFileAdaptee _jsonFileAdaptee;

        public JsonFileAdapter(bool prettyPrint = false)
        {
            _jsonFileAdaptee = new JsonFileAdaptee(prettyPrint);
        }

        public async Task SaveDataAsync(string path, object data)
        {
            var jsonData = _jsonFileAdaptee.ConvertToDictionary(data);
            await _jsonFileAdaptee.AppendJsonDataAsync(path, jsonData);
        }

        public async Task<string[]> GetDataAsync(string path)
        {
            return await _jsonFileAdaptee.ReadJsonDataAsync(path);
        }
    }
}
