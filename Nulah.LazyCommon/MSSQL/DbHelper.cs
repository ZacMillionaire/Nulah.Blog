using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Nulah.LazyCommon.Core.MSSQL {
    public static class DbHelper {
        public static bool TestConnection(string connectionString) {
            using(var connection = new SqlConnection(connectionString)) {
                try {
                    connection.Open();
                    return true;
                } catch(SqlException) {
                    return false;
                }
            }

        }
    }
}
