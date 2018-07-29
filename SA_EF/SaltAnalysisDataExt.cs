using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SA_EF
{
    public partial class SaltAnalysisData : INotifyPropertyChanged
    {
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
        private Nullable<decimal> _kaliumConcentration;
        public Nullable<decimal> KaliumConcentration
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
            get { return _hgCoefficient; }
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
        [NotMapped]
        public SaltCalculationSchemes DefaultCalculationScheme
        {
            get { return _defaultClaculationScheme; }
            set
            {
                if (!(value == SaltCalculationSchemes.Chloride 
                    || value == SaltCalculationSchemes.SulfateSodiumI
                    || value == SaltCalculationSchemes.SulfateMagnesiumI))
                    throw new NotImplementedException(
                        "На данный момент доступны только хлоридная, сульфатно-натриевая(I) и сульфатно-магниевая(I) схемы");
                _defaultClaculationScheme = value;
                OnPropertyChanged("DefaultCalculationScheme");
            }
        }
        private SaltCalculationSchemes _recommendedCalculationScheme = SaltCalculationSchemes.Chloride;
        [NotMapped]
        public SaltCalculationSchemes RecommendedCalculationScheme
        {
            get { return _recommendedCalculationScheme; }
            set
            {
                _recommendedCalculationScheme = value;
                OnPropertyChanged("RecommendedCalculationScheme");
            }
        }
        [NotMapped]
        public string LabNumber { get; set; } //just to show in the datagrid
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
