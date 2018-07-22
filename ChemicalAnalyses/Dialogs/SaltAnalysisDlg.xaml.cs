using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using ChemicalAnalyses.Alumni;
using System.Windows.Data;
using SettingsHelper;
using SA_EF;
using System.Diagnostics;
using System.Data.Entity;
using SA_EF.Interfaces;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SaltAnalysisDlg : Window
    {
        private int _validationErrorCount = 0;
        private int _validationRowErrorCount = 0;
        public ObservableCollection<SaltAnalysisData> sa { get; set; }
        public string TypeOfWork { get; set; }
        public List<Sample> Labnumbers { get; set; }
        private bool _all_selected = false;

        public static ObservableCollection<KeyValuePair<SaltCalculationSchemes, string>> SchemesNames { get; set; }
        //public static ObservableCollection<string> SchemesNames { get; private set; }

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

            if (SchemesNames == null) SchemesNames = new ObservableCollection<KeyValuePair< SaltCalculationSchemes, string>>
                (Enum.GetValues(typeof(SaltCalculationSchemes)).OfType<SaltCalculationSchemes>()
                .Select(p => new KeyValuePair<SaltCalculationSchemes, string>( p, p.ToName())));
               
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
            using (var context = new ChemicalAnalysesEntities())
            {
#if DEBUG
                context.Database.Log = s => { Debug.WriteLine(s); };
#endif
                if (Labnumbers?.Count == 1)
                {
                    try
                    {
                        var smpl = context.Samples
                            .Find(Labnumbers[0].IDSample).SaltAnalysisDatas;
                        foreach (SaltAnalysisData sad in smpl)
                        {
                            sad.LabNumber = Labnumbers[0].LabNumber;
                            sa.Add(sad);
                        }
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    var sampl_list = (from Labnumber in Labnumbers
                                      join smpl in context.SaltAnalysisDatas on Labnumber.IDSample equals smpl.IDSample
                                      select smpl);
                    foreach (SaltAnalysisData sad in sampl_list)
                    {
                        sad.LabNumber = context.Samples.Where(p=>p.IDSample == sad.IDSample)
                            .FirstOrDefault().LabNumber;
                        sa.Add(sad);
                    }
                }
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
                    using (var context = new ChemicalAnalysesEntities())
                    {
                        dgrdSA.Items.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                        {
                            context.Entry(p).State = EntityState.Added;
                            s.AppendLine(p.LabNumber);
                        });
                        context.SaveChanges();
                    }
                    CALogger.WriteToLogFile("Созданы новые анализы для следующих образцов: " + s);
                }
                else if (TypeOfWork == "Edit")
                {
                    using (var context = new ChemicalAnalysesEntities())
                    {
                        dgrdSA.Items.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                        {
                            context.Entry(p).State = EntityState.Modified;
                            s.AppendLine(p.LabNumber);
                        });
                        context.SaveChanges();
                    }
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
                using (var context = new ChemicalAnalysesEntities())
                {
                    dgrdSA.SelectedItems.Cast<SaltAnalysisData>().ToList().ForEach(p =>
                        {
                            s.AppendLine(p.LabNumber);
                            context.Entry(p).State=EntityState.Deleted;
                        });
                    context.SaveChanges();
                }
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
            e.CanExecute = dgrdSA.SelectedItems.Count != 0 && _validationErrorCount == 0;
        }

        private void CalculateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmWhenSchemesDiffer()) return;

            dgrdSA.SelectedItems.Cast<SaltAnalysisData>().ToList().ForEach(p => {
                p.CalcDryValues();
                p.KDry = p.CalcKaliumValue();
                //calculate using the user-selected scheme
                p.CalcSchemeResults(p, p.DefaultCalculationScheme);
                //set recommended scheme to the calculated one
                p.RecommendedCalculationScheme = p.CalcRecommendedScheme(p);
            });
            MessageBox.Show(dgrdSA.SelectedItems.Count.ToString() + " образцов были расчитаны");
            btnPrint.Visibility = Visibility.Visible;
        }

        private void PrintCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!ConfirmWhenSchemesDiffer()) return;
            SAPrintPreview sAPrintPreview = new SAPrintPreview
            {
                Owner = this,
                Title = @"Предварительный просмотр результатов расчета"
            };

            List<SchemesPrintingGrid> pGrids = new List<SchemesPrintingGrid>();
            IEnumerable<ISaltAnalysisCalcResults> res = dgrdSA.SelectedItems.Cast<ISaltAnalysisCalcResults>();
            foreach (SaltCalculationSchemes schem in Enum.GetValues(typeof(SaltCalculationSchemes))
                .Cast<SaltCalculationSchemes>())
            {
                var tmp = res.Where(p => p.DefaultCalculationScheme == schem);
                if (tmp.Count() > 0) pGrids.Add(new SchemesPrintingGrid(tmp) {
                    Name = "pg" + schem.ToString(),
                    ResultsType = schem
                });
            }
           
            foreach (SchemesPrintingGrid pgrd in pGrids)
            {
                TextBox tbTitle = new TextBox()
                {
                    Text = string.Format( "Химический состав, % (схема: {0})", pgrd.ResultsType.ToName()),
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Width = 1000,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true
                };
                sAPrintPreview.fdSA.Pages.Add(
                    new PageContent()
                    {
                        Name = "pk1_" + pgrd.Name,
                        Child = new FixedPage()
                        {
                            Name = "fp1_"+pgrd.Name,
                            Width = 1056,
                            Height = 890,
                            Children = { new Canvas() { Children = { pgrd, tbTitle } } }
                        }
                    });
                Canvas.SetLeft(pgrd, 10);
                Canvas.SetTop(pgrd, 50);
                Canvas.SetTop(tbTitle, 10);
                Canvas.SetLeft(tbTitle, 10);
            }

            StackPanel spHygro = new StackPanel() { Orientation = Orientation.Horizontal };
            sAPrintPreview.grdOptions.Children.Add(spHygro);

            Grid.SetColumn(spHygro, 0);
            Grid.SetColumnSpan(spHygro, 2);
            Label lbHygro = new Label() { Content = "Выводить гигроскопическую влагу для всех образцов? " };
            spHygro.Children.Add(lbHygro);
            CheckBox cbHygro = new CheckBox();
            Binding bdHygro = new Binding("ShowHygroscopicWaterForAll") {
                Source = pGrids.Where(p=>p.ResultsType == SaltCalculationSchemes.Chloride).FirstOrDefault()};
            cbHygro.SetBinding(CheckBox.IsCheckedProperty, bdHygro);
            spHygro.Children.Add(cbHygro);

            sAPrintPreview.ShowDialog();
        }

        private void dgrdSA_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(OnRowErrorEvent));
        }

        private void PrintCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dgrdSA.SelectedItems.Count != 0 && _validationErrorCount == 0;
        }

        private void SelectAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = dgrdSA.Items.Count != 0;
        }

        private void SelectAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_all_selected) dgrdSA.SelectAll();
            else dgrdSA.UnselectAll();
            _all_selected = !_all_selected;
        }

        private bool ConfirmWhenSchemesDiffer()
        {
            if (dgrdSA.SelectedItems.Cast<SaltAnalysisData>()
               .Any(p => p.DefaultCalculationScheme != p.RecommendedCalculationScheme))
            {
                if (MessageBox.Show("В одном из выбранных для расчета образцов рекомендуемая схема расчета" +
                    " не совпадает с выбранной.\nПродолжить?", "Внимание", MessageBoxButton.YesNo,
                    MessageBoxImage.Hand, MessageBoxResult.No) == MessageBoxResult.No) return false;
            }
            return true;
        }
    }
}