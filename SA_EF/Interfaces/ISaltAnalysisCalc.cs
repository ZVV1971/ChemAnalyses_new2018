namespace SA_EF.Interfaces
{
     public interface ISaltAnalysisCalculation
    {
        decimal CalcKaliumValue();
        SaltCalculationSchemes CalcRecommendedScheme(ISaltAnalysisDryData dryData);
        decimal CalcCorrectedDryWeight(ISaltAnalysisDryData dryData, SaltCalculationSchemes defScheme);
        ISaltAnalysisCalcResults CalcSchemeResults(ISaltAnalysisDryData dryData, SaltCalculationSchemes defSchema);
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

     public interface ISaltAnalysisCalcResults
    {
         decimal CaSO4 { get; set; }
         decimal Na { get; set; }
         decimal? CrystWater { get; set; }
         decimal Carnallite { get; set; }
         decimal NaCl { get; set; }
         decimal KCl { get; set; }
         decimal CaCl2 { get; set; }
         decimal? MgCl2 { get; set; }
         decimal MgCl2AnyCase { get; set; }
         decimal KBr { get; set; }
         decimal? HygroWater { get; set; }
         decimal HygroWaterAnyCase { get; set; }
         decimal CaHCO3_2 { get; set; }
         decimal MgSO4 { get; set; }
         decimal Na2SO4 { get; set; }
         decimal NaBr { get; set; }
         string LabNumber { get; set; }
         string AnalysisDescription { get; set; }
         SaltCalculationSchemes RecommendedCalculationScheme { get; set; }
         SaltCalculationSchemes DefaultCalculationScheme { get; set; }
    }
}