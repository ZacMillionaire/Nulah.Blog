using Nulah.Blog.Models;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Controllers {
    public class SessionManager {
        private readonly LazyMapper _lazySql;
        private readonly AppSettings _appSettings;

        public SessionManager(AppSettings appSettings, LazyMapper lazySql) {
            _appSettings = appSettings;
            _lazySql = lazySql;
        }

        public PublicUser ValidateAndGetUserDataFromSession(string SessionId) {
            var userId = _lazySql.StoredProcedure("CheckSessionAndRefreshAndReturnUserId")
                .WithParameters(new Dictionary<string, object> {
                    {"@SessionId", SessionId },
                    {"@LastSeen",DateTime.UtcNow }
                })
                .Result()
                ?.FirstOrDefault();

            if(userId != null) {

                var userDetails = _lazySql.StoredProcedure("GetFullUserDetails")
                    .WithParameters(new Dictionary<string, object> {
                        {"@InternalId", userId["InternalId"].Value },
                    })
                    .Result()
                    ?.FirstOrDefault();

                var userRoles = _lazySql.StoredProcedure("GetAllRolesForUser")
                    .WithParameters(new Dictionary<string, object> {
                        {"@UserInternalId", userId["InternalId"].Value },
                    })
                    .Result()
                    .Select(x => (Guid)x["RoleId"].Value)
                    .ToArray();

                var publicUser = new PublicUser {
                    DisplayName = userDetails["DisplayName"].Value as string,
                    UserId = (int)userDetails["ExternalId"].Value,
                    InternalId = (Guid)userDetails["InternalId"].Value,
                    isLoggedIn = true,
                    Details = new UserDetails {
                        Description = userDetails["Description"].Value as string,
                        GitHubProfile = userDetails["GitHubProfile"].Value as string,
                        RoleGroupName = userDetails["RoleGroupName"].Value as string,
                        RoleGroupId = (Guid)userDetails["RoleGroupId"].Value,
                    },
                    Roles = userRoles
                };

                return publicUser;
            } else {
                return null;
            }
        }
    }
}
