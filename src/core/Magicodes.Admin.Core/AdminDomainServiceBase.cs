using Abp.Domain.Services;

namespace Magicodes.Admin
{
    public abstract class AdminDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected AdminDomainServiceBase()
        {
            LocalizationSourceName = AdminConsts.LocalizationSourceName;
        }
    }
}
