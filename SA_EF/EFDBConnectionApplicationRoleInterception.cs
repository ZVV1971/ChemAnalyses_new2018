//https://gist.github.com/crmckenzie/f19df419453bd12adaa1 //EF6 with Application Roles
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
        private static bool _isSetApproleExecuted = false;

        public DbConnectionApplicationRoleInterceptor(){}

        public DbConnectionApplicationRoleInterceptor(string appRole, string password)
        {
            _appRole = appRole;
            _password = password;
        }

        public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (connection.State != ConnectionState.Open) return;
#if DEBUG
            Debug.WriteLine("Connection Opened.");
#endif
            ActivateApplicationRole(connection, _appRole, _password);
        }

        public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (connection.State != ConnectionState.Open) return;
#if DEBUG
            Debug.WriteLine("Connection Closing.");
#endif
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

        private void SetApplicationRole(DbConnection dbConn, string appRoleName, string password)
        {
            if (!_isSetApproleExecuted) {
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
#if DEBUG
                    Debug.WriteLine("ExecutingNonQuery to Set Application Role");
#endif
                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception ex) { }

                    int res = 0;

                    if (cookie.Value != null 
                        //Check if cookie value is not 0xFFFFFFFF == ERROR
                        && BitConverter.ToInt32((byte[])cookie.Value, 0) != -1)
                    {
                        _cookie = (byte[])cookie.Value;
                        _isSetApproleExecuted = true;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT IS_MEMBER(N'db_accessadmin')";
                        try { res = (int)cmd.ExecuteScalar(); }
                        catch { }
                    }

                    OnAppRoleTreatment(new AppRoleTreatmentEventArgs(_isSetApproleExecuted, res == 1 &&
                        _isSetApproleExecuted));
                }
            }
        }

        public virtual void DeActivateApplicationRole(DbConnection dbConn, byte[] cookie)
        {
            if (_isSetApproleExecuted)
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
                _isSetApproleExecuted = false;
            }
        }
        #region Other DbConnection Interception
        public void BeganTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext){ }

        public void BeginningTransaction(DbConnection connection,
            BeginTransactionInterceptionContext interceptionContext){ }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void ConnectionStringGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext) {}

        public void ConnectionStringGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext) {}

        public void ConnectionStringSet(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext){ }

        public void ConnectionStringSetting(DbConnection connection,
            DbConnectionPropertyInterceptionContext<string> interceptionContext){ }

        public void ConnectionTimeoutGetting(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext){ }

        public void ConnectionTimeoutGot(DbConnection connection,
            DbConnectionInterceptionContext<int> interceptionContext){ }

        public void DataSourceGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void DataSourceGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void DatabaseGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void DatabaseGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void EnlistedTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext){ }

        public void EnlistingTransaction(DbConnection connection,
            EnlistTransactionInterceptionContext interceptionContext){ }

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext){}

        public void ServerVersionGetting(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void ServerVersionGot(DbConnection connection,
            DbConnectionInterceptionContext<string> interceptionContext){ }

        public void StateGetting(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext){ }

        public void StateGot(DbConnection connection,
            DbConnectionInterceptionContext<ConnectionState> interceptionContext){ }
        #endregion

        public static event AppRoleTreatmentEventHandler AppRoleTreatment;

        protected virtual void OnAppRoleTreatment (AppRoleTreatmentEventArgs e)
        {
            AppRoleTreatment?.Invoke(this, e);
        }
    }

    public class AppRoleTreatmentEventArgs : EventArgs
    {
        private readonly bool _hasAppRolePassed = false;
        private readonly bool _isMemberOfAdmin = false;
        
        public AppRoleTreatmentEventArgs(bool hasApprolePassed, bool isMemberOfAdmin)
        {
            _isMemberOfAdmin = isMemberOfAdmin;
            _hasAppRolePassed = hasApprolePassed;
        }

        public bool HasAppRolePassed
        {
            get { return _hasAppRolePassed; }
        }

        public bool IsMemberOfAdmin
        {
            get { return _isMemberOfAdmin; }
        }
    }

    public delegate void AppRoleTreatmentEventHandler(object sender, AppRoleTreatmentEventArgs e);
}