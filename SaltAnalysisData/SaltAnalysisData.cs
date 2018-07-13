using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using SettingsHelper;
using System.Configuration;
using System.Reflection;
using System.Globalization;
using System.IO;
using Calibration;

namespace SaltAnalysisDatas
{
    public partial class SaltAnalysisData : INotifyPropertyChanged
    {
        static SqlConnection connection;

        public SaltAnalysisData()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
        }

        public SaltAnalysisData(decimal CarnalliteThreshold):this()
        {
            carnalliteThreshold = CarnalliteThreshold;
        }

        static SaltAnalysisData()
        {
            if (connection is null) connection = new SqlConnection(ConnectionStringGiver.GetValidConnectionString(String.Empty));
            if (lcDict is null) lcDict = new Dictionary<int, ILinearCalibration>();
            if (elementsWeights is null)
            {
                Uri UriAssemblyFolder = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
                string appPath = UriAssemblyFolder.LocalPath;
                
                //Open the configuration file and retrieve the applicationSettings section.
                Configuration config = ConfigurationManager.OpenExeConfiguration(appPath + @"\ChemicalAnalyses.exe");
                elementsWeights = (ClientSettingsSection)config.SectionGroups["applicationSettings"].Sections[0];
                NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
                //read application level constants
                if (!decimal.TryParse(elementsWeights.Settings.Get("Mg").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awMg))
                    awMg = 24.305M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("H").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awH))
                    awH = 1.008M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("O").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awO))
                    awO = 15.999M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("Ca").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awCa))
                    awCa = 40.078M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("Cl").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awCl))
                    awCl = 35.45M;
                _water2MagnesiumRatioInCarnallite = 6 * (2 * awH + awO) / awMg;
                if (!decimal.TryParse(elementsWeights.Settings.Get("Na").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awNa))
                    awNa = 23.99M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("K").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awK))
                    awK = 39.099M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("C").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awC))
                    awC = 12.011M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("S").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awS))
                    awS = 32.07M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("Br").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awBr))
                    awBr = 79.9M;
                if (!decimal.TryParse(elementsWeights.Settings.Get("B").Value.ValueXml.InnerText, NumberStyles.Number, nfi, out awB))
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

        public static void Delete(int idsmpl)
        {
            SqlTransaction sqlTran = null;
            var commandString = "DELETE FROM SaltAnalysis WHERE(IDSaltAnalysis=@iDSaltAnalysis)";
            SqlCommand deleteCommand = new SqlCommand(commandString, connection);
            deleteCommand.Parameters.AddWithValue("iDSaltAnalysis", idsmpl);
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                sqlTran = connection.BeginTransaction();
                deleteCommand.Transaction = sqlTran;
                deleteCommand.ExecuteScalar();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        public void Update()
        {
            var commandString = "UPDATE SaltAnalysis SET AnalysisDate=@AnalysisDate " +
                ", BromumAliquote=@BromumAliquote, BromumBlank=@BromumBlank " +
                ", BromumStandardTitre=@BromumStandardTitre, BromumTitre=@BromumTitre " +
                ", CalciumAliquote=@CalciumAliquote, CalciumTitre=@CalciumTitre, CalciumTrilonTitre=@CalciumTrilonTitre " +
                ", CarbonatesTitre=@CarbonatesTitre, ChlorumAliquote=@ChlorumAliquote, ChlorumTitre=@ChlorumTitre " +
                ", HgCoefficient=@HgCoefficient, HumidityCrucibleDry110SampleWeight=@HumidityCrucibleDry110SampleWeight " +
                ", HumidityCrucibleDry180SampleWeight=@HumidityCrucibleDry180SampleWeight " +
                ", HumidityCrucibleEmptyWeight=@HumidityCrucibleEmptyWeight " +
                ", HumidityCrucibleWetSampleWeight=@HumidityCrucibleWetSampleWeight " +
                ", HydrocarbonatesTitre=@HydrocarbonatesTitre, IDSample=@IDSample, KaliumCalibration=@KaliumCalibration " +
                ", KaliumConcentration=@KaliumConcentration, KaliumDiapason=@KaliumDiapason " +
                ", KaliumValue=@KaliumValue, KaliumVolume=@KaliumVolume, MagnesiumAliquote=@MagnesiumAliquote " +
                ", MagnesiumTrilonTitre=@MagnesiumTrilonTitre, MagnesiumTitre=@MagnesiumTitre " +
                ", ResiduumCrucibleEmptyWeight=@ResiduumCrucibleEmptyWeight " +
                ", ResiduumCrucibleFullWeight=@ResiduumCrucibleFullWeight " +
                ", SulfatesAliquote=@SulfatesAliquote " +
                ", SulfatesBlank=@SulfatesBlank, SulfatesCrucibleEmptyWeight=@SulfatesCrucibleEmptyWeight " +
                ", SulfatesCrucibleFullWeight=@SulfatesCrucibleFullWeight, WetWeight=@WetWeight " +
                "WHERE (IDSaltAnalysis = @iDSaltAnalysis)";
            SqlCommand updateCommand = new SqlCommand(commandString, connection);
            SqlTransaction sqlTran = null;
            if (HumidityCrucibleDry180SampleWeight.HasValue)
                updateCommand.Parameters.Add(new SqlParameter("HumidityCrucibleDry180SampleWeight", HumidityCrucibleDry180SampleWeight));
            else updateCommand.Parameters.Add(new SqlParameter("HumidityCrucibleDry180SampleWeight", DBNull.Value));
            
            updateCommand.Parameters.AddRange(new SqlParameter[]
        {
            new SqlParameter ("AnalysisDate", AnalysisDate),
            new SqlParameter ("BromumAliquote",BromumAliquote),
            new SqlParameter ("BromumBlank",BromumBlank),
            new SqlParameter ("BromumStandardTitre",BromumStandardTitre),
            new SqlParameter ("BromumTitre", BromumTitre),
            new SqlParameter ("CalciumAliquote", CalciumAliquote),
            new SqlParameter ("CalciumTitre", CalciumTitre),
            new SqlParameter ("CalciumTrilonTitre", CalciumTrilonTitre),
            new SqlParameter ("CarbonatesTitre", CarbonatesTitre),
            new SqlParameter ("ChlorumAliquote", ChlorumAliquote),
            new SqlParameter ("ChlorumTitre", ChlorumTitre),
            new SqlParameter ("HgCoefficient",HgCoefficient),
            new SqlParameter ("HumidityCrucibleDry110SampleWeight",HumidityCrucibleDry110SampleWeight),
            new SqlParameter ("HumidityCrucibleEmptyWeight",HumidityCrucibleEmptyWeight),
            new SqlParameter ("HumidityCrucibleWetSampleWeight",HumidityCrucibleWetSampleWeight),
            new SqlParameter ("HydrocarbonatesTitre",HydrocarbonatesTitre),
            new SqlParameter ("IDSample",IDSample),
            new SqlParameter ("KaliumCalibration",KaliumCalibration),
            new SqlParameter ("KaliumConcentration",KaliumConcentration),
            new SqlParameter ("KaliumDiapason",KaliumDiapason),
            new SqlParameter ("KaliumValue",KaliumValue),
            new SqlParameter ("KaliumVolume", KaliumVolume),
            new SqlParameter ("MagnesiumAliquote", MagnesiumAliquote),
            new SqlParameter ("MagnesiumTrilonTitre",MagnesiumTrilonTitre),
            new SqlParameter ("MagnesiumTitre",MagnesiumTitre),
            new SqlParameter ("ResiduumCrucibleEmptyWeight", ResiduumCrucibleEmptyWeight),
            new SqlParameter ("ResiduumCrucibleFullWeight", ResiduumCrucibleFullWeight),
            new SqlParameter ("SulfatesAliquote", SulfatesAliquote),
            new SqlParameter ("SulfatesBlank" , SulfatesBlank),
            new SqlParameter ("SulfatesCrucibleEmptyWeight", SulfatesCrucibleEmptyWeight),
            new SqlParameter ("SulfatesCrucibleFullWeight", SulfatesCrucibleFullWeight),
            new SqlParameter ("WetWeight", WetWeight),
            new SqlParameter ("iDSaltAnalysis",IDSaltAnalysis)
        });
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                sqlTran = connection.BeginTransaction();
                updateCommand.Transaction = sqlTran;
                updateCommand.ExecuteScalar();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                throw new Exception(ex.Message, ex);//Bubble up the original exception
            }
            finally
            {
                connection.Close(); //Close connection even if the querry failed
            }
        }

        public void Insert()
        {
            var commandString = "INSERT INTO SaltAnalysis (AnalysisDate, BromumAliquote, BromumBlank, " +
                "BromumStandardTitre, BromumTitre, CalciumAliquote, CalciumTitre, CalciumTrilonTitre," +
                "CarbonatesTitre, ChlorumAliquote, ChlorumTitre, HgCoefficient, HumidityCrucibleDry110SampleWeight," +
                "HumidityCrucibleDry180SampleWeight, HumidityCrucibleEmptyWeight, HumidityCrucibleWetSampleWeight," +
                "HydrocarbonatesTitre, IDSample, KaliumCalibration, KaliumConcentration, KaliumDiapason," +
                "KaliumValue, KaliumVolume, MagnesiumAliquote, MagnesiumTrilonTitre, MagnesiumTitre," +
                "ResiduumCrucibleEmptyWeight, ResiduumCrucibleFullWeight, SulfatesAliquote, SulfatesBlank," +
                "SulfatesCrucibleEmptyWeight, SulfatesCrucibleFullWeight, WetWeight)" +
                "VALUES (@AnalysisDate, @BromumAliquote, @BromumBlank, " +
                "@BromumStandardTitre, @BromumTitre, @CalciumAliquote, @CalciumTitre, @CalciumTrilonTitre," +
                "@CarbonatesTitre, @ChlorumAliquote, @ChlorumTitre, @HgCoefficient, @HumidityCrucibleDry110SampleWeight," +
                "@HumidityCrucibleDry180SampleWeight, @HumidityCrucibleEmptyWeight, @HumidityCrucibleWetSampleWeight," +
                "@HydrocarbonatesTitre, @IDSample, @KaliumCalibration, @KaliumConcentration, @KaliumDiapason," +
                "@KaliumValue, @KaliumVolume, @MagnesiumAliquote, @MagnesiumTrilonTitre, @MagnesiumTitre," +
                "@ResiduumCrucibleEmptyWeight, @ResiduumCrucibleFullWeight, @SulfatesAliquote, @SulfatesBlank," +
                "@SulfatesCrucibleEmptyWeight, @SulfatesCrucibleFullWeight, @WetWeight)";
            SqlCommand insertCommand = new SqlCommand(commandString, connection);
            SqlTransaction sqlTran = null;
            if (HumidityCrucibleDry180SampleWeight.HasValue)
                insertCommand.Parameters.Add(new SqlParameter("HumidityCrucibleDry180SampleWeight", HumidityCrucibleDry180SampleWeight));
            else insertCommand.Parameters.Add(new SqlParameter("HumidityCrucibleDry180SampleWeight", DBNull.Value));
            insertCommand.Parameters.AddRange(new SqlParameter[]
        {
            new SqlParameter ("AnalysisDate", AnalysisDate),
            new SqlParameter ("BromumAliquote",BromumAliquote),
            new SqlParameter ("BromumBlank",BromumBlank),
            new SqlParameter ("BromumStandardTitre",BromumStandardTitre),
            new SqlParameter ("BromumTitre", BromumTitre),
            new SqlParameter ("CalciumAliquote", CalciumAliquote),
            new SqlParameter ("CalciumTitre", CalciumTitre),
            new SqlParameter ("CalciumTrilonTitre", CalciumTrilonTitre),
            new SqlParameter ("CarbonatesTitre", CarbonatesTitre),
            new SqlParameter ("ChlorumAliquote", ChlorumAliquote),
            new SqlParameter ("ChlorumTitre", ChlorumTitre),
            new SqlParameter ("HgCoefficient",HgCoefficient),
            new SqlParameter ("HumidityCrucibleDry110SampleWeight",HumidityCrucibleDry110SampleWeight),
            new SqlParameter ("HumidityCrucibleEmptyWeight",HumidityCrucibleEmptyWeight),
            new SqlParameter ("HumidityCrucibleWetSampleWeight",HumidityCrucibleWetSampleWeight),
            new SqlParameter ("HydrocarbonatesTitre",HydrocarbonatesTitre),
            new SqlParameter ("IDSample",IDSample),
            new SqlParameter ("KaliumCalibration",KaliumCalibration),
            new SqlParameter ("KaliumConcentration",KaliumConcentration),
            new SqlParameter ("KaliumDiapason",KaliumDiapason),
            new SqlParameter ("KaliumValue",KaliumValue),
            new SqlParameter ("KaliumVolume", KaliumVolume),
            new SqlParameter ("MagnesiumAliquote", MagnesiumAliquote),
            new SqlParameter ("MagnesiumTrilonTitre",MagnesiumTrilonTitre),
            new SqlParameter ("MagnesiumTitre",MagnesiumTitre),
            new SqlParameter ("ResiduumCrucibleEmptyWeight", ResiduumCrucibleEmptyWeight),
            new SqlParameter ("ResiduumCrucibleFullWeight", ResiduumCrucibleFullWeight),
            new SqlParameter ("SulfatesAliquote", SulfatesAliquote),
            new SqlParameter ("SulfatesBlank" , SulfatesBlank),
            new SqlParameter ("SulfatesCrucibleEmptyWeight", SulfatesCrucibleEmptyWeight),
            new SqlParameter ("SulfatesCrucibleFullWeight", SulfatesCrucibleFullWeight),
            new SqlParameter ("WetWeight", WetWeight)
        });
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) return;

            try
            {
                sqlTran = connection.BeginTransaction();
                insertCommand.Transaction = sqlTran;
                insertCommand.ExecuteNonQuery();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                throw new Exception(ex.Message, ex);//Bubble up the original exception
            }
            finally
            {
                connection.Close(); //Close connection even if the querry failed
            }
        }

        public static IEnumerable<SaltAnalysisData> GetAllSamples(string conditionString)
        {
            string commandString = "SELECT * FROM [SaltAnalysisView] "
                + ((conditionString != null && conditionString != "") ? ("WHERE " + conditionString) : "");
            SqlCommand getAllSaltAnalysesCommand = new SqlCommand(commandString, connection);

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot establish connection!", MessageBoxButton.OK);
            }

            if (connection.State != ConnectionState.Open) yield break;


            using (SqlDataReader reader = getAllSaltAnalysesCommand.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var iDSaltAnalysis = reader.GetInt32(0);
                        var iDSample = reader.GetInt32(1);
                        var labNumber = reader.GetString(2);
                        var wetWeight = reader.GetDecimal(3);
                        var magnesiumTitre = reader.GetDecimal(4);
                        var magnesiumAliquote = reader.GetDecimal(5);
                        var magnesiumTrilonTitre = reader.GetDecimal(6);
                        var kaliumCalibration = reader.GetInt32(7);
                        var analysisDate = reader.GetDateTime(8);
                        var kaliumDiapason = reader.GetInt32(9);
                        var kaliumConcentration = reader.GetDecimal(10);
                        var sulfatesCrucibleEmptyWeight = reader.GetDecimal(11);
                        var sulfatesCrucibleFullWeight = reader.GetDecimal(12);
                        var residuumCrucibleFullWeight = reader.GetDecimal(13);
                        var residuumCrucibleEmptyWeight = reader.GetDecimal(14);
                        var humidityCrucibleEmptyWeight = reader.GetDecimal(15);
                        var humidityCrucibleWetSampleWeight = reader.GetDecimal(16);
                        var humidityCrucibleDry110Weight = reader.GetDecimal(17);
                        var humidityCrucibleDry180Weight = reader.GetSqlDecimal(18);
                        var calciumTitre = reader.GetDecimal(19);
                        var chlorumTitre = reader.GetDecimal(20);
                        var bromumTitre = reader.GetDecimal(21);
                        var kaliumValue = reader.GetDecimal(22);
                        var carbonatesTitre = reader.GetDecimal(23);
                        var hydrocarbonatesTitre = reader.GetDecimal(24);
                        var calciumTrilonTitre = reader.GetDecimal(25);
                        var calciumAliquote = reader.GetDecimal(26);
                        var bromumAliquote = reader.GetDecimal(27);
                        var sulfatesAaliquote = reader.GetDecimal(28);
                        var kaliumVolume = reader.GetDecimal(29);
                        var hgCoefficient = reader.GetDecimal(30);
                        var bromumBlank = reader.GetDecimal(31);
                        var bromumStandardTitre = reader.GetDecimal(32);
                        var sulfatesBlank = reader.GetDecimal(33);
                        var chlorumAliquote = reader.GetDecimal(34);

                        var sample = new SaltAnalysisData
                        {
                            IDSaltAnalysis = iDSaltAnalysis,
                            IDSample = iDSample,
                            LabNumber = labNumber.Trim(),
                            WetWeight = wetWeight,
                            MagnesiumTitre = magnesiumTitre,
                            MagnesiumAliquote = magnesiumAliquote,
                            MagnesiumTrilonTitre = magnesiumTrilonTitre,
                            KaliumCalibration = kaliumCalibration,
                            AnalysisDate = analysisDate,
                            KaliumDiapason = kaliumDiapason,
                            KaliumConcentration = kaliumConcentration,
                            SulfatesCrucibleEmptyWeight = sulfatesCrucibleEmptyWeight,
                            SulfatesCrucibleFullWeight = sulfatesCrucibleFullWeight,
                            ResiduumCrucibleFullWeight = residuumCrucibleFullWeight,
                            ResiduumCrucibleEmptyWeight = residuumCrucibleEmptyWeight,
                            HumidityCrucibleEmptyWeight = humidityCrucibleEmptyWeight,
                            HumidityCrucibleWetSampleWeight = humidityCrucibleWetSampleWeight,
                            HumidityCrucibleDry110SampleWeight = humidityCrucibleDry110Weight,
                            HumidityCrucibleDry180SampleWeight =
                                humidityCrucibleDry180Weight.IsNull ? (decimal?)null
                                : humidityCrucibleDry180Weight.Value,
                            CalciumTitre = calciumTitre,
                            ChlorumTitre = chlorumTitre,
                            BromumTitre = bromumTitre,
                            KaliumValue = kaliumValue,
                            CarbonatesTitre = carbonatesTitre,
                            HydrocarbonatesTitre = hydrocarbonatesTitre,
                            CalciumTrilonTitre = calciumTrilonTitre,
                            CalciumAliquote = calciumAliquote,
                            BromumAliquote = bromumAliquote,
                            SulfatesAliquote = sulfatesAaliquote,
                            KaliumVolume = kaliumVolume,
                            HgCoefficient = hgCoefficient,
                            BromumBlank = bromumBlank,
                            BromumStandardTitre = bromumStandardTitre,
                            SulfatesBlank = sulfatesBlank,
                            ChlorumAliquote = chlorumAliquote
                        };
                        yield return sample;
                    }
                };
            }
            connection.Close();
        }

        public int IDSaltAnalysis { get; set; }
        public int IDSample { get; set; }
        public string LabNumber { get; set; } //just to show in the datagrid
        //1
        private decimal _wetWeight = 4;
        public decimal WetWeight
        {
            get { return _wetWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("WetWeight",
                    "Значение сырой навески не может быть отрицательным числом!");
                _wetWeight = value;
                OnPropertyChanged("WetWeight");
            }
        }
        //2
        private decimal _magnesiumTitre = 1;
        public decimal MagnesiumTitre
        {
            get { return _magnesiumTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MagnesiumTitre", 
                    "Значение титра не может быть отрицательным числом");
                _magnesiumTitre = value;
                OnPropertyChanged("MagnesiumTitre");
            }
        }
        //3
        private decimal _magnesiumAliquote = 50;
        public decimal MagnesiumAliquote
        {
            get { return _magnesiumAliquote; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MagnesiumAliquote", 
                    "Значение аликвоты не может быть отрицательным числом");
                _magnesiumAliquote = value;
                OnPropertyChanged("MagnesiumAliquote");
            }
        }
        //4
        private decimal _magnesiumTrilonTitre = 1;
        public decimal MagnesiumTrilonTitre
        {
            get { return _magnesiumTrilonTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("MagnesiumTrilonTitre", 
                    "Значение титра не может быть отрицательным числом");
                _magnesiumTrilonTitre = value;
                OnPropertyChanged("MagnesiumTrilonTitre");
            }
        }
        private int _kaliumCalibration = 1;
        public int KaliumCalibration
        {
            get { return _kaliumCalibration; }
            set
            {
                _kaliumCalibration = value;
                OnPropertyChanged("KaliumCalibration");
            }
        }
        private DateTime _analysisdate = DateTime.Today;
        public DateTime AnalysisDate
        {
            get { return _analysisdate; }
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentOutOfRangeException("AnalysisDate", 
                        "Дата анализа не может лежать в будущем!");
                _analysisdate = value;
                OnPropertyChanged("AnalysisDate");
            }
        }
        private int _kaliumDiapason = 1;
        public int KaliumDiapason
        {
            get { return _kaliumDiapason; }
            set
            {
                if (!(value == 1 || value == 2))
                    throw new ArgumentOutOfRangeException("KaliumDiapason", 
                        "Значение диапазона 1 или 2");
                _kaliumDiapason = value;
                OnPropertyChanged("KaliumDiapason");
            }
        }
        private decimal _kaliumConcentration; // calculated volume no check
        public decimal KaliumConcentration
        {
            get { return _kaliumConcentration; }
            set
            {
                _kaliumConcentration = value;
                OnPropertyChanged("KaliumConcentration");
            }
        }
        private decimal _sulfatesCrucibleEmptyWeight = 10;
        public decimal SulfatesCrucibleEmptyWeight
        {
            get { return _sulfatesCrucibleEmptyWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("SulfatesCrucibleEmptyWeight",
                    "Значение веса пустого тигля не может быть отрицательным числом!");
                _sulfatesCrucibleEmptyWeight = value;
                OnPropertyChanged("SulfatesCrucibleEmptyWeight");
            }
        }

        private decimal _sulfatesCrucibleFullWeight = 12;
        public decimal SulfatesCrucibleFullWeight
        {
            get { return _sulfatesCrucibleFullWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("SulfatesCrucibleFullWeight",
                    "Значение веса тигля с осадком не может быть отрицательным числом!");
                if (value <= SulfatesCrucibleEmptyWeight) throw new ArgumentOutOfRangeException("SulfatesCrucibleFullWeight",
                   "Значение веса тигля с осадком не может быть меньшим или равным весу пустого тигля!");
                _sulfatesCrucibleFullWeight = value;
                OnPropertyChanged("SulfatesCrucibleFullWeight");
            }
        }

        private decimal _residuumCrucibleFullWeight = 15;
        public decimal ResiduumCrucibleFullWeight
        {
            get { return _residuumCrucibleFullWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("ResiduumCrucibleFullWeight",
                    "Значение веса тигля с осадком не может быть отрицательным числом!");
                if (value <= ResiduumCrucibleEmptyWeight) throw new ArgumentOutOfRangeException("ResiduumCrucibleFullWeight",
                    "Значение веса тигля с осадком не может быть меньше или равно весу пустого тигля!");
                _residuumCrucibleFullWeight = value;
                OnPropertyChanged("ResiduumCrucibleFullWeight");
            }
        }

        private decimal _residuumCrucibleEmptyWeight = 10;
        public decimal ResiduumCrucibleEmptyWeight
        {
            get { return _residuumCrucibleEmptyWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("ResiduumCrucibleEmptyWeight",
                    "Значение веса пустого бюкса не может быть отрицательным числом!");
                _residuumCrucibleEmptyWeight = value;
                OnPropertyChanged("ResiduumCrucibleEmptyWeight");
            }
        }

        private decimal _humidityCrucibleEmptyWeight = 10;
        public decimal HumidityCrucibleEmptyWeight
        {
            get { return _humidityCrucibleEmptyWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("HumidityCrucibleEmptyWeight",
                    "Значение веса пустого тигля не может быть отрицательным числом!");
                _humidityCrucibleEmptyWeight = value;
                OnPropertyChanged("HumidityCrucibleEmptyWeight");
            }
        }

        private decimal _humidityCrucibleWetSampleWeight = 15;
        public decimal HumidityCrucibleWetSampleWeight
        {
            get { return _humidityCrucibleWetSampleWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("HumidityCrucibleWetSampleWeight",
                    "Значение веса тигля с сырой навеской не может быть отрицательным числом!");
                if (value <= HumidityCrucibleEmptyWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleWetSampleWeight",
                    "Значение веса тигля с сырой навеской не может быть меньшим или равным весу пустого тигля!");
                _humidityCrucibleWetSampleWeight = value;
                OnPropertyChanged("HumidityCrucibleWetSampleWeight");
            }
        }

        private decimal _humidityCrucibleDry110SampleWeight = 14;
        public decimal HumidityCrucibleDry110SampleWeight
        {
            get { return _humidityCrucibleDry110SampleWeight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("HumidityCrucibleDry110SampleWeight",
                    "Значение веса тигля с сухой (110) навеской не может быть равным нулю или отрицательным числом!");
                if (value <= HumidityCrucibleEmptyWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleDry110SampleWeight",
                    "Значение веса тигля с сухой (110) навеской не может быть меньшим или равным весу пустого тигля!");
                if (value > HumidityCrucibleWetSampleWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleDry110SampleWeight",
                    "Значение веса тигля с сухой (110) навеской не может быть большим веса тигля с сырой навеской!");
                _humidityCrucibleDry110SampleWeight = value;
                OnPropertyChanged("HumidityCrucibleDry110SampleWeight");
            }
        }

        private decimal? _humidityCrucibleDry180SampleWeight;
        public decimal? HumidityCrucibleDry180SampleWeight
        {
            get { return this._humidityCrucibleDry180SampleWeight; }
            set
            {
                if (value != null)
                {
                    if (value < 0) throw new ArgumentOutOfRangeException("HumidityCrucibleDry180SampleWeight",
                        "Значение веса тигля с сухой (180) навеской не может быть равным нулю или отрицательным числом!");
                    if (value <= HumidityCrucibleEmptyWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleDry180SampleWeight",
                        "Значение веса тигля с сухой (180) навеской не может быть меньшим или равным весу пустого тигля!");
                    if (value > HumidityCrucibleWetSampleWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleDry180SampleWeight",
                        "Значение веса тигля с сухой (180) навеской не может быть большим веса тигля с сырой навеской!");
                    if (value > HumidityCrucibleDry110SampleWeight) throw new ArgumentOutOfRangeException("HumidityCrucibleDry180SampleWeight",
                        "Значение веса тигля с сухой (180) навеской не может быть большим веса тигля с навеской при 110!");
                    _humidityCrucibleDry180SampleWeight = value;
                    OnPropertyChanged("HumidityCrucibleDry180SampleWeight");
                }
                else _humidityCrucibleDry180SampleWeight = null;
                OnPropertyChanged("HumidityCrucibleDry180SampleWeight");
            }
        }
        private decimal _calciumTitre = 5;
        public decimal CalciumTitre
        {
            get { return _calciumTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("CalciumTitre", 
                    "Значение титра не может быть отрицательным числом");
                _calciumTitre = value;
                OnPropertyChanged("CalciumTitre");
            }
        }

        private decimal _chlorumTitre = 3;
        public decimal ChlorumTitre
        {
            get { return _chlorumTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("ChlorumTitre", 
                    "Значение титра не может быть отрицательным числом");
                _chlorumTitre = value;
                OnPropertyChanged("ChlorumTitre");
            }
        }

        private decimal _bromumTitre = 7;
        public decimal BromumTitre
        {
            get { return _bromumTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("BromumTitre", 
                    "Значение титра не может быть отрицательным числом");
                _bromumTitre = value;
                OnPropertyChanged("BromumTitre");
            }
        }

        private decimal _kaliumValue = 10;
        public decimal KaliumValue
        {
            get { return _kaliumValue; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("KaliumValue", 
                    "Значение показаний не может быть отрицательным числом");
                _kaliumValue = value;
                OnPropertyChanged("KaliumValue");
            }
        }

        private decimal _carbonatesTitre = 0;
        public decimal CarbonatesTitre
        {
            get { return _carbonatesTitre; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("CarbonatesTitre", 
                    "Значение титра не может быть отрицательным числом");
                _carbonatesTitre = value;
                OnPropertyChanged("CarbonatesTitre");
            }
        }

        private decimal _hydrocarbonatesTitre = 0;
        public decimal HydrocarbonatesTitre
        {
            get { return _hydrocarbonatesTitre; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("HydrocarbonatesTitre", 
                    "Значение титра не может быть отрицательным числом");
                _hydrocarbonatesTitre = value;
                OnPropertyChanged("HydrocarbonatesTitre");
            }
        }

        private decimal _calciumTrilonTitre = 1;
        public decimal CalciumTrilonTitre
        {
            get { return _calciumTrilonTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("CalciumTrilonTitre", 
                    "Значение титра не может быть отрицательным числом");
                _calciumTrilonTitre = value;
                OnPropertyChanged("CalciumTrilonTitre");
            }
        }

        private decimal _calciumAliquote = 50;
        public decimal CalciumAliquote
        {
            get { return _calciumAliquote; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("CalciumAliquote", 
                    "Значение аликвоты не может быть отрицательным числом");
                _calciumAliquote = value;
                OnPropertyChanged("CalciumAliquote");
            }
        }

        private decimal _chlorumAliquote = 5;
        public decimal ChlorumAliquote
        {
            get { return _chlorumAliquote; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("ChlorumAliquote", 
                    "Значение аликвоты не может быть отрицательным числом");
                _chlorumAliquote = value;
                OnPropertyChanged("ChlorumAliquote");
            }
        }

        private decimal _bromumAliquote = 50;
        public decimal BromumAliquote
        {
            get { return _bromumAliquote; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("BromumAliquote", 
                    "Значение аликвоты не может быть отрицательным числом");
                _bromumAliquote = value;
                OnPropertyChanged("BromumAliquote");
            }
        }

        private decimal _sulfatesAliquote = 100;
        public decimal SulfatesAliquote
        {
            get { return _sulfatesAliquote; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("SulfatesAliquote", 
                    "Значение аликвоты не может быть отрицательным числом");
                _sulfatesAliquote = value;
                OnPropertyChanged("SulfatesAliquote");
            }
        }

        private decimal _kaliumVolume = 1;
        public decimal KaliumVolume
        {
            get { return _kaliumVolume; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("KaliumVolume", 
                    "Значение аликвоты не может быть отрицательным числом");
                _kaliumVolume = value;
                OnPropertyChanged("KaliumVolume");
            }
        }
        private decimal _hgCoefficient;
        public decimal HgCoefficient
        {
            get {return _hgCoefficient; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("HgCoefficient", 
                    "Значение параметра не может быть отрицательным либо равным 0!");
                _hgCoefficient = value;
                OnPropertyChanged("HgCoefficient");
            }
        }

        private decimal _bromumBlank = 1;
        public decimal BromumBlank
        {
            get { return _bromumBlank; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("BromumBlank", 
                    "Значение не может быть отрицательным!");
                _bromumBlank = value;
                OnPropertyChanged("BromumBlank");
            }
        }

        private decimal _bromumStandardTitre = (decimal)0.1332;
        public decimal BromumStandardTitre
        {
            get { return _bromumStandardTitre; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("BromumStandardTitre", 
                    "Значение не может быть отрицательным!");
                _bromumStandardTitre = value;
                OnPropertyChanged("BromumStandardTitre");
            }
        }

        private decimal _sulfatesBlank = (decimal)0.001;
        public decimal SulfatesBlank
        {
            get { return _sulfatesBlank; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("SulfatesBlank", 
                    "Значение не может быть отрицательным!");
                _sulfatesBlank = value;
                OnPropertyChanged("SulfatesBlank");
            }
        }
        private SaltCalculationSchemes _defaultClaculationScheme = 
            SaltCalculationSchemes.Chloride;
        public SaltCalculationSchemes DefaultCalculationScheme
        {
            get { return _defaultClaculationScheme; }
            set
            {
                if (value != SaltCalculationSchemes.Chloride)
                    throw new NotImplementedException("Доступна только хлоридная схема");
                _defaultClaculationScheme = value;
                OnPropertyChanged("DefaultCalculationScheme");
            }
        }
        private SaltCalculationSchemes _recommendedCalculationScheme = SaltCalculationSchemes.Chloride;
        public SaltCalculationSchemes RecommendedCalculationScheme
        {
            get { return _recommendedCalculationScheme; }
            set
            {
                _recommendedCalculationScheme = value;
                OnPropertyChanged("RecommendedCalculationScheme");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}