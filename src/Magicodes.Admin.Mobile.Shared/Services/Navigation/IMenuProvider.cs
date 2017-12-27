using System.Collections.Generic;
using MvvmHelpers;
using Magicodes.Admin.Models.NavigationMenu;

namespace Magicodes.Admin.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}