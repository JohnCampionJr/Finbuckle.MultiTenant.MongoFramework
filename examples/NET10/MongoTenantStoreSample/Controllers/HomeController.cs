using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MongoTenantStoreSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var ti = HttpContext.GetMultiTenantContext<MongoTenantInfo>()?.TenantInfo;
            return View(ti);
        }
    }
}
