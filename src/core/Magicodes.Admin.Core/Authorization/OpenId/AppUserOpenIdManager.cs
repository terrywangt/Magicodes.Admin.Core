using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;

namespace Magicodes.Admin.Authorization.OpenId
{
    public class AppUserOpenIdManager : IAppUserOpenIdManager, ITransientDependency
    {
        private readonly IRepository<AppUserOpenId, long> _appUserOpenIdRepository;
        public IAbpSession AbpSession { get; set; }
        public AppUserOpenIdManager(IRepository<AppUserOpenId, long> appUserOpenIdRepository)
        {
            _appUserOpenIdRepository = appUserOpenIdRepository;
            AbpSession = NullAbpSession.Instance;
        }
        public async Task<string> GetOpenId(OpenIdPlatforms from)
        {
            var user = await _appUserOpenIdRepository.FirstOrDefaultAsync(p => p.UserId == AbpSession.GetUserId() && p.From == from);
            return user?.OpenId;
        }
    }
}
