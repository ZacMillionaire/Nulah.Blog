using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
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
        /// Prepares the query to be run
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public LazyMapper Query(string Query) {

            //_connection = new SqlConnection(_connectionString);
            //_transactionName = Guid.NewGuid().ToString();
            //_command = new SqlCommand(Query, _connection);

            return PrepareQuery(Query, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Prepares the query to be run under the given transaction name.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Transaction"></param>
        /// <returns></returns>
        public LazyMapper Query(string Query, string TransactionName) {

            //_connection = new SqlConnection(_connectionString);
            //_transactionName = TransactionName;
            //_command = new SqlCommand(Query, _connection);

            return PrepareQuery(Query, TransactionName);
        }

        private LazyMapper PrepareQuery(string Query, string TransactionName) {

            _connection = new SqlConnection(_connectionString);
            _transactionName = TransactionName;
            _command = new SqlCommand(Query, _connection);
            _command.CommandType = CommandType.Text; // This is the default value, but just incase

            return this;
        }

        /// <summary>
        /// Prepares to execute a stored procedure by the given name.
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        public LazyMapper StoredProcedure(string ProcedureName) {

            //_connection = new SqlConnection(_connectionString);
            //_transactionName = Guid.NewGuid().ToString();
            //_command = new SqlCommand(ProcedureName, _connection);

            return PrepareStoredProcedure(ProcedureName, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Prepares to execute a stored procedure by the given name, under the given transaction
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        public LazyMapper StoredProcedure(string ProcedureName, string TransactionName) {

            //_connection = new SqlConnection(_connectionString);
            //_transactionName = TransactionName;
            //_command = new SqlCommand(ProcedureName, _connection);

            return PrepareStoredProcedure(ProcedureName, TransactionName);
        }

        private LazyMapper PrepareStoredProcedure(string ProcedureName, string TransactionName) {

            _connection = new SqlConnection(_connectionString);
            _transactionName = TransactionName;
            _command = new SqlCommand(ProcedureName, _connection);
            _command.CommandType = CommandType.StoredProcedure;

            return this;
        }


        public LazyMapper WithParameters(Dictionary<string, object> Parameters) {

            if(_command.CommandType == CommandType.StoredProcedure) {
                foreach(var parameter in Parameters) {
                    _command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
                }
            } else {
                throw new NotSupportedException($".WithParameters(Dictionary<string, object>) can only be used on stored procedures currently.");
            }

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

        /// <summary>
        /// Returns the result of the query
        /// </summary>
        /// <returns></returns>
        public LazyTableResult Result() {

            LazyTableResult result = null;

            using(_connection) {
                _connection.Open();
                SqlTransaction transaction = _connection.BeginTransaction(_transactionName.Substring(0, 32));
                try {
                    _command.Transaction = transaction;
                    using(var sqlReader = _command.ExecuteReader()) {
                        if(sqlReader.HasRows) {
                            result = ReadSqlRows(sqlReader);
                        }
                    }
                    transaction.Commit();
                } catch(Exception e) {
                    transaction.Rollback();
                    throw new Exception($"An exception occured attempting to execute a non-query. See inner exception for details.", e);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the result of a query, after a user defined function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public List<T> Result<T>(Func<LazyTableResult, T> f) {

            LazyTableResult result = null;
            List<T> funcResult = new List<T>();

            using(_connection) {
                _connection.Open();
                SqlTransaction transaction = _connection.BeginTransaction(_transactionName.Substring(0, 32));
                try {
                    _command.Transaction = transaction;
                    using(var sqlReader = _command.ExecuteReader()) {
                        if(sqlReader.HasRows) {
                            result = ReadSqlRows(sqlReader);
                            funcResult.Add(f(result));
                        }
                    }
                    transaction.Commit();
                } catch(Exception e) {
                    transaction.Rollback();
                    throw new Exception($"An exception occured attempting to execute a non-query. See inner exception for details.", e);
                }
            }

            return funcResult;
        }
        private LazyTableResult ReadSqlRows(SqlDataReader SqlReader) {

            LazyTableResult result = new LazyTableResult();

            while(SqlReader.Read()) {

                LazyRow thisRow = new LazyRow();

                for(var i = 0; i < SqlReader.FieldCount; i++) {
                    LazyColumn col = new LazyColumn(
                        Type: SqlReader.GetFieldType(i),
                        Value: SqlReader.GetValue(i),
                        Name: SqlReader.GetName(i)
                    );
                    thisRow.Add(col.Name, col);
                }

                result.Add(thisRow);
            }

            return result;
        }

        public class LazyTableResult : List<LazyRow> {
            private List<LazyRow> _rows { get; set; }


            public bool IsReadOnly => false;

            public LazyTableResult() {
                _rows = new List<LazyRow>();
            }
            /*
            public int Count => _rows.Count;
            public void Add(LazyRow Row) {
                _rows.Add(Row);
            }

            public void Clear() {
                _rows = new List<LazyRow>();
            }

            public bool Contains(LazyRow item) {
                return _rows.Contains(item);
            }

            public void CopyTo(LazyRow[] array, int arrayIndex) {
                _rows.CopyTo(array, arrayIndex);
            }

            public bool Remove(LazyRow item) {
                return _rows.Remove(item);
            }

            public IEnumerator<LazyRow> GetEnumerator() {
                return _rows.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return _rows.GetEnumerator();
            }*/
        }

        public class LazyRow : Dictionary<string, LazyColumn> {
            private Dictionary<string, LazyColumn> _columns { get; set; }

            public int Columns
            {
                get
                {
                    return _columns.Count;
                }
            }

            public LazyRow() {
                _columns = new Dictionary<string, LazyColumn>();
            }
        }

        public class LazyColumn {
            public string Name { get; set; }
            public object Value { get; set; }

            private Type _type { get; set; }

            public LazyColumn(string Name, object Value, Type Type) {
                this.Name = Name;
                this.Value = Value;
                _type = Type;
            }

            /// <summary>
            /// Returns the type of the column returned from the database
            /// </summary>
            /// <returns></returns>
            public Type GetColumnType() {
                return _type;
            }

            public override string ToString() {
                return Value.ToString();
            }
        }
    }
}
