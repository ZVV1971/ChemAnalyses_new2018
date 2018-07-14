namespace SA_EF
{ 
    public partial class DataPoint
    {
        public int IDCalibration { get; set; }
        public int IDCalibrationData { get; set; }
        public int Diapason { get; set; }
        //public decimal Concentration { get; set; }
        //public decimal Value { get; set; }
    
        public virtual LinearCalibration Calibrations { get; set; }
    }
}