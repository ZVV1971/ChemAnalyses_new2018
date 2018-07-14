using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SA_EF
{
    public partial class LinearCalibration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LinearCalibration()
        {
            this.CalibrationData = new HashSet<DataPoint>();
            this.SaltAnalysis = new HashSet<SaltAnalysisData>();
            LinearCalibrationData = new ObservableCollection<DataPoint>[2];
            LinearCalibrationData[0] = new ObservableCollection<DataPoint>();
            LinearCalibrationData[1] = new ObservableCollection<DataPoint>();
            Slope = new decimal[2];
            RSquared = new decimal[2];
            Intercept = new decimal[2];
            CalibrationDate = DateTime.Today;
        }
    
        public int CalibrationID { get; set; }
        //public DateTime CalibrationDate { get; set; }
        //public string Description { get; set; }
        //public string CalibrationType { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DataPoint> CalibrationData { get; set; }
        public virtual CalibrationType CalibrationTypeSelector { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaltAnalysisData> SaltAnalysis { get; set; }
    }
}