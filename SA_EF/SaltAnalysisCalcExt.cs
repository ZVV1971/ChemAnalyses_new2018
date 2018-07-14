using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Reflection;
using System.Globalization;
using System.IO;

namespace SA_EF
{
    public partial class SaltAnalysisData
    {
        #region DrywWeights
        [NotMapped]
        public decimal MgWet { get; set; }
        [NotMapped]
        public decimal MgDry { get; set; }
        [NotMapped]
        public decimal CaDry { get; set; }
        [NotMapped]
        public decimal ClDry { get; set; }
        [NotMapped]
        public decimal BrDry { get; set; }
        [NotMapped]
        public decimal ResiduumDry { get; set; }
        [NotMapped]
        public decimal SulfatesDry { get; set; }
        [NotMapped]
        public decimal SampleCorrectedDryWeight { get; set; }
        [NotMapped]
        public decimal CarbonatesDry { get; set; }
        [NotMapped]
        public decimal HydrocarbonatesDry { get; set; }
        [NotMapped]
        public decimal HumidityContent { get; set; }
        [NotMapped]
        public decimal KDry { get; set; }
        #endregion DryWeights
        //Calibrations "Pool" in case there will be many of them present 
        //storing and retireving works faster than reading from SQL server
        static IDictionary<int, ILinearCalibration> lcDict;

        //Application Level settings holding atomic weights of chemical elements used in calculation
        static ClientSettingsSection elementsWeights;

        #region SchemeResults
        [NotMapped]
        public decimal CaSO4 { get; private set; }
        [NotMapped]
        public decimal Na { get; private set; }
        [NotMapped]
        public decimal? CrystWater { get; private set; }
        [NotMapped]
        public decimal Carnallite { get; private set; }
        [NotMapped]
        public decimal NaCl { get; private set; }
        [NotMapped]
        public decimal KCl { get; private set; }
        [NotMapped]
        public decimal CaCl2 { get; private set; }
        [NotMapped]
        public decimal? MgCl2 { get; private set; }
        [NotMapped]
        public decimal MgCl2AnyCase { get; private set; }
        [NotMapped]
        public decimal KBr { get; private set; }
        [NotMapped]
        public decimal? HygroWater { get; private set; }
        [NotMapped]
        public decimal HygroWaterAnyCase { get; private set; }
        #endregion SchemeResults
        #region AtomicWeights_Of_ChemicalElememts
        static decimal awMg;
        static decimal awCa;
        static decimal awH;
        static decimal awO;
        static decimal awCl;
        static decimal awNa;
        static decimal awK;
        static decimal awC;
        static decimal awS;
        static decimal awBr;
        static decimal awB;
        #endregion AtomicWeights_Of_ChemicalElememts
        #region Application level calculated consts
        private static decimal _water2MagnesiumRatioInCarnallite;
        private static decimal _SO4_2_CaS04;
        private static decimal _CaSO4_2_SO4;
        private static decimal _CaCl2_2_Ca;
        private static decimal _MgCl2_2_Mg;
        private static decimal _KCl_2_K;
        private static decimal _NaCl_2_Cl;
        private static decimal _KBr_2_Br;
        private static decimal _Carnallite_2_Magnesium;
        private static decimal _Eq_CO3;
        private static decimal _Eq_HCO3;
        private static decimal _Eq_Mg;
        private static decimal _Eq_Ca;
        private static decimal _Eq_SO4;
        //May be changed at the user level so need to hold for each instance separately
        private decimal carnalliteThreshold = 0.0008M;
        #endregion
        [NotMapped]
        public string AnalysisDescription { get; set; }

        public SaltAnalysisData()
        {
            if (lcDict is null) lcDict = new Dictionary<int, ILinearCalibration>();
            if (elementsWeights is null)
            {
                Uri UriAssemblyFolder = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
                string appPath = UriAssemblyFolder.LocalPath;

                //Open the configuration file and retrieve the applicationSettings section.
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(appPath + @"\ChemicalAnalyses.exe");
                    elementsWeights = (ClientSettingsSection)config.SectionGroups["applicationSettings"].Sections[0];
                }
                catch { }
                
                NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
                //read application level constants
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("Mg").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awMg))
                    awMg = 24.305M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("H").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awH))
                    awH = 1.008M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("O").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awO))
                    awO = 15.999M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("Ca").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awCa))
                    awCa = 40.078M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("Cl").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awCl))
                    awCl = 35.45M;
                _water2MagnesiumRatioInCarnallite = 6 * (2 * awH + awO) / awMg;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("Na").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awNa))
                    awNa = 23.99M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("K").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awK))
                    awK = 39.099M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("C").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awC))
                    awC = 12.011M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("S").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awS))
                    awS = 32.07M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("Br").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awBr))
                    awBr = 79.9M;
                if (elementsWeights == null || !decimal.TryParse(elementsWeights.Settings.Get("B").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awB))
                    awB = 10.81M;
                //ions ratio in minerals constants
                _SO4_2_CaS04 = (awS + 4 * awO) / (awS + 4 * awO + awCa);
                _CaSO4_2_SO4 = (awS + 4 * awO + awCa) / (awS + 4 * awO);
                _CaCl2_2_Ca = (awCa + awCl * 2) / awCa;
                _MgCl2_2_Mg = (awMg + awCl * 2) / awMg;
                _KCl_2_K = (awK + awCl) / awK;
                _NaCl_2_Cl = (awNa + awCl) / awCl;
                _KBr_2_Br = (awK + awBr) / awBr;
                _Carnallite_2_Magnesium = (12 * awH + 6 * awO + awMg + 3 * awCl + awK) / awMg;
                //mass percentage to normality concentration constants
                _Eq_CO3 = 1000 / (awC + 3 * awO);
                _Eq_HCO3 = 1000 / (awC + awH + 3 * awO);
                _Eq_Mg = 1000 / (awMg);
                _Eq_Ca = 1000 / (awCa);
                _Eq_SO4 = 1000 / (awS + 4 * awO);
            }
        }

        public SaltAnalysisData(IDictionary<int, ILinearCalibration> lc) : this()
        {
            if (lc != null) lcDict = lc;
        }

        public SaltAnalysisData(decimal CarnalliteThreshold) : this()
        {
            carnalliteThreshold = CarnalliteThreshold;
        }

        public SaltAnalysisData(IDictionary<int, ILinearCalibration> lc,
                    decimal CarnalliteThreshold) : this(CarnalliteThreshold)
        {
            if (lc != null) lcDict = lc;
        }

        public void CalcValues()
        {
            MgWet = (MagnesiumTitre * MagnesiumTrilonTitre / MagnesiumAliquote
                - CalciumTitre * CalciumTrilonTitre / CalciumAliquote)
                //0.0125 = 0.5 * 0.05 * 500 /1000
                * 0.0125M * awMg / WetWeight;

            HumidityContent = (MgWet >= carnalliteThreshold)
                        ? (HumidityCrucibleWetSampleWeight - ((HumidityCrucibleDry180SampleWeight.HasValue) ?
                        HumidityCrucibleDry180SampleWeight.Value : HumidityCrucibleDry110SampleWeight))
                        / (HumidityCrucibleWetSampleWeight - HumidityCrucibleEmptyWeight)
                        : (HumidityCrucibleWetSampleWeight - HumidityCrucibleDry110SampleWeight)
                        / (HumidityCrucibleWetSampleWeight - HumidityCrucibleEmptyWeight);
            //Set corrected dry weight of the sample depending on the scheme
            switch (DefaultCalculationScheme)
            {
                case SaltCalculationSchemes.Chloride:
                    SampleCorrectedDryWeight = (MgWet >= carnalliteThreshold)
                        ? NewDryWeight(WetWeight, MgWet, HumidityContent, _water2MagnesiumRatioInCarnallite)
                        : WetWeight * (1 - HumidityContent);
                    break;
                default://All other schemes
                    SampleCorrectedDryWeight = WetWeight * (1 - HumidityContent);
                    break;
            }

            MgDry = (MagnesiumTitre * MagnesiumTrilonTitre / MagnesiumAliquote
                - CalciumTitre * CalciumTrilonTitre / CalciumAliquote)
                * 0.0125M * awMg / SampleCorrectedDryWeight;

            CaDry = CalciumTitre * CalciumTrilonTitre * 0.0125M
                * awCa / (SampleCorrectedDryWeight * CalciumAliquote);

            ClDry = ChlorumTitre * awCl * HgCoefficient / (SampleCorrectedDryWeight * ChlorumAliquote * 20);

            BrDry = (BromumTitre - BromumBlank) * BromumStandardTitre * 0.5M
                / (SampleCorrectedDryWeight * BromumAliquote);

            ResiduumDry = (ResiduumCrucibleFullWeight - ResiduumCrucibleEmptyWeight) / SampleCorrectedDryWeight;

            SulfatesDry = (SulfatesCrucibleFullWeight - SulfatesCrucibleEmptyWeight - SulfatesBlank) *
                205.75M / (SampleCorrectedDryWeight * SulfatesAliquote);

            CarbonatesDry = CarbonatesTitre / (1000 * SampleCorrectedDryWeight);

            HydrocarbonatesDry = HydrocarbonatesTitre / (1000 * SampleCorrectedDryWeight);
        }

        public void CalcKaliumValue()
        {
            ILinearCalibration lc;
            if (!lcDict.ContainsKey(KaliumCalibration))
            {
                using (var context = new ChemicalAnalysesEntities())
                {
                    lc = context.LineaCalibrations.Find(KaliumCalibration);
                    (lc as LinearCalibration).GetLinearCoefficients();
                        //LinearCalibration.GetAllLC("[IDCalibration] = " + KaliumCalibration, true).FirstOrDefault();
                    lcDict.Add(KaliumCalibration, lc);
                }
            }
            else { lc = lcDict[KaliumCalibration]; }
            KDry = lc.ValueToConcentration(KaliumValue, KaliumDiapason - 1) * KaliumVolume
                / (2 * SampleCorrectedDryWeight);
        }
        public SaltCalculationSchemes CalcRecommendedScheme()
        {
            decimal Coeff1 = 0, Coeff2 = 0, Coeff3 = 0, Coeff4 = 0;
            try
            {
                Coeff1 = (CarbonatesDry * _Eq_CO3 + HydrocarbonatesDry * _Eq_HCO3) /
                  (CaDry * _Eq_Ca + MgDry * _Eq_Mg);
            }
            catch { }
            try
            {
                Coeff2 = (CarbonatesDry * _Eq_CO3 + HydrocarbonatesDry * _Eq_HCO3 + SulfatesDry * _Eq_SO4) /
                  (CaDry * _Eq_Ca + MgDry * _Eq_Mg);
            }
            catch { }
            try { Coeff3 = SulfatesDry * _Eq_SO4 / (CaDry * _Eq_Ca); }
            catch { }
            try { Coeff4 = (CarbonatesDry * _Eq_CO3 + HydrocarbonatesDry * _Eq_HCO3) / (CaDry * _Eq_Ca); }
            catch { }
            if (Coeff1 >= 1) return SaltCalculationSchemes.Carbonate;
            else if (Coeff2 < 1)
            {
                if (Coeff3 >= 1)
                {
                    if (Coeff4 < 1) return SaltCalculationSchemes.SulfateSodiumI;
                    else return SaltCalculationSchemes.SulfateSodiumII;
                }
                else return SaltCalculationSchemes.Chloride;
            }
            else
            {
                if (Coeff3 >= 1)
                {
                    if (Coeff4 < 1) return SaltCalculationSchemes.SulfateMagnesiumI;
                    else return SaltCalculationSchemes.SulfateMagnesiumII;
                }
                else throw new ArgumentOutOfRangeException("RecommendedCalculationScheme",
                     "Unknown calculation scheme results");
            }
        }

        public void CalcSchemeResults()
        {
            switch (DefaultCalculationScheme)
            {
                case SaltCalculationSchemes.Chloride:
                    CaSO4 = SulfatesDry * _CaSO4_2_SO4;                                     //1
                    decimal _Ca_bound2_SO4 = CaSO4 - SulfatesDry;                           //2
                    decimal _Ca_bound2_Cl = CaDry - _Ca_bound2_SO4;                         //3
                    CaCl2 = _Ca_bound2_Cl * _CaCl2_2_Ca;                                    //4
                    decimal _Cl_bound2_Ca = CaCl2 - _Ca_bound2_Cl;                          //5
                    MgCl2AnyCase = MgDry * _MgCl2_2_Mg;                                     //6 
                    MgCl2 = (MgWet >= carnalliteThreshold) ? MgDry *
                        _MgCl2_2_Mg : (decimal?)null;                                         //7
                    decimal _Cl_bound_2_Mg = (MgCl2.HasValue) ? MgCl2.Value : 0 - MgDry;    //8
                    KBr = BrDry * _KBr_2_Br;                                                //9
                    decimal _K_bound2_Br = KBr - BrDry;                                     //10
                    decimal _K_bound_2_Cl = KDry - _K_bound2_Br;                            //11
                    KCl = _K_bound_2_Cl * _KCl_2_K;                                         //12
                    decimal _Cl_bound2_K = KCl - _K_bound_2_Cl;                             //13
                    decimal _summaryCl = _Cl_bound2_Ca + _Cl_bound_2_Mg + _Cl_bound2_K;     //14
                    decimal _CL_bound2_Na = ClDry - _summaryCl;                             //15
                    Na = _CL_bound2_Na * awNa / awCl;                                       //16
                    NaCl = Na + _CL_bound2_Na;                                              //17
                    CrystWater = (MgWet >= carnalliteThreshold)
                       ? MgDry * _water2MagnesiumRatioInCarnallite
                       : (decimal?)null;
                    HygroWater = HumidityContent - CrystWater;                              //Non-BK
                    Carnallite = MgDry * _Carnallite_2_Magnesium;
                    HygroWaterAnyCase = (MgWet >= carnalliteThreshold) ?
                        HumidityContent - MgDry * _water2MagnesiumRatioInCarnallite : HumidityContent;
                    break;
                default: //All others - not yet implemented
                    break;
            }
        }

        /// <summary>
        /// recursive calculation of sample dry weight with WaterInCarnallite as a criteria
        /// </summary>
        /// <param name="SampleWet"> Wet (not yet dry) weight of the sample</param>
        /// <param name="MgWet"> Wet (not yet dry) weight of Mg</param>
        /// <param name="Humidity180"></param>
        /// <param name="WaterInCarnallite">Constant defining water content in carnallite</param>
        /// <returns> Corrected dry weight of the sample</returns>
        private decimal NewDryWeight(decimal SampleWet, decimal MgWet,
            decimal Humidity180, decimal WaterInCarnallite)
        {
            decimal WCryst2, WHygr2, SampleDry2, MgDry1, MgDry2;

            MgDry1 = MgWet;
            WCryst2 = Math.Round(MgDry1 * WaterInCarnallite, 4);
            WHygr2 = Humidity180 - WCryst2;
            SampleDry2 = SampleWet * (1 - WHygr2);
            MgDry2 = MgWet * SampleWet / SampleDry2;
            if (Math.Abs(MgDry2 - MgDry1) > 0.0001M)
                return NewDryWeight(SampleDry2, MgDry2, Humidity180, WaterInCarnallite);
            else return SampleDry2;
        }
    }

    /// <summary>
    /// Enumerates possible calculation schemes
    /// </summary>
    public enum SaltCalculationSchemes
    {
        /// <summary>
        /// Carbonate calculation scheme
        /// </summary>
        Carbonate,
        /// <summary>
        /// The first variant of Sulfate-Sodium calculation scheme
        /// </summary>
        SulfateSodiumI,
        /// <summary>
        /// The second variant of Sulfate-Sodium calculation scheme
        /// </summary>
        SulfateSodiumII,
        /// <summary>
        /// The first variant of Sulfate-Magnesium calculation scheme
        /// </summary>
        SulfateMagnesiumI,
        /// <summary>
        /// The second variant of Sulfate-Magnesium calculation scheme
        /// </summary>
        SulfateMagnesiumII,
        /// <summary>
        /// Chloride calculation scheme
        /// </summary>
        Chloride
    };
}
