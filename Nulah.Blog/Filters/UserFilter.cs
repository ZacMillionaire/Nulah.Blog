using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nulah.Blog.Controllers;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {
    public class UserFilter : IActionFilter {

        private readonly LazyMapper _lazySql;
        private readonly SessionManager _sessionController;

        public UserFilter(LazyMapper lm, SessionManager sc) {
            _lazySql = lm;
            _sessionController = sc;
        }

        public void OnActionExecuted(ActionExecutedContext context) {
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            var ViewData = ( context.Controller as Controller ).ViewData;
            var UserData = new PublicUser();

            if(context.HttpContext.User.Identity.IsAuthenticated) {
                var user = context.HttpContext.User;
                var sessionClaim = user.Claims.First(x => x.Type == "SessionId");
                UserData = _sessionController.RefreshSession(sessionClaim.Value);
            }
            ViewData.Add("User", UserData);
        }
    }
}
