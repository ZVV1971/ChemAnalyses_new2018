using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace SA_EF
{
    public partial class ChemicalAnalysesEntities : DbContext
    {
        public ChemicalAnalysesEntities()
            : base("name=CAEntities")
        {
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