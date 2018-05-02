using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nulah.Blog.Controllers;
using Nulah.Blog.Models;
using Nulah.LazyCommon.Core.MSSQL;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nulah.Blog.Areas.Admin.Controllers {
    [Area("Admin")]
    public class RoleGroupController : Controller {
        private readonly AppSettings _appSettings;
        private readonly LazyMapper _lazySql;

        public RoleGroupController(AppSettings appSettings, LazyMapper lm) {
            _appSettings = appSettings;
            _lazySql = lm;
        }

        [HttpGet]
        [Route("~/Admin/RoleGroups")]
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        [Route("~/Admin/RoleGroups/New")]
        public IActionResult NewRoleGroupForm() {
            var roleManager = new RoleManager(_appSettings, _lazySql);
            ViewData.Add("Roles", roleManager.GetAllRolesWithDescriptions());
            return View();
        }

        [HttpPost]
        [Route("~/Admin/RoleGroups/New")]
        [ValidateAntiForgeryToken]
        public IActionResult NewRoleGroupForm_Post([FromForm]RoleGroupFormData newRoleGroupData) {
            var roleGroupManager = new RoleGroupManager(_appSettings, _lazySql);
            if(roleGroupManager.RoleGroupExistsByName(newRoleGroupData.Name)) {
                ViewData.Add("Error", $"A role group already exists with the name {newRoleGroupData.Name}");
                return View("NewRoleGroupError");
            } else {
                ViewData.Add("Error", $"Role group creation not fully implemented yet");
                return View("NewRoleGroupError");
            }

            //return View("NewRoleGroupCreated");
        }


        [HttpGet]
        [Route("~/Admin/RoleGroups/Edit")]
        public IActionResult EditRoleGroupList() {
            var roleGroupManager = new RoleGroupManager(_appSettings, _lazySql);
            ViewData.Add("RoleGroups", roleGroupManager.GetAllRoleGroups());
            return View();
        }

        [HttpGet]
        [Route("~/Admin/RoleGroups/Edit/{RoleGroupGuid}")]
        public IActionResult EditRoleGroupForm(Guid RoleGroupGuid) {
            var roleGroupManager = new RoleGroupManager(_appSettings, _lazySql);
            var roleManager = new RoleManager(_appSettings, _lazySql);

            ViewData.Add("Roles", roleManager.GetAllRolesWithDescriptions());
            ViewData.Add("RoleGroup", roleGroupManager.GetRoleDetails(RoleGroupGuid));

            return View();
        }

        [HttpPost]
        [Route("~/Admin/RoleGroups/Edit/{RoleGroupGuid}")]
        public IActionResult EditRoleGroupForm_Post([FromForm]RoleGroupFormData updatedRoleGroupData, Guid RoleGroupGuid) {

            if(RoleGroupGuid == _appSettings.AdministratorRoleGroupId) {
                throw new Exception("The Administrator role group cannot be modified.");
            }

            // TODO: Convert this to a stored procedure, most likely with a table parameter.
            // https://stackoverflow.com/questions/5595353/how-to-pass-table-value-parameters-to-stored-procedure-from-net-code
            // Heck, I'm not even sure having it as a stored proc would be correct
            var roleGroupManager = new RoleGroupManager(_appSettings, _lazySql);
            var preEdit = roleGroupManager.GetRoleDetails(RoleGroupGuid);

            var removedRoles = preEdit.Roles.Where(x => updatedRoleGroupData.Roles.Contains(x.Id) == false).Select(x => x.Id);
            var addedRoles = updatedRoleGroupData.Roles.Where(x => preEdit.Roles.Any(y => y.Id == x) == false);

            string insertRolesQuery;
            string deleteRolesQuerySP;
            string deleteRoleQuery;

            if(addedRoles.Count() > 0) {
                var valueList = new List<string>();

                foreach(var addedRole in addedRoles) {
                    valueList.Add($"(N'{RoleGroupGuid}',N'{addedRole}')");
                }

                insertRolesQuery = $@"INSERT INTO [dbo].[RoleGroup_Roles]
                       ([RoleGroupId]
                       ,[RoleId])
                 VALUES {string.Join(",", valueList)}";
            }

            if(removedRoles.Count() > 0) {

                deleteRolesQuerySP = $"SET @DeleteQuery = N'DELETE FROM [dbo].[RoleGroup_Roles] WHERE [RoleGroupId] = @RoleGroupId AND [RoleId] = @RoleId'{Environment.NewLine}" +
                    "SET @DeleteParamDef = N'@RoleGroupId UNIQUEIDENTIFIER, @RoleId UNIQUEIDENTIFIER'";

                var sb = new StringBuilder();

                foreach(var removedRole in removedRoles) {
                    sb.AppendLine($"EXECUTE sp_executesql @DeleteQuery, @DeleteParamDef, @RoleGroupId = N'{RoleGroupGuid}',@RoleId = N'{removedRole}'");
                }

                deleteRoleQuery = sb.ToString();
            }

            return View();
        }
    }

    public class RoleGroupFormData {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid[] Roles { get; set; }
    }
}
