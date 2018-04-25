using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {

    public enum UserRole {
        LoggedIn,
        LoggedOut
    }

    public class LoginFilter : ActionFilterAttribute {

        //private readonly LazyMapper _lazyMapper;
        //
        //public RoleFilter(LazyMapper lm) {
        //    _lazyMapper = lm;
        //}

        private readonly UserRole _shouldBe;
        private readonly string _redirect = "~/";

        public LoginFilter(UserRole ShouldBe) {
            _shouldBe = ShouldBe;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            if(_shouldBe == UserRole.LoggedOut && context.HttpContext.User.Identity.IsAuthenticated == true) {
                context.Result = new LocalRedirectResult(_redirect);
            } else if(_shouldBe == UserRole.LoggedIn && context.HttpContext.User.Identity.IsAuthenticated == false) {
                context.Result = new LocalRedirectResult("~/Login");
            }
        }
    }
}
