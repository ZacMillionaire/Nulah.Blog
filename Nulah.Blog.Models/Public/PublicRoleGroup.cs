using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Blog.Models.Public {
    public class PublicRoleGroup {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MemberCount { get; set; }
        public PublicUser Members { get; set; }
        public PublicRoleDetails[] Roles { get; set; }
    }
}
