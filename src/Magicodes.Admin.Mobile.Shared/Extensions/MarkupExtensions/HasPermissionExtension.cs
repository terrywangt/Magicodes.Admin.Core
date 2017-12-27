using System;
using Magicodes.Admin.Core;
using Magicodes.Admin.Core.Dependency;
using Magicodes.Admin.Services.Permission;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Magicodes.Admin.Extensions.MarkupExtensions
{
    [ContentProperty("Text")]
    public class HasPermissionExtension : IMarkupExtension
    {
        public string Text { get; set; }
        
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ApplicationBootstrapper.AbpBootstrapper == null || Text == null)
            {
                return false;
            }

            var permissionService = DependencyResolver.Resolve<IPermissionService>();
            return permissionService.HasPermission(Text);
        }
    }
}