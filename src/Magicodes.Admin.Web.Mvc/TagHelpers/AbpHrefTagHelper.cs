using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicodes.Admin.Web.TagHelpers
{
    [HtmlTargetElement("link")]
    public class AbpLinkHrefTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (output.Attributes["abp-href"] == null)
            {
                return;
            }

            var href = output.Attributes["abp-href"].Value.ToString();

            var env = IocManager.Instance.Resolve<IHostingEnvironment>();

            if (env.IsDevelopment())
            {
                output.Attributes.SetAttribute("href", href);
            }
            else
            {
                output.Attributes.SetAttribute("href", href.Replace(".css", ".min.css"));
            }

            IocManager.Instance.Release(env);
        }
    }
}
