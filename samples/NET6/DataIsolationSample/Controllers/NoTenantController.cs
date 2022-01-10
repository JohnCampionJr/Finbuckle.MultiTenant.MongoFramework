using System.Linq;
using DataIsolationSample.Data;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataIsolationSample.Models;

namespace DataIsolationSample.Controllers
{
    public class NoTenantController : Controller
    {
        public NoTenantController() { }

        public IActionResult Index()
        {
            return View();
        }
    }
}
