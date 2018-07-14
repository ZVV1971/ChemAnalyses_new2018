using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SettingsHelper;
using SA_EF;

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
            using (var context = new ChemicalAnalysesEntities())
            {
                foreach (LinearCalibration clbr in 
                context.LineaCalibrations.Where(p=>p.CalibrationType== type))
                //LinearCalibration.GetAllLC("[CalibrationType] = N'" + type + "'")
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
            using (var context = new ChemicalAnalysesEntities())
            {
                LinearCalibration lc = context.LineaCalibrations
                    .Find(((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID);
                //LinearCalibration.GetAllLC("[IDCalibration] = " +
                //((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID, true)?.Single();
                if (lc != null) lc.GetLinearCoefficients();
                else return;
                CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
                if (cldDlg.ShowDialog() == true)
                {
                    try
                    {
                        context.SaveChanges();
                        //lc.Update();
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
        }

        private void ViewCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection.SelectedItem != null;
        }

        private void ViewCommand_Executed (object sender, ExecutedRoutedEventArgs e)
        {
            //deep read the currently selected calibration
            using (var context = new ChemicalAnalysesEntities())
            {
                LinearCalibration lc = context.LineaCalibrations
                    .Find(((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID);
                if (lc != null) lc.GetLinearCoefficients();
                else return;
                //LinearCalibration.GetAllLC("[IDCalibration] = " +
                //((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID, true)?.Single();
                CalibrationViewDialog cvDlg = new CalibrationViewDialog(ref lc);
                cvDlg.Show(); //just to show it, no results are necessary
            }
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LinearCalibration lc = new LinearCalibration() { CalibrationType = 
                //(ChemicalElemetCalibration)Enum.Parse(typeof(ChemicalElemetCalibration), 
                type};
            CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
            if (cldDlg.ShowDialog() == true)
            {
                CALogger.WriteToLogFile(string.Format("Создана калибровка {0} - {1}", 
                    lc.Description , lc.CalibrationType.ToString()));
                using (var context = new ChemicalAnalysesEntities())
                {
                    context.LineaCalibrations.Add(lc);
                    context.SaveChanges();
                }
                    //lc.Insert();
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
            CALogger.WriteToLogFile(string.Format("Установлена по умолчанию калибровка для {0} - {1}",
                type, ((LinearCalibration)cbLCSelection.SelectedItem).ToString()));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            using (var context = new ChemicalAnalysesEntities())
            {
                if (cbLCSelection.SelectedItem != null)
                {
                    e.CanExecute = context.LineaCalibrations
                        .Find((cbLCSelection.SelectedItem as LinearCalibration).CalibrationID)?
                        .SaltAnalysis.Count == 0;
                }
                else e.CanExecute = false;
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                using (var context = new ChemicalAnalysesEntities())
                {
                    LinearCalibration tmpLC = context.LineaCalibrations
                    .Find(((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID);
                    //((LinearCalibration)cbLCSelection.SelectedItem);
                    //LinearCalibration.Delete(tmpLC.CalibrationID);
                    CALogger.WriteToLogFile(string.Format("Удалена калибровка ID{0};{1} - {2}",
                         tmpLC.CalibrationID, tmpLC.Description, tmpLC.CalibrationType.ToString()));
                    context.LineaCalibrations.Remove(tmpLC);
                    context.SaveChanges();
                }
                FillData();
            }
            catch
            {
                MessageBox.Show("Не удалось удалить калибровку!\nИмеются связанные данные.");
            }
        }
    }
}