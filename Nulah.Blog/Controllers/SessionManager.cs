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

        public PublicUser RefreshSession(string SessionId) {
            var user = _lazySql.StoredProcedure("CheckSessionAndReturnUserData")
                .WithParameters(new Dictionary<string, object> {
                    {"@SessionId", SessionId },
                    {"@LastSeen",DateTime.UtcNow }
                })
                .Result()
                .First();
            var publicUser = new PublicUser {
                DisplayName = user["DisplayName"].Value as string,
                UserId = (int)user["ExternalId"].Value,
                InternalId = (Guid)user["InternalId"].Value,
                isLoggedIn = true,
                Details = new UserDetails {
                    Description = user["Description"].Value as string,
                    GitHubProfile = user["GitHubProfile"].Value as string
                }
            };

            return publicUser;
        }
    }
}
