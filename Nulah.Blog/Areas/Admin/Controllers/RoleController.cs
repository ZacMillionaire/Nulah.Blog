using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Models;
using Nulah.LazyCommon.Core.MSSQL;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nulah.Blog.Areas.Admin.Controllers {
    [Area("Admin")]
    public class RoleController : Controller {
        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RoleController(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        [HttpGet]
        [Route("~/Admin/Roles")]
        public IActionResult Index() {
            return View();
        }
    }
}
