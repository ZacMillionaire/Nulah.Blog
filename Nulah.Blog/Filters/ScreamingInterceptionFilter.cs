using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Filters {
    public class ScreamingInterceptionFilter : ExceptionFilterAttribute {

        private readonly RequestDelegate _next;

        public ScreamingInterceptionFilter(RequestDelegate Next) {
            _next = Next;
        }

        //public ScreamingInterceptionFilter(RequestDelegate Next, ILoggerFactory LogFactory, IDatabase Redis) {
        //    _next = Next;
        //    _logFactory = LogFactory;
        //    _logger = LogFactory.CreateLogger<ScreamingInterceptionFilter>();
        //    _redis = Redis;
        //}

        public async Task Invoke(HttpContext Context) {
            try {
                await _next.Invoke(Context);
            } catch(Exception e) {
                // set status code and redirect to error page
                Context.Response.StatusCode = 500;
                //Context.Response.Redirect("/Error/500");
            }
        }
    }

    public static class ScreamingInterceptionExtention {
        public static IApplicationBuilder UseScreamingExceptions(this IApplicationBuilder Builder) {
            return Builder.UseMiddleware<ScreamingInterceptionFilter>();
        }
    }

}