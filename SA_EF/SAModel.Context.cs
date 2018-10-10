using System;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;

namespace SA_EF
{
    [DbConfigurationType(typeof(FE6CodeConfig))]
    public partial class ChemicalAnalysesEntities : DbContext
    {
        private static string _userName = "";
        public static string UserName { set { if (value != null) _userName = value; } }
        private static string _password = "";
        public static string Password { set { if (value != null) _password = value; } }
        private static bool _areUserNameAndPwdSet = false;
        public static bool AreUserNameAndPwdSet { get {return _areUserNameAndPwdSet; } }
        private static bool _isAdmin = false;
        public static bool IsAdmin { get { return _isAdmin; } }
        public static string connectionString { private get; set; } = "name=CAEntities";
        private static DbConnectionApplicationRoleInterceptor dbConnInterceptor;

        public ChemicalAnalysesEntities(bool relogin = false) :base(ConnectionStringRebuilder(connectionString))
        {
            if (relogin || !_areUserNameAndPwdSet)
            {
                if (dbConnInterceptor != null) DbInterception.Remove(dbConnInterceptor);
                dbConnInterceptor = new DbConnectionApplicationRoleInterceptor(_userName, _password);
                DbInterception.Add(dbConnInterceptor);
                DbConnectionApplicationRoleInterceptor.AppRoleTreatment += OnAppRoleTreatment;
                DbConnectionApplicationRoleInterceptor.EFStateChange += OnEFStateChange;
            }
        }

        protected void OnAppRoleTreatment(object sender, AppRoleTreatmentEventArgs e)
        {
            if (e != null)
            {
                _areUserNameAndPwdSet = e.HasAppRolePassed;
                _isAdmin = e.IsMemberOfAdmin;
            }
        }

        protected void OnEFStateChange(object sender, EFConnectionStatesChangeArgs e)
        {
            if (e != null)
            {
                string s = string.Empty ;
                switch (e.State)
                {
                    case EFConnectionStates.Connecting:
                        s = "Подключение…";
                        break;
                    case EFConnectionStates.Authorizing:
                        s = "Авторизация…";
                        break;
                    case EFConnectionStates.Initializing:
                        s = "Инициализация…";
                        break;
                    default:
                        break;
                }
                OnStateChange(new StatesEventArgs(s));
            }
        }

        protected static string ConnectionStringRebuilder(string connectionString)
        {
            string connstr = "";
            string certName = "";
            string pattern = @"(.+data source=)(\S+==)(.+user id=)(\S+[^;])(;password=)(\S+==)(.+)";
            Regex regex = new Regex(pattern);
            try
            {
                connstr = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).
                    ConnectionStrings.ConnectionStrings["CAEntities"].ToString();
                certName = Properties.Settings.Default.CertificateName;
            }
            catch (Exception ex)
            { }
            MatchCollection match = regex.Matches(connstr);
            if (match.Count == 1 || match[0].Groups.Count == 6)
            {
                X509EncDec cert = new X509EncDec(certName);
                string substitution = @"$1 " + cert.DecryptRsa(match[0].Groups[2].Value) 
                    + "$3" + cert.DecryptRsa(match[0].Groups[4].Value) + "$5" 
                    + cert.DecryptRsa(match[0].Groups[6].Value) + "$7";
                return regex.Replace(connstr, substitution);
            }
            return connectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        
        public virtual DbSet<LinearCalibration> LineaCalibrations { get; set; }
        public virtual DbSet<DataPoint> DataPoints { get; set; }
        public virtual DbSet<SaltAnalysisData> SaltAnalysisDatas { get; set; }
        public virtual DbSet<CalibrationType> CalibrationType { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }

        public static event StatesChangesEventHandler StateChanged;
        protected virtual void OnStateChange (StatesEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

    }

    public class StatesEventArgs: EventArgs
    {
        public string NameOfTheState { get; set; }
        public StatesEventArgs(string s)
        {
            NameOfTheState = s;
        }
    }
    public delegate void StatesChangesEventHandler (object sender, StatesEventArgs e);
}