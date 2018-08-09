using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Configuration;

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
        private ObservableCollection<KeyValuePair<string, decimal?>> _schemeTolerances;
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public ObservableCollection<KeyValuePair<string, decimal?>> SchemeTolerances
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

        public SchemeResultsTolerance(IEnumerable<KeyValuePair<string,decimal?>> collection)
        {
            if (collection != null) SchemeTolerances =
                     new ObservableCollection<KeyValuePair<string, decimal?>>(collection);
            else SchemeTolerances = new ObservableCollection<KeyValuePair<string, decimal?>>();
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
                        SchemeTolerances = new ObservableCollection<KeyValuePair<string, decimal?>>();
                        for (int i = 0; i < vals.Length;)
                        {
                            if (decimal.TryParse(vals[i + 1], out d)) SchemeTolerances.Add(new KeyValuePair<string, decimal?>(vals[i], d));
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
            foreach (KeyValuePair<string, decimal?> kvp in SchemeTolerances)
            {
                stringBuilder.Append(delimiter + kvp.Key + ";" + kvp.Value.ToString());
                delimiter = ";";
            }
            return stringBuilder.ToString();
        }
    }
}