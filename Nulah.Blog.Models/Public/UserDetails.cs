﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nulah.Blog.Models.Public {
    public class UserDetails {
        public string GitHubProfile { get; set; }
        public string Description { get; set; }
        public string RoleGroupName { get; set; }
        public Guid RoleGroupId { get; set; }
    }
}
