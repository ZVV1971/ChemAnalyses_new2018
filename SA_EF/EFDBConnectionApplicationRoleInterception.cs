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

        public DbConnectionApplicationRoleInterceptor(){}

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
            //var currentUser = GetCurrentUserName(dbConn);
            if (!_set_approle_executed) {
                using (var cmd = dbConn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_setapprole";
                    cmd.Parameters.Add(new SqlParameter("@rolename", appRoleName));
                    cmd.Parameters.Add(new SqlParameter("@password", password));
                    cmd.Parameters.Add(new SqlParameter("@fCreateCookie", SqlDbType.Bit) { Value = true });
                    var cookie = new SqlParameter("@cookie", SqlDbType.Binary, 50)
                    {Direction = ParameterDirection.InputOutput};

                    cmd.Parameters.Add(cookie);

                    Debug.WriteLine("ExecutingNonQuery to Set Application Role");

                    try {cmd.ExecuteNonQuery();}
                    catch (Exception ex){}

                    if (cookie.Value == null)
                    {
                        throw new InvalidOperationException(
                            "Failed to set application role, verify the database is configured correctly and" +
                            " the application role name/password is valid.");
                    }

                    //cmd.CommandType = CommandType.Text;
                    //cmd.CommandText = "SELECT IS_MEMBER(N'db_accessadmin')";
                    //var res = cmd.ExecuteScalar();
                    _cookie = (byte[])cookie.Value;
                    _set_approle_executed = true;
                }
            }
            //var appUserName = GetCurrentUserName(dbConn);
            //if (string.Compare(currentUser, appUserName, true) == 0)
            //{throw new InvalidOperationException("Failed to set MediaTypeNames.Application Role.");}
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
                    try
                    { cmd.ExecuteNonQuery();}
                    catch (Exception ex) {}
                }
                _set_approle_executed = false;
            }
        }
        #region Other DbConnection Interception
        public void BeganTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        { }

        public void BeginningTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext)
        { }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void ConnectionStringGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void ConnectionStringGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void ConnectionStringSet(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        { }

        public void ConnectionStringSetting(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext)
        { }

        public void ConnectionTimeoutGetting(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        { }

        public void ConnectionTimeoutGot(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext)
        { }

        public void DataSourceGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void DataSourceGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void DatabaseGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void DatabaseGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void EnlistedTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        { }

        public void EnlistingTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext)
        { }

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void ServerVersionGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void ServerVersionGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext)
        { }

        public void StateGetting(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        { }

        public void StateGot(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext)
        { }
        #endregion
    }
}