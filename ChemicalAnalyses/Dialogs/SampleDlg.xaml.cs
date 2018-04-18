using System.Windows;
using Samples;

namespace ChemicalAnalyses.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для Sample.xaml
    /// </summary>
    public partial class SampleDlg : Window
    {
        public Sample smpl { get; set; }

        public SampleDlg(ref Sample smpl)
        {
            this.smpl = smpl;
            InitializeComponent();
            grdSample.DataContext = this.smpl;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}