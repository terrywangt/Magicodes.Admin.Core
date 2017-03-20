using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;
using Magicodes.Admin.EntityFramework;

namespace Magicodes.Admin.Migrations.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly AdminDbContext _context;

        public DefaultSettingsCreator(AdminDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "wenqiang.li@xin-lai.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "Test");

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "zh-CN");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}