using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSample.Pages
{
    public class NoTenantModel : PageModel
    {
        private readonly ILogger<NoTenantModel> _logger;

        public NoTenantModel(ILogger<NoTenantModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
