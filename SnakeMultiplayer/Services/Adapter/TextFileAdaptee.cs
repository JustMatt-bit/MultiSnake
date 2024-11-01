using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SnakeMultiplayer.Services.Adapter
{
    public class TextFileAdaptee
    {
        private readonly string _delimiter;
        private readonly Encoding _fileEncoding;

        public TextFileAdaptee(string delimiter = ":", Encoding fileEncoding = null)
        {
            _delimiter = delimiter;
            _fileEncoding = fileEncoding ?? Encoding.UTF8;
        }

        public async Task AppendTextDataAsync(string path, string data)
        {
            await File.AppendAllTextAsync(path, data + Environment.NewLine, _fileEncoding);
        }

        public async Task<string[]> ReadAllTextDataAsync(string path)
        {
            return await File.ReadAllLinesAsync(path, _fileEncoding);
        }
    }
}
