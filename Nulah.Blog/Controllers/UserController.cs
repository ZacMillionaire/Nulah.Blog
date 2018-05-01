using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Core.Email;
using Nulah.Blog.Models;
using Nulah.Blog.Models.Public;
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

            var emailAlreadyRegistered = EmailAddressRegistered(EmailAddress);

            if(emailAlreadyRegistered) {
                throw new Exception("EmailExists");
            }


            var emailer = new Emailer(_appSettings.SendGridApiKey);
            var tokenHelper = new TokenGenerator();
            var registrationToken = tokenHelper.GenerateRegistrationToken();
            var uniqueToken = tokenHelper.GenerateUniqueToken(16);

            string RegistrationLink = $"{_appSettings.DomainBaseUrl}/Register/Confirm?t={registrationToken}";

            // TODO: Rip out the hard coded email stuff and shove them back into configurable template files like
            // I used to have.
            emailer.SendEmail(
                ToEmailAddress: new Email {
                    Email = EmailAddress
                },
                FromEmailAddress: new Email {
                    Email = "newuser+noreply@moar.ws",
                    Name = "Moar.ws Mailer"
                },
                Subject: "New User Registration - Confirm Email Address",
                HtmlContent: $@"<div style=""text-align:center; width: 100%"">
                    <p>
                        <h1>No fancy looking emails from us</h1>
                    </p>
                    <h2>Just click <a href=""{RegistrationLink}"">This Link</a></h2>
                    <p>
                        ...and enter this code...
                    </p>
                    <h2>{uniqueToken}</h2>
                    <p>
                        ...and you'll be able to continue the registration process!
                    </p>
                    <h3>Can't click the link?</h3>
                    <p>Copy and paste this link into your browser to get there manually (Don't forget to copy the code above afterwards as well!)</p>
                    <p>{RegistrationLink}</p>
                </div>",
                PlainTextContent: $"Click or copy the link into your browser to complete your registration: {RegistrationLink}"
            );

            _lazySql.StoredProcedure($"CreateOrUpdatePendingRegistration")
                .WithParameters(new Dictionary<string, object> {
                    {"@TokenId",registrationToken },
                    {"@UniqueCode",uniqueToken },
                    {"@EmailAddress",EmailAddress }
                })
                .Commit();
        }

        public bool EmailAddressRegistered(string EmailAddress) {
            bool emailAlreadyRegistered = (bool)_lazySql.StoredProcedure("UserExistsWithEmailAddress")
                .WithParameters(new Dictionary<string, object> {
                                { "@EmailAddress",EmailAddress }
                })
                .Result()
                .First()["EmailExists"].Value;
            return emailAlreadyRegistered;
        }

        public bool CompleteUserRegistration(string EmailToken, string UniqueCode) {

            var tokenDict = new Dictionary<string, object> {
                    { "@TokenId", EmailToken },
                    { "@UniqueCode", UniqueCode }
                };

            bool verified = (bool)_lazySql.StoredProcedure("ValidateEmailFromRegistration")
                .WithParameters(tokenDict)
                .Result()
                .First()["Valid"].Value;

            if(verified) {
                string emailAddress = (string)_lazySql.StoredProcedure("GetEmailFromPendingRegistrations")
                    .WithParameters(tokenDict)
                    .Result()
                    .First()["EmailAddress"].Value;

                _lazySql.StoredProcedure("CreateBlankProfile")
                    .WithParameters(new Dictionary<string, object> {
                        { "@EmailAddress",emailAddress },
                        { "@InternalId", Guid.NewGuid() }
                    })
                    .Commit();

                _lazySql.StoredProcedure("DeleteFromPending")
                    .WithParameters(new Dictionary<string, object> {
                        {"@EmailAddress",emailAddress }
                    })
                    .Commit();

                return true;
            } else {
                return false;
            }
        }

        public void SendLoginEmail(string EmailAddress) {
            var emailAddressRegistered = EmailAddressRegistered(EmailAddress);

            if(emailAddressRegistered == false) {
                throw new Exception("EmailNotFound");
            }

            var tokenHelper = new TokenGenerator();
            var passwordHelper = new PasswordManagement(tokenHelper);
            var otp = passwordHelper.GenerateOneTimePassword();

            try {
                _lazySql.StoredProcedure("SetUserOneTimePasswordHash")
                    .WithParameters(new Dictionary<string, object> {
                    {"@EmailAddress",EmailAddress },
                    {"@PasswordHash",otp.Hash }
                    })
                    .Commit();

                var emailer = new Emailer(_appSettings.SendGridApiKey);

                string loginLink = $"{_appSettings.DomainBaseUrl}/Login/FromEmail";

                // TODO: Rip out the hard coded email stuff and shove them back into configurable template files like
                // I used to have.
                emailer.SendEmail(
                    ToEmailAddress: new Email {
                        Email = EmailAddress
                    },
                    FromEmailAddress: new Email {
                        Email = "login+noreply@moar.ws",
                        Name = "Moar.ws Mailer"
                    },
                    Subject: "Login Request - One Time Password",
                    HtmlContent: $@"<div style=""text-align:center; width: 100%"">
                    <p>
                        <h1>Your One Time Password</h1>
                    </p>
                    <p>
                        Hey there!
                    </p>
                    <p>
                        You (or someone else) has requested a one time password to log in to moar.ws!
                    </p>
                    <p>
                        Wasn't you? Feel free to ignore this email then, as whoever it was can't do anything without the code below.
                    </p>
                    <p>
                        Was you? Use the following code as your password to login to your account (you'll need to enter your email address again) <a href=""{loginLink}"">here</a>
                    </p>
                    <p>
                        Your one time password is:
                    </p>
                    <h2>{otp.Password}</h2>
                    <p>
                        This single use password will remain valid until you have used it to log in, or another login request is made.
                    </p>
                </div>",
                    PlainTextContent: $"Your one time password is: {otp.Password}"
                );
            } catch(Exception e) {
                throw new Exception($"An error occured sending a login email", e);
            }
        }

        public Guid Login(string EmailAddress, string OneTimePassword, string UserAgent) {
            var emailAddressRegistered = EmailAddressRegistered(EmailAddress);

            if(emailAddressRegistered == false) {
                throw new Exception("EmailNotFound");
            }

            var tokenHelper = new TokenGenerator();
            var passwordHelper = new PasswordManagement(tokenHelper);
            var otpHash = passwordHelper.HashFromPassword(OneTimePassword);

            var validPasswordForEmail = (bool)_lazySql.StoredProcedure("ValidateUserPasswordHash")
                .WithParameters(new Dictionary<string, object> {
                    {"@EmailAddress",EmailAddress },
                    {"@PasswordHash",otpHash }
                })
                .Result()
                .First()["Valid"].Value;

            if(validPasswordForEmail == false) {
                throw new Exception("BadPassword");
            }

            var sessionToken = Guid.NewGuid();

            _lazySql.StoredProcedure("SetUserSessionAndDeletePassword")
                .WithParameters(new Dictionary<string, object> {
                    {"@EmailAddress",EmailAddress },
                    {"@SessionId",sessionToken },
                    {"@UserAgent",UserAgent },
                    {"@LastSeen",DateTime.UtcNow }
                })
                .Commit();

            return sessionToken;
        }

        public void Logout(string SessionId) {
            Guid sessionToken = Guid.Parse(SessionId);

            _lazySql.Query($"DELETE FROM [dbo].[UserSessions] WHERE [SessionId] = '{SessionId}'")
                .Commit();


        }

        public PublicRoleDetails[] GetUserRoles(Guid InternalUserId) {
            var userRoles = _lazySql.StoredProcedure("GetExpandedAllRolesForUser")
                .WithParameters(new Dictionary<string, object> {
                    {"@UserInternalId",InternalUserId }
                })
                .Result()
                ?.Select(x => new PublicRoleDetails {
                    Description = x["Description"].Value as string,
                    Id = (Guid)x["RoleId"].Value,
                    Name = x["Name"].Value as string,
                    Source = x["Source"].Value as string
                })
                .ToArray();

            return userRoles;
        }
    }
}
