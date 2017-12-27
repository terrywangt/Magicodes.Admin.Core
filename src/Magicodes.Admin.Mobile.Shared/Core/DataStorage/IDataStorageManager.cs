using System.Threading.Tasks;

namespace Magicodes.Admin.Core.DataStorage
{
    public interface IDataStorageManager
    {
        bool HasKey(string key);

        T Retrieve<T>(string key, T defaultValue = default(T), bool shouldDecrpyt = false);

        Task StoreAsync<T>(string key, T value, bool shouldEncrypt = false);

        void RemoveIfExists(string key);
    }
}