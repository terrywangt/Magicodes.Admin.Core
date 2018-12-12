using System.Threading.Tasks;

namespace Magicodes.Admin.Authorization.OpenId
{
    public interface IAppUserOpenIdManager
    {
        /// <summary>
        /// 获取当前用户的OpenId
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        Task<string> GetOpenId(OpenIdPlatforms from);
    }
}
