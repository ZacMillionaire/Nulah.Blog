using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Nulah.LazyCommon.Core.MSSQL {
    public class LazyMapper {

        private readonly string _connectionString;

        private SqlConnection _connection;
        private SqlCommand _command;
        private string _transactionName;

        public LazyMapper(string ConnectionString) {
            _connectionString = ConnectionString;
        }

        /// <summary>
        /// Preps the query to be run
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public LazyMapper Query(string Query) {

            _connection = new SqlConnection(_connectionString);
            _transactionName = Guid.NewGuid().ToString();
            _command = new SqlCommand(Query, _connection);

            return this;
        }

        /// <summary>
        /// Prepares the query to be run under the given transaction name.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Transaction"></param>
        /// <returns></returns>
        public LazyMapper Query(string Query, string TransactionName) {

            _connection = new SqlConnection(_connectionString);
            _transactionName = TransactionName;
            _command = new SqlCommand(Query, _connection);

            return this;
        }

        /// <summary>
        /// Executes a non-query that returns no results, or where the results aren't important.
        /// </summary>
        /// <returns></returns>
        public LazyMapper Commit() {
            using(_connection) {
                _connection.Open();
                SqlTransaction transaction = _connection.BeginTransaction(_transactionName.Substring(0, 32));
                try {
                    //transaction = _connection.BeginTransaction(_transactionName);
                    _command.Transaction = transaction;
                    _command.ExecuteNonQuery();
                    transaction.Commit();
                } catch(Exception e) {
                    transaction.Rollback();
                    throw new Exception($"An exception occured attempting to execute a non-query. See inner exception for details.", e);
                }

                return this;
            }
        }

    }
}
