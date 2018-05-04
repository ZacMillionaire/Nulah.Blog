using System;

namespace Nulah.Blog.Models {
    public class AppSettings {
        public string MSSQLConnectionString
        {
            get
            {
                return _mssqlConnectionString;
            }
            set
            {
                if(_mssqlConnectionString == null) {
                    _mssqlConnectionString = value;
                } else {
                    throw new FieldAccessException($"Cannot set property after creation.");
                }
            }
        }

        public string SendGridApiKey { get; set; }
        public string DomainBaseUrl { get; set; }
        public Guid AdministratorRoleGroupId { get; set; }
        public Guid DefaultUserRoleGroupId { get; set; }

        protected string _mssqlConnectionString;
    }
}
