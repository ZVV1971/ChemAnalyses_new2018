namespace SA_EF
{  
    public partial class SaltAnalysisData
    {
        public int IDSaltAnalysis { get; set; }
        public int IDSample { get; set; }
        //public decimal WetWeight { get; set; }
        //public decimal SulfatesCrucibleEmptyWeight { get; set; }
        //public decimal SulfatesCrucibleFullWeight { get; set; }
        //public decimal ResiduumCrucibleEmptyWeight { get; set; }
        //public decimal ResiduumCrucibleFullWeight { get; set; }
        //public decimal HumidityCrucibleEmptyWeight { get; set; }
        //public decimal HumidityCrucibleWetSampleWeight { get; set; }
        //public decimal HumidityCrucibleDry110SampleWeight { get; set; }
        //public Nullable<decimal> HumidityCrucibleDry180SampleWeight { get; set; }
        //public DateTime AnalysisDate { get; set; }
        //public decimal MagnesiumTitre { get; set; }
        //public decimal CalciumTitre { get; set; }
        //public decimal ChlorumTitre { get; set; }
        //public decimal BromumTitre { get; set; }
        //public decimal CarbonatesTitre { get; set; }
        //public decimal HydrocarbonatesTitre { get; set; }
        //public decimal MagnesiumAliquote { get; set; }
        //public decimal CalciumAliquote { get; set; }
        //public decimal ChlorumAliquote { get; set; }
        //public decimal BromumAliquote { get; set; }
        //public decimal SulfatesAliquote { get; set; }
        //public decimal MagnesiumTrilonTitre { get; set; }
        //public decimal CalciumTrilonTitre { get; set; }
        //public decimal HgCoefficient { get; set; }
        //public decimal BromumBlank { get; set; }
        //public decimal BromumStandardTitre { get; set; }
        //public decimal SulfatesBlank { get; set; }
        //public decimal KaliumValue { get; set; }
        //public decimal KaliumVolume { get; set; }
        //public Nullable<decimal> KaliumConcentration { get; set; }
        //public int KaliumDiapason { get; set; }
        //public int KaliumCalibration { get; set; }
    
        public virtual LinearCalibration Calibration { get; set; }
        public virtual Sample Samples { get; set; }
    }
}