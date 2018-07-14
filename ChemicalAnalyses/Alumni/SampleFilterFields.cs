using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ChemicalAnalyses.Alumni
{
    public class SampleFilterFields: INotifyPropertyChanged
    {
        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value > DateTime.Today) throw new ArgumentOutOfRangeException("EndDate",
                    "Конечная дата не может лежать в будущем!");
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }
        private DateTime _startDate = DateTime.Today.AddYears(-1);
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value > EndDate) throw new ArgumentOutOfRangeException("StartDate",
                    "Начальная дата не может находиться позже конечной!");
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private string _labNumber;
        public string LabNumber
        {
            get { return _labNumber; }
            set
            {
                _labNumber = value;
                OnPropertyChanged("LabNumber");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return "Начальная дата: " + StartDate.ToShortDateString() +
                    "\nКонечная дата: " + EndDate.ToShortDateString() +
                    (((LabNumber == null ) || string.Empty.Equals(LabNumber.Trim())) ? ""
                    : ("\nЛабораторный номер:" + LabNumber));
        }
    }
}