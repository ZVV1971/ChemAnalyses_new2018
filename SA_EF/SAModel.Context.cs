using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace SA_EF
{
    public partial class ChemicalAnalysesEntities : DbContext
    {
        public static bool IsLoginPwdSet { get; set; } = false;
        private static string _connectionString = "name=CAEntities";
        public static string connectionString
        {
            get
            {
                if (_connectionString.StartsWith("name=")) return _connectionString;
                string pattern = @"(.+integrated security=)([^;]+)(;initial catalog=[^;]+)(.+)";
                RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
                Regex regex = new Regex(pattern, options);
                return regex.Replace(_connectionString, m => m.Groups[1].Value + "False;User ID="
                + UserName + ";Password=" + Password + m.Groups[4].Value);
            }
            set
            {
                _connectionString = value;
            }
        } 
        public static string UserName { get; set; } = "CAlogin";
        public static string Password { get; set; } = "guestpwd";

        public ChemicalAnalysesEntities()
            :base(connectionString)
        {}
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LinearCalibration> LineaCalibrations { get; set; }
        public virtual DbSet<DataPoint> DataPoints { get; set; }
        public virtual DbSet<SaltAnalysisData> SaltAnalysisDatas { get; set; }
        public virtual DbSet<CalibrationType> CalibrationType { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }
    
        public virtual int GetSamplesByMultipleIDs()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GetSamplesByMultipleIDs");
        }
    
        public virtual int UpdateCalibrationData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateCalibrationData");
        }
    }
}