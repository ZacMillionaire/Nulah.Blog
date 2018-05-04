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
    [LoginFilter(UserRole.LoggedOut)]
    public class RegisterController : Controller {

        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RegisterController(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        [Route("~/Register")]
        [HttpGet]
        //[RoleFilter(UserRole.LoggedOut)]
        public IActionResult Register() {
            return View();
        }

        [Route("~/Register")]
        [HttpPost]
        //[RoleFilter(UserRole.LoggedOut)]
        [ValidateAntiForgeryToken]
        public void DoRegistration([FromForm]RegistrationFormData formData) {
            try {
                UserController userController = new UserController(_appSettings, _lazySql);
                userController.PreRegisterUser(formData.EmailAddress);

                Response.Redirect("/Register/InProgress");
            } catch(Exception e) {
                Response.Redirect($"/Register/Error?Reason={e.GetBaseException().Message}");
            }
        }

        [HttpGet]
        [Route("~/Register/InProgress")]
        public IActionResult InProgressRegistration() {
            return View();
        }

        [HttpGet]
        [Route("~/Register/Error")]
        public IActionResult RegistrationError(string Reason) {
            ViewData["Reason"] = Reason;
            return View();
        }

        [HttpGet]
        [Route("~/Register/Confirm")]
        //[RoleFilter(UserRole.LoggedOut)]
        public IActionResult ConfirmRegistration(string t) {

            if(string.IsNullOrWhiteSpace(t)) {
                return RedirectToAction("Register");
            }

            ViewData.Add("Token", t);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("~/Register/Confirm")]
        public IActionResult FinishRegistration([FromForm]VerifyEmailFormData formData) {
            UserController userController = new UserController(_appSettings, _lazySql);
            var registered = userController.CompleteUserRegistration(formData.EmailCode, formData.UniqueCode);
            if(registered) {
                return View("RegistrationSuccess");
            } else {
                return RedirectToAction("RegistrationError", new Dictionary<string, object> { { "Reason", "RegistrationFailed" } });
            }
        }

    }

    public class RegistrationFormData {
        [EmailAddress]
        public string EmailAddress { get; set; }
    }

    public class VerifyEmailFormData {
        public string EmailCode { get; set; }
        public string UniqueCode { get; set; }
    }
}
