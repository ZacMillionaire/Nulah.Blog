using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nulah.Blog.Areas.FrontPage.Controllers {
    [Area("FrontPage")]
    public class IndexController : Controller {

        [Route("~/")]
        public IActionResult FrontPage() {
            return View();
        }
    }
}
