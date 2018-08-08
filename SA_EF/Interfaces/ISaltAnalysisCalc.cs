using System.Windows.Media;

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
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal MgDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal CaDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal ClDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal BrDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal ResiduumDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal SulfatesDry { get; set; }
        decimal SampleCorrectedDryWeight { get; set; }
        decimal CarbonatesDry { get; set; }
        decimal HydrocarbonatesDry { get; set; }
        decimal HumidityContent { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal KDry { get; set; }
    }
    
     public interface ISaltAnalysisCalcResults
    {
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal CaSO4 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal Na { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal? CrystWater { get; set; }
        decimal Carnallite { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal NaCl { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal KCl { get; set; }
        [SchemesToCheckAttibute]
        decimal CaCl2 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI)]
        decimal? MgCl2 { get; set; }
        decimal MgCl2AnyCase { get; set; }
        [SchemesToCheckAttibute]
        decimal KBr { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal? HygroWater { get; set; }
        decimal HygroWaterAnyCase { get; set; }
        decimal CaHCO3_2 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.SulfateMagnesiumI | SaltCalculationSchemes.SulfateSodiumI)]
        decimal MgSO4 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.SulfateSodiumI)]
        decimal Na2SO4 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.SulfateMagnesiumI | SaltCalculationSchemes.SulfateSodiumI)]
        decimal NaBr { get; set; }
        string LabNumber { get; set; }
        string AnalysisDescription { get; set; }
        SaltCalculationSchemes RecommendedCalculationScheme { get; set; }
        SaltCalculationSchemes DefaultCalculationScheme { get; set; }
        bool  IsCalculated { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal  IonSum { get; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        decimal  SaltSum { get; }
        SolidColorBrush IonSumColor { get; set; }
        SolidColorBrush SaltSumColor { get; set; }
    }
}