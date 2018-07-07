using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SaltAnalysisDatas;
using Samples;
using System.Windows.Documents;
using ChemicalAnalyses.Alumni;
using System.Windows.Data;
using SettingsHelper;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SaltAnalysisDlg : Window
    {
        private int _validationErrorCount = 0;
        private int _validationRowErrorCount = 0;
        public ObservableCollection<SaltAnalysisData> sa { get; set; }
        public string TypeOfWork { get; set; }
        public List<Sample> Labnumbers { get; set; }

        public SaltAnalysisDlg(List<Sample> lst, string typeOfWork = "Create")
        {
            TypeOfWork = typeOfWork;
            decimal HgCoeff = Properties.Settings.Default.HgCoefficient;
            decimal BrBlank = Properties.Settings.Default.BrBlank;
            decimal BrTitre = Properties.Settings.Default.BrTitre;
            decimal SulfatesBlank = Properties.Settings.Default.SulfatesBlank;
            decimal CaTrilonB = Properties.Settings.Default.CaTrilonB;
            decimal MgTrilonB = Properties.Settings.Default.MgTrilonB;
            int KaliumCalibrationNumber = Properties.Settings.Default.KaliumCalibrationNumber;

            Labnumbers = lst;

            sa = new ObservableCollection<SaltAnalysisData>();
            InitializeComponent();
            if (TypeOfWork == "Create")
            {//Creating new analyses 'data with on-default settings
                Labnumbers.ForEach(p => sa.Add(new SaltAnalysisData
                {
                    IDSample = p.IDSample,
                    LabNumber = p.LabNumber,
                    HgCoefficient = HgCoeff,
                    BromumBlank = BrBlank,
                    BromumStandardTitre = BrTitre,
                    CalciumTrilonTitre = CaTrilonB,
                    MagnesiumTrilonTitre = MgTrilonB,
                    KaliumCalibration = KaliumCalibrationNumber
                }));
                //Change OK button tooltip
                btnOK.ToolTip = "Сохранить новые введенные данные для анализов";
                btnDelete.Visibility = Visibility.Collapsed;
                btnPrint.Visibility = Visibility.Collapsed;
            }
            else
            {
                FillData();
                //Change OK button tooltip
                btnOK.ToolTip = "Сохранить измененные данные для анализов";
                btnDelete.Visibility = Visibility.Visible;
                btnPrint.Visibility = Visibility.Collapsed;
            }
            grdMain.DataContext = this;
        }

        private void FillData()
        {
            sa.Clear();
            string cond = "";
            //if the list contans just one sample make simple condition
            if (Labnumbers?.Count == 1) cond = "[IDSample] = " + Labnumbers[0].IDSample;
            else // if many - use IN statement
            {
                bool fl = false;
                cond = "[IDSample] IN (";
                foreach (Sample smpl in Labnumbers)
                {
                    cond = cond + ((fl) ? ", " : " ") + smpl.IDSample;
                    fl = true;
                }
                cond += " )";
            }
            foreach (SaltAnalysisData sad in SaltAnalysisData.GetAllSamples(cond))
            {
                sa.Add(sad);
            }
        }

        private void OnErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {_validationErrorCount++; break;}
                case ValidationErrorEventAction.Removed:
                    {_validationErrorCount--; break;}
                default:
                    throw new Exception("Unknown action");
            }
        }

        private void OnRowErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null) throw new Exception("Unexpected event args");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {_validationRowErrorCount++; break;}
                case ValidationErrorEventAction.Removed:
                    {_validationRowErrorCount--; break;}
                default:
                    throw new Exception("Unknown action");
            }
        }

        private void SaveCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_validationErrorCount == 0) && (_validationRowErrorCount == 0) && (dgrdSA.Items.Count >= 1);
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder s = new StringBuilder();
            try
            {
                if (TypeOfWork == "Create")
                {
                    dgrdSA.Items.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                    {
                        p.Insert();
                        s.AppendLine(p.LabNumber);
                    });
                    CALogger.WriteToLogFile("Созданы новые анализы для следующих образцов: " + s);
                }
                else if (TypeOfWork == "Edit")
                {
                    dgrdSA.Items.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                    {
                        p.Update();
                        s.AppendLine(p.LabNumber);
                    });
                    CALogger.WriteToLogFile("Изменены данные анализов для следующих образцов: " + s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " в " + ex.Source, "Ошибка");
            }
            DialogResult = true;
        }

        private void DeleteCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dgrdSA.SelectedItems.Count != 0;
        }

        private void DeleteCommand_Executed (object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Удаленные данные будет невозможно восстановить\n Продолжить?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            StringBuilder s = new StringBuilder();
            try
            {
                dgrdSA.SelectedItems.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                    {
                        s.AppendLine(p.LabNumber);
                        SaltAnalysisData.Delete(p.IDSaltAnalysis);
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " в " + ex.Source, "Ошибка");
            }
            finally
            {
                CALogger.WriteToLogFile("Удалены данные анализов для следующих образцов: " + s);
                FillData();
            }
        }

        private void CalibrationButton_Click (object sender, RoutedEventArgs e)
        {
            if (dgrdSA.SelectedItems.Count > 1)
                {
                MessageBox.Show("Выбрано несколько образцов"+Environment.NewLine
                    +"Выберите только один!","Ошибка", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
                }
            
            CalibrationSelectionDlg dlg = null;
            dlg = new CalibrationSelectionDlg("Kalium", ((SaltAnalysisData)dgrdSA.SelectedItem).KaliumCalibration);
            dlg.btnSetDefault.Content = "Установить для №" + ((SaltAnalysisData)dgrdSA.SelectedItem).LabNumber;
            dlg.btnSetDefault.ToolTip = "Установить выбранную калибровку только для анализа образца с лабораторным № " +
                ((SaltAnalysisData)dgrdSA.SelectedItem).LabNumber;
            if (dlg.ShowDialog() == true)
            {
                ((SaltAnalysisData)dgrdSA.SelectedItem).KaliumCalibration = dlg.CalibrationNumber;
            }
        }

        private void CalculateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dgrdSA.SelectedItems.Count != 0;
        }

        private void CalculateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dgrdSA.SelectedItems.Cast<SaltAnalysisData>().ToList().ForEach(p => {
                p.CalcValues();
                p.CalcKaliumValue();
                p.CalcSchemeResults();
                p.CalcRecommendedScheme();
            });
            MessageBox.Show(dgrdSA.SelectedItems.Count.ToString() + " were filled with calculation results");
            btnPrint.Visibility = Visibility.Visible;
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SAPrintPreview sAPrintPreview = new SAPrintPreview
            {
                Owner = this,
                Title = @"Предварительный просмотр результатов расчета, схема ""Хлоридная"" (IV)" };
            ChlorideSchemePrintingGrid pgrdChloride = 
                new ChlorideSchemePrintingGrid(dgrdSA.SelectedItems.Cast<SaltAnalysisData>().ToList()) { Name = "pg1" };
            TextBox tbTitle = new TextBox()
            {
                Text = "Химический состав, %",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Width = 1000,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                BorderThickness = new Thickness(0),
                IsReadOnly = true
            };
            sAPrintPreview.fdSAChlorideScheme.Pages.Add(
                new PageContent()
                {
                    Name = "pk1",
                    Child = new FixedPage()
                    {
                        Name = "fp1", Width=1056, Height=890,
                        Children = { new Canvas() { Children = { pgrdChloride, tbTitle } } }
                    }
                });
            
            Canvas.SetLeft(pgrdChloride,10);
            Canvas.SetTop(pgrdChloride, 50);
            Canvas.SetTop(tbTitle, 10);
            Canvas.SetLeft(tbTitle, 10);
            StackPanel spHygro = new StackPanel() { Orientation = Orientation.Horizontal };
            sAPrintPreview.grdOptions.Children.Add(spHygro);

            Grid.SetColumn(spHygro, 0);
            Grid.SetColumnSpan(spHygro, 2);
            Label lbHygro = new Label() { Content = "Выводить гигроскопическую влагу для всех образцов? " };
            spHygro.Children.Add(lbHygro);
            CheckBox cbHygro = new CheckBox();
            Binding bdHygro = new Binding("ShowHygroscopicWaterFroAll") { Source = pgrdChloride};
            cbHygro.SetBinding(CheckBox.IsCheckedProperty, bdHygro);
            spHygro.Children.Add(cbHygro);

            sAPrintPreview.ShowDialog();
        }

        private void dgrdSA_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(OnRowErrorEvent));
        }
    }
}