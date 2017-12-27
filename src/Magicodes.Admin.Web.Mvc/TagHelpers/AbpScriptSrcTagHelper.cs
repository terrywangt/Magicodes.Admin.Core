using Abp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicodes.Admin.Web.TagHelpers
{
    [HtmlTargetElement("script")]
    public class AbpScriptSrcTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (output.Attributes["abp-src"] == null)
            {
                return;
            }

            var href = output.Attributes["abp-src"].Value.ToString();

            var env = IocManager.Instance.Resolve<IHostingEnvironment>();

            if (env.IsDevelopment())
            {
                output.Attributes.SetAttribute("src", href);
            }
            else
            {
                output.Attributes.SetAttribute("src", href.Replace(".js", ".min.js"));
            }

            IocManager.Instance.Release(env);
        }
    }
}