using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nulah.Blog.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {
    public class RoleFilter : ActionFilterAttribute {

        private readonly Guid _shouldBe;
        private readonly string _redirect = "~/";

        public RoleFilter(string RoleShouldBe) {
            _shouldBe = Guid.Parse(RoleShouldBe);
        }

        public override void OnActionExecuting(ActionExecutingContext context) {

            if(context.HttpContext.User.Identity.IsAuthenticated == false) {
                context.Result = new LocalRedirectResult("~/Login");
                return;
            }


            var ViewData = ( context.Controller as Controller ).ViewData;
            PublicUser UserData = (PublicUser)ViewData["User"];

            if(UserData.Roles.Contains(_shouldBe) == false) {
                context.Result = new LocalRedirectResult(_redirect);
                return;
            }
        }
    }
}
