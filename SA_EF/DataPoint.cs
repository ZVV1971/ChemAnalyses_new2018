using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SA_EF
{    
    public partial class DataPoint : INotifyPropertyChanged
    {
        private decimal conc = (decimal)0.001;
        public decimal Concentration
        {
            get { return conc; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Concentration",
                    "Концентрация должна быть положительным числом!");
                else { conc = value; OnPropertyChanged("Concentration"); }
            }
        }
        private decimal val = 1;
        public decimal Value
        {
            get { return val; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Value",
                    "Показание прибора должно быть положительным числом!");
                else { val = value; OnPropertyChanged("value"); }
            }
        }

        public int IDCalibration { get; set; }
        public int IDCalibrationData { get; set; }
        public int Diapason { get; set; }
        //public decimal Concentration { get; set; }
        //public decimal Value { get; set; }
    
        public virtual LinearCalibration Calibrations { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
