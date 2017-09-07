using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Magicodes.Admin.Web.Startup
{
    public class HiddenApiFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            //请自行编写相关API隐藏逻辑
            //foreach (var group in context.ApiDescriptionsGroups.Items)
            //{
            //    foreach (var api in group.Items)
            //    {
            //        if (!api.RelativePath.Contains("/tms/"))
            //        {
            //            swaggerDoc.Paths.Remove("/" + api.RelativePath);
            //        }
            //    }
            //}
        }
    }
}