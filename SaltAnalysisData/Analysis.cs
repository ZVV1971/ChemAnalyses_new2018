using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaltAnalysisDatas
{
    public abstract class Analysis
    {
        public int IDAnalysis { get; set; }
        public string AnalysisDescription { get; set; }
        public int IDSample { get; set; }
        public string LabNumber { get; set; } //just to show in the datagrid
    }
}
