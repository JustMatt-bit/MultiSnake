using System;
using System.Threading.Tasks;
using System.IO;

namespace SnakeMultiplayer.Services.Adapter
{
    public class TextFileAdapter : IFileStorage
    {
        private readonly TextFileAdaptee _textFileAdaptee;

        public TextFileAdapter(string delimiter = ":")
        {
            _textFileAdaptee = new TextFileAdaptee(delimiter);
        }

        public async Task SaveDataAsync(string path, object data)
        {
            if (data is string textData)
            {
                await _textFileAdaptee.AppendTextDataAsync(path, textData);
            }
            else
            {
                throw new InvalidOperationException("TextFileAdapter expects data as a string.");
            }
        }

        public async Task<string[]> GetDataAsync(string path)
        {
            return await _textFileAdaptee.ReadAllTextDataAsync(path);
        }
    }
}
