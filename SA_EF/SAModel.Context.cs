using System.Data.Entity;
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
        public static string connectionString { get; set; } = "name=CAEntities";
        private static DbConnectionApplicationRoleInterceptor dbConnInterceptor;

        public ChemicalAnalysesEntities(bool relogin = false) :base(connectionString)
        {
            if (relogin || !_areUserNameAndPwdSet)
            {
                if (dbConnInterceptor != null) DbInterception.Remove(dbConnInterceptor);
                dbConnInterceptor = new DbConnectionApplicationRoleInterceptor(_userName, _password);
                DbInterception.Add(dbConnInterceptor);
                DbConnectionApplicationRoleInterceptor.AppRoleTreatment += OnAppRoleTreatment;
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
   
        public virtual DbSet<LinearCalibration> LineaCalibrations { get; set; }
        public virtual DbSet<DataPoint> DataPoints { get; set; }
        public virtual DbSet<SaltAnalysisData> SaltAnalysisDatas { get; set; }
        public virtual DbSet<CalibrationType> CalibrationType { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }
    }
}