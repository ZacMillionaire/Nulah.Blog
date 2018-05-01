using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Controllers;
using Nulah.Blog.Filters;
using Nulah.Blog.Models;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;

namespace Nulah.Blog.Areas.Admin.Controllers {
    [Area("Admin")]
    [RoleFilter("7A52EF09-D1B2-409B-9D9E-55D73A769B1F")]
    public class AdminController : Controller {
        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public AdminController(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        [Route("~/Admin")]
        [HttpGet]
        public IActionResult Index() {
            //var userController = new UserController(_appSettings, _lazySql);
            //ViewData.Add("UserRoles", userController.GetUserRoles(( (PublicUser)ViewData["User"] ).InternalId));

            return View();
        }
    }
}
