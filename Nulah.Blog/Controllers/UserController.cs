using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Core.Email;
using Nulah.Blog.Models;
using Nulah.LazyCommon.Core.MSSQL;
using Nulah.LazyCommon.Core.UserManagement;

namespace Nulah.Blog.Controllers {

    public class UserController {

        private readonly LazyMapper _lazySql;
        private readonly AppSettings _appSettings;

        public UserController(AppSettings appSettings, LazyMapper lazySql) {
            _appSettings = appSettings;
            _lazySql = lazySql;
        }

        public void PreRegisterUser(string EmailAddress) {
            var emailer = new Emailer(_appSettings.SendGridApiKey);
            var tokenHelper = new TokenGenerator();
            var registrationToken = tokenHelper.GenerateRegistrationToken();

            string RegistrationLink = $"{_appSettings.DomainBaseUrl}/ConfirmRegistration?t={registrationToken}";

            emailer.SendEmail(
                ToEmailAddress: new Email {
                    Email = EmailAddress
                },
                FromEmailAddress: new Email {
                    Email = "noreply@moar.ws",
                    Name = "Moar.ws Mailer"
                },
                Subject: "New User Registration - Confirm Email Address",
                HtmlContent: $@"<strong>No fancy looking emails from us</strong><br/>
Just click this link: <a href=""{RegistrationLink}"">{RegistrationLink}</a> and you'll be able to continue the registration process!<br/>
Can't click the link? Copy and paste this link into your browser to get their manually: {RegistrationLink}",
                PlainTextContent: $"Click or copy the link into your browser to complete your registration: {RegistrationLink}"
            );

            _lazySql.Query($"INSERT INTO [dbo].[PendingRegistrations] ([TokenId],[EmailAddress])" +
                $"VALUES " +
                $"('{registrationToken}','{EmailAddress}')")
                .Commit();
        }
    }
}
