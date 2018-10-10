using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SA_EF;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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