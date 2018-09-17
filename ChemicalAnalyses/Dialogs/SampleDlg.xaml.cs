using System.Windows;
using SA_EF;

namespace ChemicalAnalyses.Dialogs
{
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
            smpl.LabNumber = smpl.LabNumber.Trim();
            this.DialogResult = true;
        }
    }
}