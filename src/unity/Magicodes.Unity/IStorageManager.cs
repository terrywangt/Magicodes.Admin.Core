using Magicodes.Storage.Core;

namespace Magicodes.Unity
{
    public interface IStorageManager
    {
        IStorageProvider StorageProvider { get; set; }

        void Initialize();
    }
}