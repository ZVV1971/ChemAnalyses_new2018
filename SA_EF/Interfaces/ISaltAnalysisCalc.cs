namespace SA_EF.Interfaces
{
    public interface ISaltAnalysisCalculation
    {
        decimal CalcKaliumValue();
        SaltCalculationSchemes CalcRecommendedScheme();
        decimal CalcCorrectedDryWeight(SaltCalculationSchemes defScheme);
    }

    public interface ISaltAnalysisDryData
    {
        decimal MgWet { get; set; }
        decimal MgDry { get; set; }
        decimal CaDry { get; set; }
        decimal ClDry { get; set; }
        decimal BrDry { get; set; }
        decimal ResiduumDry { get; set; }
        decimal SulfatesDry { get; set; }
        decimal SampleCorrectedDryWeight { get; set; }
        decimal CarbonatesDry { get; set; }
        decimal HydrocarbonatesDry { get; set; }
        decimal HumidityContent { get; set; }
        decimal KDry { get; set; }
    }
}