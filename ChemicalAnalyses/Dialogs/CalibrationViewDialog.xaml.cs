using System.Windows;
//using Calibration;
using SA_EF;

namespace ChemicalAnalyses.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для CalibrationViewDialog.xaml
    /// </summary>
    public partial class CalibrationViewDialog : Window
    {
        public bool HasChanged = false;

        public bool HasSaved = true;

        public LinearCalibration lcCalibration
        {
            get { return (LinearCalibration)GetValue(lcCalibrationProperty); }
            set { SetValue(lcCalibrationProperty, value); }
        }

        public static readonly DependencyProperty lcCalibrationProperty =
           DependencyProperty.Register("lcCalibration",
                typeof(LinearCalibration), typeof(CalibrationViewDialog),
                new PropertyMetadata());

        public CalibrationViewDialog(ref LinearCalibration lc)
        {
            InitializeComponent();
            lcCalibration = lc;
            grdMain.DataContext = this;
        }
    }
}