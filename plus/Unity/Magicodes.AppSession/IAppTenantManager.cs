using Abp;
using Abp.Dependency;

namespace Magicodes.AppSession
{
    /// <summary>
    /// App租户管理器
    /// </summary>
    public interface IAppTenantManager : IShouldInitialize, ITransientDependency
    {
        int GetTenantId();
    }
}