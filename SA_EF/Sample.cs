using System;
using System.Collections.Generic;

namespace SA_EF
{
    public partial class Sample
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sample()
        {
            this.SaltAnalysisDatas = new HashSet<SaltAnalysisData>();
        }
    
        public int IDSample { get; set; }
        //public string LabNumber { get; set; }
        //public System.DateTime SamplingDate { get; set; }
        //public string Description { get; set; }
        public Nullable<int> SamplesCount { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaltAnalysisData> SaltAnalysisDatas { get; set; }
    }
}