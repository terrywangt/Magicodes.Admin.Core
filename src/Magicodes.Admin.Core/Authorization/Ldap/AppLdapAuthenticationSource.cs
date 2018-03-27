using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;

namespace Magicodes.Admin.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}