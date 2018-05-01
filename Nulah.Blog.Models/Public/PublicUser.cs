using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    }
}
