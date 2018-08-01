using Abp;
using Abp.Dependency;
using Magicodes.Storage.Core;

namespace Magicodes.Unity.Storage
{
    /// <summary>
    /// 存储管理程序
    /// </summary>
    public interface IStorageManager: ISingletonDependency, IShouldInitialize
    {
        /// <summary>
        /// 存储提供程序
        /// </summary>
        IStorageProvider StorageProvider { get; set; }
    }
}