namespace SA_EF.Interfaces
{
    public interface ILinearCalibration
    {
        int CalibrationID { get; set; }
        decimal[] Slope { get; set; }
        decimal[] Intercept { get; set; }
        decimal ValueToConcentration(decimal val, int diap);
    }
}