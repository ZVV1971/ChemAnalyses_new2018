using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SA_EF;
using ChemicalAnalyses.Alumni;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChemicalAnalyses.Dialogs
{
    public partial class CalibrationDataDialog : Window
    {
        private int errorCount = 0;
        private bool fdNoEqualDP = true;
        private bool sdNoEqualDP = true;
        public LinearCalibration lc { get; set; }

        public static ObservableCollection<KeyValuePair<string, string>> elems { get; set; }

        public CalibrationDataDialog (ref LinearCalibration calibration)
         {
            InitializeComponent();
            lc = calibration;
            lc.CalibrationType = lc.CalibrationType.Trim();
            grdCalibrationDialog.DataContext = this;
            if (elems == null) elems = new ObservableCollection<KeyValuePair<string, string>>
                 (Enum.GetValues(typeof(ChemicalElemetCalibration)).OfType<ChemicalElemetCalibration>()
                .Select(p => new KeyValuePair<string, string>(p.ToString(), p.ToName())));
            // if new is being created don't disable type selector
            if (lc.CalibrationData.Count != 0) cbChemicalElemets.IsEnabled = false;
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