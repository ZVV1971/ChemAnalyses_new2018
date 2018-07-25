using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SA_EF
{
    public partial class Sample : INotifyPropertyChanged
    {
        private string _labnumber;
        public string LabNumber
        {
            get { return _labnumber; }
            set
            {
                if (value == null || value.Equals(String.Empty))
                    throw new ArgumentNullException("LabNumber", "Номер не может быть пустой строкой," +
                        "\nтак как используется для идентификации");
                if (value?.Length > 15 || value?.Length < 2)
                    throw new ArgumentOutOfRangeException("LabNumber", "Неверный формат номера!");
                if (value.Contains(";"))
                    throw new ArgumentOutOfRangeException("LabNumber", "Номер не может содержать символ ';'!");
                _labnumber = value;
                OnPropertyChanged("LabNumber");
            }
        }

        private DateTime _samplingdate = DateTime.Today;
        public DateTime SamplingDate
        {
            get { return _samplingdate; }
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentOutOfRangeException("SamplingDate", "Дата отбора не может лежать в будущем!");
                _samplingdate = value;
                OnPropertyChanged("SamplingDate");
            }
        }

        private string _desc;
        public string Description
        {
            get { return _desc; }
            set
            {
                if (value == null || value.Equals(string.Empty))
                    throw new ArgumentNullException("Description", "Описание не может быть пустым");
                if (value?.Length > 200) throw new ArgumentOutOfRangeException("Description", "Слишком длинная строка!");
                _desc = value;
                OnPropertyChanged("Description");
            }
        }

        public override string ToString()
        {
            return String.Format("Лабораторный номер: {0}, Дата отбора: {1:dd-MM-yyy}\n Описание: {2}",
                LabNumber, SamplingDate, Description);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}