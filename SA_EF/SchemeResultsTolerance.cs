using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SA_EF
{
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
                    _universalTolerance = value;
                    OnPropertyChanged(nameof(UniversalTolerance));
                }
            }
        }
        private ObservableCollection<KeyValuePair<string, decimal?>> _schemeTolerances;
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

        public SchemeResultsTolerance() : this(null) { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop)); }
    }
}