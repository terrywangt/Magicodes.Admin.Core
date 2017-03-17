using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Layout
{
    public class FooterViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public string GetProductNameWithEdition()
        {
            var productName = "Admin";

            if (LoginInformations.Tenant?.EditionDisplayName != null)
            {
                productName += " " + LoginInformations.Tenant.EditionDisplayName;
            }

            return productName;
        }
    }
}