using System;
using System.Windows;
using System.Windows.Controls;
using Calibration;
using System.Windows.Input;

namespace ChemicalAnalyses.Dialogs
{
    public partial class CalibrationDataDialog : Window
    {
        private int errorCount = 0;
        private bool fdNoEqualDP = true;
        private bool sdNoEqualDP = true;
        public LinearCalibration lc { get; set; }
        public CalibrationDataDialog (ref LinearCalibration calibration)
         {
            InitializeComponent();
            lc = calibration;
            grdCalibrationDialog.DataContext = this;
            Array values = Enum.GetValues(typeof(ChemicalElemetCalibration));
            foreach (int value in values)
            {
                string display = Enum.GetName(typeof(ChemicalElemetCalibration), value);
                cbChemicalElemets.Items.Add(display);
                cbChemicalElemets.SelectedIndex = Array.IndexOf(values, calibration.CalibrationType, 0);
                cbChemicalElemets.IsEnabled = false;
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = errorCount == 0 && (dgrdFirstDiapason.Items.Count > 2)
                && (dgrdSecondDiapason.Items.Count > 2) && fdNoEqualDP && sdNoEqualDP;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnErrorEvent(object sender, RoutedEventArgs e)
        {
           var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {
                        errorCount++; break;
                    }
                case ValidationErrorEventAction.Removed:
                    {
                        errorCount--; break;
                    }
                default:
                    {
                        throw new Exception("Unknown action");
                    }
            }
        }

        private void DataGrids_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((sender as DataGrid).Name == "dgrdFirstDiapason")
                fdNoEqualDP = !lc.ContainsEqualDataPoints(0);
            else sdNoEqualDP = !lc.ContainsEqualDataPoints(1);
        }
    }
}