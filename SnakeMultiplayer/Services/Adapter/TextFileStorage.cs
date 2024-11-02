using System;
using System.Threading.Tasks;
using System.IO;

namespace SnakeMultiplayer.Services.Adapter
{
    public class TextFileStorage : IFileStorage
    {
        public TextFileStorage()
        {
        }

        public async Task SaveDataAsync(string path, object data)
        {
            await File.AppendAllTextAsync(path, data + Environment.NewLine);
        }

        public async Task<string[]> GetDataAsync(string path)
        {
            return await File.ReadAllLinesAsync(path);
        }
    }
}
