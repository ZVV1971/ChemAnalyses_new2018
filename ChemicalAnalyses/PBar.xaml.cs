using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ChemicalAnalyses
{
    public partial class PBar : Window, INotifyPropertyChanged
    {
        private Window wndThis = null;

        private string _messageText = "Подключение…";

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                OnPropertyChanged("MessageText");
            }
        }
 
        public PBar()
        {
            InitializeComponent();
            DataContext = this;
            wndThis = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}