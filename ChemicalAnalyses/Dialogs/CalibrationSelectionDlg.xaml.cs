using EntityFrameworkExtras.EF6;
using SA_EF;
using SettingsHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChemicalAnalyses.Dialogs
{
    public partial class CalibrationSelectionDlg : Window
    {
        public ObservableCollection<LinearCalibration> lcList { get; set; }
        public int CalibrationNumber { get; set; }
        string type { get; set; }
        private ChemicalAnalysesEntities context;

        public CalibrationSelectionDlg(string type="Kalium", int number = 0)
        {
            lcList = new ObservableCollection<LinearCalibration>();
            InitializeComponent();
            this.type = type;
            CalibrationNumber = number;
            context = new ChemicalAnalysesEntities();
#if DEBUG
            context.Database.Log = s => { Debug.WriteLine(s); };
#endif
            FillData();
            Title = "Выбор калибровки для: " + type;
            grdMain.DataContext = this;
            //let tooltips be shown on disabled controls as well
            try
            {
                ToolTipService.ShowOnDisabledProperty.OverrideMetadata(
                typeof(Control),
                new FrameworkPropertyMetadata(true));
            }
            catch { }//if it's already overridden
        }

        private void FillData()
        {
            lcList.Clear();
            try
            {
                foreach (LinearCalibration clbr in context.LineaCalibrations
                    .Where(p => p.CalibrationType.Trim() == type))
                    lcList.Add(clbr);
            }
            catch(Exception ex) { }
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
        { e.CanExecute = cbLCSelection?.SelectedItem != null;}

        private void EditCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LinearCalibration lc = context.LineaCalibrations
                .Find(((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID);
            if (lc != null) lc.GetLinearCoefficients();
            else return;
            CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
            if (cldDlg.ShowDialog() == true)
            {
                try
                {
                    context.Database.BeginTransaction(IsolationLevel.Serializable);
                    context.Entry(lc).State = EntityState.Modified;

                    List<LCData> t = new List<LCData>();
                    for (int i = 0; i <= lc.LinearCalibrationData.Rank; i++)
                    {
                        lc.LinearCalibrationData[i].ToList().ForEach(p =>
                        {
                            t.Add(new LCData()
                            {
                                IDCalibration = lc.CalibrationID,
                                IDCalibrationData = p.IDCalibrationData,
                                Diapason = i + 1,
                                Concentration = p.Concentration,
                                Value = p.Value
                            });
                        });
                    }
                    context.Database.ExecuteStoredProcedure(new UpdateLCWithSP() { tmp = t });
                    context.SaveChanges();
                    context.Database.CurrentTransaction.Commit();
                    CALogger.WriteToLogFile(string.Format("Изменена калибровка ID{0};{1} - {2}",
                    lc.CalibrationID, lc.Description, lc.CalibrationType));
                }
                catch (Exception ex)
                {
                    context.Database.CurrentTransaction.Rollback();
                    MessageBox.Show(ex.Message + " в " + ex.Source, "Ошибка");
                }
            }
            FillData();
        }

        private void ViewCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection?.SelectedItem != null;
        }

        private void ViewCommand_Executed (object sender, ExecutedRoutedEventArgs e)
        {
            LinearCalibration lc = context.LineaCalibrations
                .Find(((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID);
            if (lc != null) lc.GetLinearCoefficients();
            else return;
            CalibrationViewDialog cvDlg = new CalibrationViewDialog(ref lc);
            cvDlg.Show(); //just to show it, no results are necessary
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {e.CanExecute = true;}

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LinearCalibration lc = new LinearCalibration() { CalibrationType = type};
            CalibrationDataDialog cldDlg = new CalibrationDataDialog(ref lc);
            if (cldDlg.ShowDialog() == true)
            {
                CALogger.WriteToLogFile(string.Format("Создана калибровка {0} - {1}", 
                    lc.Description , lc.CalibrationType.ToString()));
                    for (int i = 0; i <= lc.LinearCalibrationData.Rank; i++)
                    {
                        lc.LinearCalibrationData[i].ToList().ForEach(p =>
                        {
                            lc.CalibrationData.Add(new DataPoint
                            {
                                Concentration = p.Concentration,
                                Value = p.Value,
                                Diapason = i + 1
                            });
                        });
                    }
                    LinearCalibration newlc = context.LineaCalibrations.Add(lc);
                    context.SaveChanges();
                    FillData();
            }
        }

        private void SetDefaultCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cbLCSelection?.SelectedItem != null;
        }

        private void SetDefaultCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationNumber = ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID;
            CALogger.WriteToLogFile(string.Format("Установлена по умолчанию калибровка для {0} - {1}",
                type, ((LinearCalibration)cbLCSelection.SelectedItem).ToString()));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {DialogResult = true;}

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (cbLCSelection?.SelectedItem != null)
                {
                    e.CanExecute = context.LineaCalibrations
                        .Find((cbLCSelection.SelectedItem as LinearCalibration).CalibrationID)?
                        .SaltAnalysis.Count == 0;
                }
                else e.CanExecute = false;
            }
            catch
            { e.CanExecute = false; }
        
            btnDeleteCalibration.ToolTip = (e.CanExecute) ? "Удалить выбранную калибровку" 
                : "Удаление калибровки невозможно." + Environment.NewLine + "Присутствуют связанные данные.";
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //using (var context = new ChemicalAnalysesEntities())
                //{
#if DEBUG
                    context.Database.Log = s => { Debug.WriteLine(s); };
#endif
                    CALogger.WriteToLogFile(string.Format("Удалена калибровка ID{0};{1} - {2}",
                         ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID,
                         ((LinearCalibration)cbLCSelection.SelectedItem).Description,
                         ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationType.ToString()));
                    context.Database.ExecuteStoredProcedure(new DeleteCalibrationByID()
                        {Calibration_ID = ((LinearCalibration)cbLCSelection.SelectedItem).CalibrationID });
                    context.SaveChanges();
                //}
                FillData();
            }
            catch
            {
                MessageBox.Show("Не удалось удалить калибровку!" + Environment.NewLine + "Имеются связанные данные.");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            context?.Dispose();
            base.OnClosing(e);
        }
    }
}