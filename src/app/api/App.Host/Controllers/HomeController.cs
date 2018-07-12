namespace App.Host.Controllers
{
    public class HomeController : AbpController
    {
        [DisableAuditing]
        public IActionResult Index()
        {
            //跳转到接口文档
            return Redirect("/swagger");
        }
    }
}
