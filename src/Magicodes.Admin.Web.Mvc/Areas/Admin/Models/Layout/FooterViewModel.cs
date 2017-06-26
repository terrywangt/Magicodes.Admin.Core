using Magicodes.Admin.Sessions.Dto;

namespace Magicodes.Admin.Web.Areas.Admin.Models.Layout
{
    public class FooterViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public string GetProductNameWithEdition()
        {
            var productName = "Magicodes.Admin";

            if (LoginInformations.Tenant?.Edition.DisplayName != null)
            {
                productName += " " + LoginInformations.Tenant.Edition.DisplayName;
            }

            return productName;
        }
    }
}