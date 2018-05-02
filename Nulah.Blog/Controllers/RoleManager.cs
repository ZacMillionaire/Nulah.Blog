using Nulah.Blog.Models;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Controllers {
    public class RoleManager {
        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RoleManager(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        public PublicRoleDetails[] GetAllRolesWithDescriptions() {
            var roles = _lazySql.Query(@"SELECT [RoleId],[Name],[Description] FROM [Roles]")
                .Result()
                .Select(x => new PublicRoleDetails {
                    Id = (Guid)x["RoleId"].Value,
                    Name = x["Name"].Value as string,
                    Description = x["Description"].Value as string,
                })
                .ToArray();

            return roles;
        }

    }
}
