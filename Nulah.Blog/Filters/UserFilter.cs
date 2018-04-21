using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {
    public class UserFilter : IActionFilter {

        private readonly LazyMapper _lazyMapper;

        public UserFilter(LazyMapper lm) {
            _lazyMapper = lm;
        }

        public void OnActionExecuted(ActionExecutedContext context) {
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            var ViewData = ( context.Controller as Controller ).ViewData;
            var UserData = new PublicUser();

            if(context.HttpContext.User.Identity.IsAuthenticated) {
                var user = context.HttpContext.User;

            } else {
                ViewData.Add("User", UserData);
            }
        }
    }
}
