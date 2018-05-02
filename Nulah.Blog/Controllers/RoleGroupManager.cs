using Nulah.Blog.Models;
using Nulah.Blog.Models.Public;
using Nulah.LazyCommon.Core.MSSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nulah.Blog.Controllers {
    public class RoleGroupManager {
        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RoleGroupManager(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        public bool RoleGroupExistsByName(string RoleGroupName) {
            var exists = (bool)_lazySql.StoredProcedure("RoleGroupExists")
                .WithParameters(new Dictionary<string, object> {
                    { "@RoleGroupName",RoleGroupName }
                })
                .Result()
                .First()["Exists"].Value;

            return exists;
        }

        public List<PublicRoleGroup> GetAllRoleGroups() {
            var roleGroups = _lazySql.StoredProcedure("GetAllRoleGroupsWithUserCount")
                .Result()
                .Select(x => new PublicRoleGroup {
                    Id = (Guid)x["RoleGroupId"].Value,
                    Description = x["Description"].Value as string,
                    MemberCount = (int)x["Members"].Value,
                    Name = x["Name"].Value as string
                })
                .ToList();

            return roleGroups;
        }


        public PublicRoleGroup GetRoleDetails(Guid RoleGroupId) {
            var roleGroupDetails = _lazySql.StoredProcedure("GetFullRoleGroupDetails")
                .WithParameters(new Dictionary<string, object> {
                    {"@RoleGroupId",RoleGroupId }
                })
                .Result();

            var groupRoles = roleGroupDetails.Where(x => x["RoleId"].Value.Equals(System.DBNull.Value) == false)
                .Select(x => new PublicRoleDetails {
                    Id = (Guid)x["RoleId"].Value,
                    Name = x["RoleName"].Value as string,
                    Description = x["RoleDescription"].Value as string
                })
                .ToArray();

            var roleDetails = new PublicRoleGroup {
                Id = RoleGroupId,
                Description = roleGroupDetails.First()["Description"].Value as string,
                Name = roleGroupDetails.First()["Name"].Value as string,
                Roles = groupRoles
            };

            return roleDetails;
        }
    }
}
