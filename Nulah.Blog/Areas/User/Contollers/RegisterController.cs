using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Controllers;
using Nulah.Blog.Core.Email;
using Nulah.Blog.Filters;
using Nulah.Blog.Models;
using Nulah.LazyCommon.Core.MSSQL;
using Nulah.LazyCommon.Core.UserManagement;

namespace Nulah.Blog.Areas.User.Contollers {
    [Area("User")]
    public class RegisterController : Controller {

        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RegisterController(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        [Route("~/Register")]
        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [Route("~/Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void DoRegistration([FromForm]FormData formData) {

            var userController = new UserController(_appSettings, _lazySql);

            userController.PreRegisterUser(formData.EmailAddress);
        }

        [Route("~/ConfirmRegistration")]
        public IActionResult CompleteRegistration(string t) {
            throw new NotImplementedException();
        }

    }
    public class FormData {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
