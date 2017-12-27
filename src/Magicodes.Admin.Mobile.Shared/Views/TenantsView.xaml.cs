using Magicodes.Admin.Models.Tenants;
using Magicodes.Admin.ViewModels;
using Xamarin.Forms;

namespace Magicodes.Admin.Views
{
    public partial class TenantsView : ContentPage, IXamarinView
    {
        public TenantsView()
        {
            InitializeComponent();
        }

        private async void ListView_OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await ((TenantsViewModel)BindingContext).LoadMoreTenantsIfNeedsAsync(e.Item as TenantListModel);
        }
    }
}