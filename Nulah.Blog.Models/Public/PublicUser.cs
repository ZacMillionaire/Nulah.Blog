using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nulah.Blog.Models.Public {
    public class PublicUser {

        public PublicUser() {
            DisplayName = "Unregistered User";
            isLoggedIn = false;
            UserId = 0;
            Roles = new Guid[] { };
        }

        public string DisplayName { get; set; }
        public bool isLoggedIn { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public Guid InternalId { get; set; }
        public UserDetails Details { get; set; }
        public Guid[] Roles { get; set; }

        public bool CanViewAdminPanel() {
            return Roles.Contains(Guid.Parse("7A52EF09-D1B2-409B-9D9E-55D73A769B1F"));
        }

        public bool HasRole(string GuidString) {
            return Roles.Contains(Guid.Parse(GuidString));
        }
    }
}
