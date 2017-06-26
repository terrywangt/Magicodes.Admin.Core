using Abp.AutoMapper;
using Magicodes.Admin.MultiTenancy.Dto;

namespace Magicodes.Admin.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(EditionsSelectOutput))]
    public class EditionsSelectViewModel : EditionsSelectOutput
    {
        public EditionsSelectViewModel(EditionsSelectOutput output)
        {
            output.MapTo(this);
        }
    }
}
