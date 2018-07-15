using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace SA_EF
{
    public partial class LinearCalibration : INotifyPropertyChanged, ILinearCalibration
    {
        private DateTime _date = DateTime.Today;
        public DateTime CalibrationDate
        {
            get { return _date; }
            set
            {
                if (value <= DateTime.Today)
                {
                    _date = value;
                    OnPropertyChanged("CalibrationDate");
                }
                else throw new ArgumentOutOfRangeException("CalibrationDate",
                    "Дата калибровки не может лежать в будущем!");
            }
        }

        private string _calibrationType = "Kalium";
        public string CalibrationType
        {
            get { return _calibrationType; }
            set
            {
                _calibrationType = value;
                OnPropertyChanged("CalibrationType");
            }
        }

        private string _description = "Введите описание";
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == null || value.Trim() == "")
                    throw new ArgumentNullException("Description", "Введите описание!");
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private decimal[] _intercept;
        [NotMapped]
        public decimal[] Intercept
        {
            get { return _intercept; }
            set
            {
                _intercept = value;
                OnPropertyChanged("Intercept");
            }
        }

        private decimal[] _slope;
        [NotMapped]
        public decimal[] Slope
        {
            get { return _slope; }
            set
            {
                _slope = value;
                OnPropertyChanged("Slope");
            }
        }

        private decimal[] _rSquared;
        [NotMapped]
        public decimal[] RSquared
        {
            get { return _rSquared; }
            private set //is calculated and set internally
            {
                _rSquared = value;
                OnPropertyChanged("RSquared");
            }
        }

        private ObservableCollection<DataPoint>[] _linearCalibrationData;
        [NotMapped]
        public ObservableCollection<DataPoint>[] LinearCalibrationData
        {
            get { return _linearCalibrationData; }
            set
            {
                _linearCalibrationData = value;
                OnPropertyChanged("LinearCalibrationData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }

        public override string ToString()
        {
            return CalibrationDate.ToShortDateString().ToString() + " " + Description;
        }

        public void GetLinearCoefficients()
        {
            if (LinearCalibrationData == null || LinearCalibrationData.Any(p => p == null)) return;
            else {
                for (int i =0; i <= LinearCalibrationData.Rank; i++)
                {
                    LinearCalibrationData[i] = 
                        new ObservableCollection<DataPoint>(CalibrationData.Where(p => p.Diapason == i + 1));
                }
            }

            for (int i = 0; i <= LinearCalibrationData.Rank; i++)
            {
                int Count = LinearCalibrationData[i].Count;
                decimal _sumConcentration = 0;
                decimal _sumValues = 0;
                decimal _sumSquares = 0;
                decimal _sumProducts = 0;

                foreach (DataPoint t in LinearCalibrationData[i])
                {
                    _sumConcentration += t.Concentration;
                    _sumValues += t.Value;
                    _sumSquares += t.Concentration * t.Concentration;
                    _sumProducts += t.Concentration * t.Value;
                }
                decimal delta = _sumSquares * Count - _sumConcentration * _sumConcentration;
                if (delta == 0)
                {
                    Slope[i] = decimal.MaxValue;
                    Intercept[i] = decimal.MaxValue;
                    RSquared[i] = 0;
                }
                else
                {
                    Slope[i] = (_sumProducts * Count - _sumConcentration * _sumValues) / delta;
                    Intercept[i] = (_sumValues * _sumSquares - _sumProducts * _sumConcentration) / delta;

                    decimal valueMean = LinearCalibrationData[i].Average(x => x.Value);
                    decimal d = LinearCalibrationData[i].Sum(p => (decimal)Math.Pow((double)(p.Value - valueMean), 2.0));
                    if (d == 0) RSquared[i] = 0;
                    else RSquared[i] = 1 - LinearCalibrationData[i].Sum(p => (decimal)Math.Pow((double)(p.Value -
                        (Slope[i] * p.Concentration + Intercept[i])), 2.0)) / d;
                }
            }
        }
        /// <summary>
        /// Converts Value to concentration basing on calibration points
        /// </summary>
        /// <param name="val">Value to be converted to  concentration</param>
        /// <param name="diap">Number of the diapason</param>
        /// <returns>Concentration</returns>
        public decimal ValueToConcentration(decimal val, int diap)
        {
            if (diap < 0 || diap > 1) throw new ArgumentOutOfRangeException("Diapason", "Недопустимый номер диапазона");
            if (val <= 0) throw new ArgumentOutOfRangeException("Value", "Неверное значение показателя");
            if (val < LinearCalibrationData[diap].Min(p => p.Value) || val > LinearCalibrationData[diap].Max(p => p.Value))
            {//calculate by coefficients
                try { return (val - Intercept[diap]) / Slope[diap]; }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException("Нулевое значение углового коэффициента", ex);
                }
            }
            else
            {//calculate by the way of interpolation between two dots
                try
                {
                    int i = LinearCalibrationData[diap].IndexOf(LinearCalibrationData[diap].Where(p => p.Value > val).First());
                    //LinearCalibrationData[diap].FindIndex(p => p.Value > val);
                    return (val - LinearCalibrationData[diap][i - 1].Value)
                        * (LinearCalibrationData[diap][i].Concentration - LinearCalibrationData[diap][i - 1].Concentration)
                        / (LinearCalibrationData[diap][i].Value - LinearCalibrationData[diap][i - 1].Value)
                        + LinearCalibrationData[diap][i - 1].Concentration;
                }
                catch (Exception ex)
                {
                    throw new ArgumentOutOfRangeException(@"Две последующие точки в калибровочных данных имеют одинаковые значения показателя", ex);
                }
            }
        }
        public bool ContainsEqualDataPoints(int diap)
        {
            if (diap < 0 || diap > 1) return true; // contains
            if (LinearCalibrationData != null && LinearCalibrationData[diap] != null)
            {
                try
                {
                    var d = (LinearCalibrationData[diap].ToList()).ToDictionary(p => p.Concentration, p => p);
                    var t = (LinearCalibrationData[diap].ToList()).ToDictionary(p => p.Value, p => p);
                    return false;
                }
                catch
                {
                    return true;
                }
            }
            else return true;
        }
    }

    public interface ILinearCalibration
    {
        int CalibrationID { get; set; }
        decimal[] Slope { get; set; }
        decimal[] Intercept { get; set; }
        decimal ValueToConcentration(decimal val, int diap);
    }

    /// <summary>
    /// Enumerates the possible chemical elements the calibration can be applied to
    /// </summary>
    public enum ChemicalElemetCalibration { Kalium, Natrium };
}