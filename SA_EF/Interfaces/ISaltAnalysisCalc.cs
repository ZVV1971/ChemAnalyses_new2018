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
        [CustomDescription("Магний")]
        decimal MgDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Кальций")]
        decimal CaDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Хлор")]
        decimal ClDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Бром")]
        decimal BrDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Нерастворимый осадок")]
        decimal ResiduumDry { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Сульфаты")]
        decimal SulfatesDry { get; set; }
        decimal SampleCorrectedDryWeight { get; set; }
        decimal CarbonatesDry { get; set; }
        decimal HydrocarbonatesDry { get; set; }
        decimal HumidityContent { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Калий")]
        decimal KDry { get; set; }
    }
    
     public interface ISaltAnalysisCalcResults
    {
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Сульфат кальция")]
        decimal CaSO4 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Натрий")]
        decimal Na { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Кристаллогидратная вода")]
        decimal? CrystWater { get; set; }
        decimal Carnallite { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Хлорид натрия")]
        decimal NaCl { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Хлорид калия")]
        decimal KCl { get; set; }
        [SchemesToCheckAttibute]
        [CustomDescription("Хлорид кальция")]
        decimal CaCl2 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI)]
        [CustomDescription("Хлорид магния")]
        decimal? MgCl2 { get; set; }
        decimal MgCl2AnyCase { get; set; }
        [SchemesToCheckAttibute]
        [CustomDescription("Бромид калия")]
        decimal KBr { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Гигроскопическая влага")]
        decimal? HygroWater { get; set; }
        decimal HygroWaterAnyCase { get; set; }
        decimal CaHCO3_2 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.SulfateMagnesiumI | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Сульфат магния")]
        decimal MgSO4 { get; set; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Сульфат натрия")]
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
        [CustomDescription("Сумма ионных форм")]
        decimal  IonSum { get; }
        [SchemesToCheckAttibute(SaltCalculationSchemes.Chloride | SaltCalculationSchemes.SulfateMagnesiumI
            | SaltCalculationSchemes.SulfateSodiumI)]
        [CustomDescription("Сумма солевых форм")]
        decimal  SaltSum { get; }
        SolidColorBrush IonSumColor { get; set; }
        SolidColorBrush SaltSumColor { get; set; }
    }
}