using System;
using System.Threading.Tasks;

namespace SnakeMultiplayer.Services.Adapter
{
    public interface IFileStorage
    {
        Task SaveDataAsync(string path, object data);
        Task<string[]> GetDataAsync(string path);
    }
}
