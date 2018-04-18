using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Calibration;
using SettingsHelper;

namespace ChemicalAnalyses.Dialogs
{
    public partial class CalibrationSelectionDlg : Window
    {
        public ObservableCollection<LinearCalibration> lcList { get; set; }
        public int CalibrationNumber { get; set; }
        string type { get; set; }

        public CalibrationSelectionDlg(string type="Kalium", int number = 0)
        {
            lcList = new ObservableCollection<LinearCalibration>();
            InitializeComponent();
            this.type = type;
            CalibrationNumber = number;
            FillData();
            Title = "Выбор калибровки для: " + type;
            grdMain.DataContext = this;
        }

        private void FillData()
        {
            lcList.Clear();
            foreach (LinearCalibration clbr in LinearCalibration.GetAllLC("[CalibrationType] = N'" + type + "'"))
            {
                lcList.Add(clbr);
            }
            try
            {
                cbLCSelection.SelectedIndex = lcList.IndexOf(lcList.First(p => p.CalibrationID == CalibrationNumber));
            }
            catch
            {
                cbLCSelection.SelectedIndex = 0;
            }
        }

        private void EditCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection.SelectedItem != null;
        }

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //deep read the currently selected calibration
            LinearCalibration lc = LinearCalibration.GetAllLC("[IDCalibration] = " +
            ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID, true)?.Single();
            CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
            if (cldDlg.ShowDialog() == true)
            {
                try
                {
                    lc.Update();
                    CALogger.WriteToLogFile(string.Format("Изменена калибровка ID{0};{1} - {2}",
                    lc.CalibrationID, lc.Description, lc.CalibrationType.ToString()));
                    FillData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " в " + ex.Source, "Ошибка");
                }
            }
        }

        private void ViewCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection.SelectedItem != null;
        }

        private void ViewCommand_Executed (object sender, ExecutedRoutedEventArgs e)
        {
            //deep read the currently selected calibration
            LinearCalibration lc = LinearCalibration.GetAllLC("[IDCalibration] = " +
            ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID, true)?.Single();
            CalibrationViewDialog cvDlg = new CalibrationViewDialog(ref lc);
            cvDlg.Show(); //just to show it, no results are necessary
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LinearCalibration lc = new LinearCalibration() { CalibrationType = 
                (ChemicalElemetCalibration)Enum.Parse(typeof(ChemicalElemetCalibration), type) };
            CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
            if (cldDlg.ShowDialog() == true)
            {
                CALogger.WriteToLogFile(string.Format("Создана калибровка {0} - {1}", 
                    lc.Description , lc.CalibrationType.ToString()));
                lc.Insert();
                FillData();
            }
        }

        private void SetDefaultCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection.SelectedItem != null;
        }

        private void SetDefaultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationNumber = ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection.SelectedItem != null;
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LinearCalibration tmpLC = ((LinearCalibration)cbLCSelection.SelectedItem);
                LinearCalibration.Delete(tmpLC.CalibrationID);
                CALogger.WriteToLogFile(string.Format("Удалена калибровка ID{0};{1} - {2}",
                     tmpLC.CalibrationID, tmpLC.Description, tmpLC.CalibrationType.ToString()));
                FillData();
            }
            catch
            {
                MessageBox.Show("Не удалось удалить калибровку!\nИмеются связанные данные.");
            }
        }
    }
}