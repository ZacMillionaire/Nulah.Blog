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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nulah.Blog.Areas.User.Contollers {
    [Area("User")]
    [LoginFilter(UserRole.LoggedIn)]
    public class ProfileController : Controller {

        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public ProfileController(LazyMapper lm, AppSettings settings) {
            _lazySql = lm;
            _appSettings = settings;
        }

        [HttpGet]
        [Route("~/Profile")]
        public IActionResult Index() {

            var userController = new UserController(_appSettings, _lazySql);
            ViewData.Add("UserRoles", userController.GetUserRoles(( (PublicUser)ViewData["User"] ).InternalId));

            return View();
        }
    }
}
