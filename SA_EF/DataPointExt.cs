using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SA_EF
{
    public partial class DataPoint : INotifyPropertyChanged, IEquatable<DataPoint>
    {
        private decimal conc = 0.001M;
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
                else { val = value; OnPropertyChanged("Value"); }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataPoint);
        }

        public bool Equals(DataPoint dp)
        {
            if (dp == null) return false;
            return Concentration.Equals(dp.Concentration) && Value.Equals(dp.Value) && Diapason.Equals(dp.Diapason);
        }

        public override int GetHashCode()
        {
            return (int)(Value * 26440451) + (int)(Concentration * 334216273) + (int)(Diapason*123);
        }
        public static bool operator ==(DataPoint dp1, DataPoint dp2)
        {
            if (ReferenceEquals(dp1, dp2)) return true;
            if (ReferenceEquals(dp1, null) || ReferenceEquals(dp2, null)) return false;
            return dp1.Equals(dp2);
        }

        public static bool operator !=(DataPoint dp1, DataPoint dp2)
        {
            return !(dp1 == dp2);
        }
    }
}