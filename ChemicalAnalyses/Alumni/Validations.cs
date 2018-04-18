using System;
using SaltAnalysisDatas;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChemicalAnalyses.Alumni
{
    public class SaltAnalysisValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            SaltAnalysisData slt = (value as BindingGroup).Items[0] as SaltAnalysisData;

            try
            {
                SaltAnalysisData new_s = new SaltAnalysisData
                {
                    AnalysisDate = slt.AnalysisDate,
                    BromumAliquote = slt.BromumAliquote,
                    BromumBlank = slt.BromumBlank,
                    BromumStandardTitre = slt.BromumStandardTitre,
                    BromumTitre = slt.BromumTitre,
                    CalciumAliquote = slt.CalciumAliquote,
                    CalciumTitre = slt.CalciumTitre,
                    CalciumTrilonTitre = slt.CalciumTrilonTitre,
                    CarbonatesTitre = slt.CarbonatesTitre,
                    ChlorumAliquote = slt.ChlorumAliquote,
                    ChlorumTitre = slt.ChlorumTitre,
                    HgCoefficient = slt.HgCoefficient,
                   
                    HumidityCrucibleEmptyWeight = slt.HumidityCrucibleEmptyWeight,
                    HumidityCrucibleWetSampleWeight = slt.HumidityCrucibleWetSampleWeight,
                    HumidityCrucibleDry110SampleWeight = slt.HumidityCrucibleDry110SampleWeight,
                    HumidityCrucibleDry180SampleWeight = slt.HumidityCrucibleDry180SampleWeight,
                    
                    HydrocarbonatesTitre = slt.HydrocarbonatesTitre,
                    IDSample = slt.IDSample,
                    KaliumCalibration = slt.KaliumCalibration,
                    KaliumConcentration = slt.KaliumConcentration,
                    KaliumDiapason = slt.KaliumDiapason,
                    KaliumValue = slt.KaliumValue,
                    KaliumVolume = slt.KaliumVolume,
                    MagnesiumAliquote = slt.MagnesiumAliquote,
                    MagnesiumTitre = slt.MagnesiumTitre,
                    MagnesiumTrilonTitre = slt.MagnesiumTrilonTitre,
                    ResiduumCrucibleEmptyWeight = slt.ResiduumCrucibleEmptyWeight,
                    ResiduumCrucibleFullWeight = slt.ResiduumCrucibleFullWeight,
                    SulfatesAliquote = slt.SulfatesAliquote,
                    SulfatesBlank = slt.SulfatesBlank,
                    SulfatesCrucibleEmptyWeight = slt.SulfatesCrucibleEmptyWeight,
                    SulfatesCrucibleFullWeight = slt.SulfatesCrucibleFullWeight,
                    WetWeight = slt.WetWeight
                };
                return ValidationResult.ValidResult;
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }
            }
        }
    }