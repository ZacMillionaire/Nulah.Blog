using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nulah.Blog.Models;
using Nulah.LazyCommon.Core.MSSQL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Nulah.Blog.Filters;
using Nulah.Blog.Controllers;

namespace Nulah.Blog {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            var settings = new AppSettings() {
                MSSQLConnectionString = Configuration.GetSection("MSSQL:ConnectionString").Get<string>(), // TODO: make app settings binding here instead of this
                SendGridApiKey = Configuration.GetSection("ApiKeys:SendGrid").Get<string>(),
                DomainBaseUrl = Configuration.GetSection("DomainBaseUrl").Get<string>()
            };

            Console.WriteLine("Testing Connection string...");

            if(DbHelper.TestConnection(settings.MSSQLConnectionString) == false) {
                throw new ApplicationException("Unable to connect to database with the given connection string.");
            }

            LazyMapper lm = new LazyMapper(settings.MSSQLConnectionString);
            SessionManager sc = new SessionManager(settings, lm);

            Console.WriteLine("Able to connect to database.");
            services.AddScoped(_ => lm);
            services.AddScoped(_ => sc);


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options => options = new CookieAuthenticationOptions {
                   LoginPath = new PathString("/Login"),
                   LogoutPath = new PathString("/Logout"),
                   AccessDeniedPath = "/",
                   //ExpireTimeSpan = new TimeSpan(30, 0, 0, 0),
                   //SlidingExpiration = true
               });

            services.AddSingleton(settings);

            services.AddMvc()
                .AddMvcOptions(options => {
                    options.Filters.Add(new UserFilter(lm, sc));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // Needs to be as close to the start of the pipeline as possible to ensure it can rewrite any errors later on
            // Specifically it's probably going to be the 500 ScreamingExceptions throws on an internal error.
            // See: https://andrewlock.net/re-execute-the-middleware-pipeline-with-the-statuscodepages-middleware-to-create-custom-error-pages/
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseScreamingExceptions();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
