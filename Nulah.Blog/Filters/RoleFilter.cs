using Microsoft.AspNetCore.Mvc.Filters;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {

    public enum UserRoles {
        isLoggedIn,
        isLoggedOut
    }

    public class RoleFilter : ActionFilterAttribute {

        //private readonly LazyMapper _lazyMapper;
        //
        //public RoleFilter(LazyMapper lm) {
        //    _lazyMapper = lm;
        //}

        public RoleFilter(UserRoles ShouldBe) { }

        public override void OnActionExecuting(ActionExecutingContext context) {
        }
    }
}
