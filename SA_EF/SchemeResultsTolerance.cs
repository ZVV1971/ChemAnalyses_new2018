using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SA_EF
{
    [Serializable]
    public class SchemeResultsTolerance : INotifyPropertyChanged
    {
        private bool _isUniversalTolerance = false;
        public bool IsUniversalTolerance
        {
            get { return _isUniversalTolerance; }
            set
            {
                _isUniversalTolerance = value;
                OnPropertyChanged(nameof(IsUniversalTolerance));
            }
        }
        private decimal? _universalTolerance = 0.005M;
        public decimal? UniversalTolerance
        {
            get { return _universalTolerance; }
            set
            {
                if (value != null && value.HasValue)
                {
                    if (value > 0.1M || value <= 0) throw new ArgumentOutOfRangeException(nameof(UniversalTolerance),
                          "Значение толеранса должно быть больше 0 и меньше 10%");
                    _universalTolerance = value;
                    OnPropertyChanged(nameof(UniversalTolerance));
                }
            }
        }
        private ObservableCollection<ParameterValuePair> _schemeTolerances;

        public ObservableCollection<ParameterValuePair> SchemeTolerances
        {
            get { return _schemeTolerances; }
            set
            {
                if (value != null)
                {
                    _schemeTolerances = value;
                    OnPropertyChanged(nameof(SchemeTolerances));
                }
            }
        }

        public SchemeResultsTolerance(IEnumerable<ParameterValuePair> collection)
        {
            if (collection != null) SchemeTolerances =
                     new ObservableCollection<ParameterValuePair>(collection);
            else SchemeTolerances = new ObservableCollection<ParameterValuePair>();
        }

        public SchemeResultsTolerance() { }

        public SchemeResultsTolerance(string serializedString)
        {
            if (serializedString != null && !serializedString.Equals(string.Empty))
            {
                string[] strs = serializedString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (strs.Length == 3)
                {
                    bool isU;
                    if (bool.TryParse(strs[0], out isU)) IsUniversalTolerance = isU;
                    decimal unT;
                    if (decimal.TryParse(strs[1], out unT)) UniversalTolerance = unT;
                    string[] vals = strs[2].Split(';');
                    if (vals.Length % 2 == 0)
                    {
                        decimal d;
                        SchemeTolerances = new ObservableCollection<ParameterValuePair>();
                        for (int i = 0; i < vals.Length;)
                        {
                            if (decimal.TryParse(vals[i + 1], out d)) SchemeTolerances.Add(new ParameterValuePair() { Item1=vals[i], Item2=d });
                            i += 2;
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string delimiter = string.Empty;
            stringBuilder.Append(IsUniversalTolerance + Environment.NewLine + UniversalTolerance + Environment.NewLine);
            foreach (ParameterValuePair kvp in SchemeTolerances)
            {
                stringBuilder.Append(delimiter + kvp.Item1 + ";" + kvp.Item2.ToString());
                delimiter = ";";
            }
            return stringBuilder.ToString();
        }
    }

    public class ParameterValuePair: INotifyPropertyChanged
    {
        private string _item1;
        public string Item1
        {
            get { return _item1;}
            set
            {
                _item1 = value;
                OnPropertyChanged(nameof(Item1));
            }
        }
        private decimal? _item2 = 0;
        public decimal? Item2
        {
            get { return _item2; }
            set
            {
                if (value.HasValue && (value < 0.001M || value > 0.1M))
                    throw new ArgumentOutOfRangeException(nameof(Item2),"Значение должно находиться в пределах 0.1%—10%");
                _item2 = value;
                OnPropertyChanged(nameof(Item2));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }
    }
}