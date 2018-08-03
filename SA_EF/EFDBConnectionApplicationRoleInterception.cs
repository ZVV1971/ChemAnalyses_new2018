//https://gist.github.com/crmckenzie/f19df419453bd12adaa1
//EF6 with Application Roles
using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace SA_EF
{
    public class DbConnectionApplicationRoleInterceptor : IDbConnectionInterceptor
    {
        private readonly string _appRole;
        private readonly string _password;
        private byte[] _cookie;

        private static bool _set_approle_executed = false;

        public DbConnectionApplicationRoleInterceptor()
        {
        }

        public DbConnectionApplicationRoleInterceptor(string appRole, string password)
        {
            _appRole = appRole;
            _password = password;
        }

        public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Opened.");
            if (connection.State != ConnectionState.Open) return;
            ActivateApplicationRole(connection, _appRole, _password);
        }

        public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Closing.");
            if (connection.State != ConnectionState.Open) return;
            DeActivateApplicationRole(connection, _cookie);
        }

        public virtual void ActivateApplicationRole(DbConnection dbConn, string appRoleName, string password)
        {
            if (dbConn == null)
                throw new ArgumentNullException("DbConnection");
            if (ConnectionState.Open != dbConn.State)
                throw new InvalidOperationException("DBConnection must be opened before activating application role");
            if (string.IsNullOrWhiteSpace(appRoleName))
                throw new ArgumentNullException("appRoleName");
            if (password == null)
                throw new ArgumentNullException("password");
            SetApplicationRole(dbConn, appRoleName, password);
        }

        private string GetCurrentUserName(DbConnection dbConn)
        {
            using (var cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "SELECT USER_NAME();";
                return (string)cmd.ExecuteScalar();
            }
        }

        private void SetApplicationRole(DbConnection dbConn, string appRoleName, string password)
        {
            var currentUser = GetCurrentUserName(dbConn);
            if (!_set_approle_executed) {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_setapprole";
                    cmd.Parameters.Add(new SqlParameter("@rolename", appRoleName));
                    cmd.Parameters.Add(new SqlParameter("@password", password));
                    cmd.Parameters.Add(new SqlParameter("@fCreateCookie", SqlDbType.Bit) { Value = true });
                    var cookie = new SqlParameter("@cookie", SqlDbType.Binary, 50)
                    {
                        Direction = ParameterDirection.InputOutput
                    };

                    cmd.Parameters.Add(cookie);

                    Debug.WriteLine("ExecutingNonQuery to Set Application Role");

                    cmd.ExecuteNonQuery();

                    if (cookie.Value == null)
                    {
                        throw new InvalidOperationException(
                            "Failed to set application role, verify the database is configure correctly and the application role name / passwordis valid.");
                    }

                    _cookie = (byte[])cookie.Value;
                    _set_approle_executed = true;
                }
            }

            //var appUserName = GetCurrentUserName(dbConn);
            //The new user name should be the application role and not the app pool account.

            //if (string.Compare(currentUser, appUserName, true) == 0)
            //{
            //    throw new InvalidOperationException(
            //        "Failed to set MediaTypeNames.Application Role, verify the app role is configure correctly or the web configuration is valid.");
            //}
        }

        public virtual void DeActivateApplicationRole(DbConnection dbConn, byte[] cookie)
        {
            if (_set_approle_executed)
            {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "sp_unsetapprole";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@cookie", SqlDbType.VarBinary, 50) { Value = cookie });
                    Debug.WriteLine("ExecutingNonQuery to Unset Application Role");
                    cmd.ExecuteNonQuery();
                }
                _set_approle_executed = false;
            }
        }

        #region Other DbConnection Interception

        public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Transaction Began.");
        }

        public void BeginningTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Transaction BeginningTransaction.");
        }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Closed.");
        }

        public void ConnectionStringGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionStringGetting.");
        }

        public void ConnectionStringGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionStringGot.");
        }

        public void ConnectionStringSet(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionStringSet.");
        }

        public void ConnectionStringSetting(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionStringSetting.");
        }

        public void ConnectionTimeoutGetting(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionTimeoutGetting.");
        }

        public void ConnectionTimeoutGot(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        {
            Debug.WriteLine("Connection ConnectionTimeoutGot.");
        }

        public void DataSourceGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection DataSourceGetting.");
        }

        public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection DataSourceGot.");
        }

        public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection DatabaseGetting.");
        }

        public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection DatabaseGot.");
        }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Disposed.");
        }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Disposing.");
        }

        public void EnlistedTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection EnlistedTransaction.");
        }

        public void EnlistingTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection EnlistingTransaction.");
        }

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Debug.WriteLine("Connection Opening.");
        }

        public void ServerVersionGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ServerVersionGetting.");
        }

        public void ServerVersionGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        {
            Debug.WriteLine("Connection ServerVersionGot.");
        }

        public void StateGetting(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            Debug.WriteLine("Connection StateGetting.");
        }

        public void StateGot(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        {
            Debug.WriteLine("Connection StateGot.");
        }

        #endregion
    }
}