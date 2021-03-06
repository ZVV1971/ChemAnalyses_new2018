﻿using ChemicalAnalyses.Alumni;
using SA_EF;
using SettingsHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TAlex.WPF.Controls;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SamplesViewDlg : Window
    {
        ObservableCollection<Sample> SamplesCollection;
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(SamplesViewDlg),
                new PropertyMetadata(null));

        public string WindowTitle
        {
            get { return (string)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }

        public static readonly DependencyProperty WindowTitleProperty =
            DependencyProperty.Register("WindowTitle", typeof(string), typeof(SamplesViewDlg));

        private static SampleFilterFields fFields;
        public SampleFilterFields GetFilter() { return fFields; }

        public decimal NumberOfAnalysesToAdd
        {
            get { return (decimal)GetValue(NumberOfAnalysesToAddProperty); }
            set { SetValue(NumberOfAnalysesToAddProperty, value); }
        }

        public static readonly DependencyProperty NumberOfAnalysesToAddProperty =
            DependencyProperty.Register(nameof(NumberOfAnalysesToAdd), 
                typeof(decimal), typeof(SamplesViewDlg), new PropertyMetadata(1M));

        public SamplesViewDlg()
        {
            SamplesCollection = new ObservableCollection<Sample>();
            InitializeComponent();
            lbSamples.ItemsSource = SamplesCollection;
            grdMain.DataContext = this;
            DataContext = this;
            try {fFields = Properties.Settings.Default.PreviousFilter;}
            catch { }
            if (fFields == null) fFields = new SampleFilterFields();
            FilterText = GetFilter().ToString();
        }

        void FillData()
        {
            SamplesCollection.Clear();
            try
            {
                using (var context = new ChemicalAnalysesEntities())
                {
                    if (context != null)
                    {
#if DEBUG
                        context.Database.Log = (s) => { Debug.WriteLine(s); };
#endif
                        string[] lnArray = null;
                        if (fFields.LabNumber != null && !fFields.LabNumber.Equals(string.Empty))
                        {
                            if (fFields.LabNumber.Contains(';'))
                            {//a list of samples semicolon-separated
                                lnArray = Regex.Split(fFields.LabNumber, ";");
                                // JOIN Samples with a list of filtering labnumbers on Sample.Labnumber
                                // and then apply WHERE clause 
                                context.Samples
                                    .Join(inner: lnArray.ToList(),
                                    outerKeySelector: e => e.LabNumber,
                                    innerKeySelector: o => o.Trim(),
                                    resultSelector: (e, o) => e)
                                    .Where(p => p.SamplingDate <= fFields.EndDate && p.SamplingDate >= fFields.StartDate)
                                 .ToList().ForEach(d => SamplesCollection.Add(d));
                            }
                            else context.Samples.Where(p => p.LabNumber.Equals(fFields.LabNumber))
                                    .Where(p => p.SamplingDate <= fFields.EndDate && p.SamplingDate >= fFields.StartDate)
                                    .ToList().ForEach(d => SamplesCollection.Add(d));
                        }
                        else context.Samples.Where(p => p.SamplingDate <= fFields.EndDate && p.SamplingDate >= fFields.StartDate)
                                    .ToList().ForEach(d => SamplesCollection.Add(d)); ;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void UpdateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Sample smpl = SamplesCollection[lbSamples.SelectedIndex];
            SampleDlg dlg = new SampleDlg(ref smpl);
            dlg.Title = "Редактировать информацию об образце";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    using (var context = new ChemicalAnalysesEntities())
                    {
                        context.Entry(smpl).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    CALogger.WriteToLogFile(string.Format("Изменены данные образца {0}", smpl.ToString()));
                    FillData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при обновлении записи", MessageBoxButton.OK);
                }
            }
        }

        private void UpdateCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(lbSamples?.SelectedItems.Count != 0)
            {
                e.CanExecute = true;
                btnEditSample.ToolTip = "Редактировать данные образца";
            }
            else
            {
                e.CanExecute = false;
                btnEditSample.ToolTip = "Для редактирования выберите образец из списка";
            }
        }

        private void FilterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void FilterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SamplesFilterDlg dlg = new SamplesFilterDlg();
            if (fFields != null) dlg.Filter = fFields;
            if (dlg.ShowDialog() == true)
            {
                fFields = dlg.Filter;
                FilterText = GetFilter().ToString();
                FillData();
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((lbSamples?.SelectedItems.Count != 0) &&
               ( lbSamples?.SelectedItems.Cast<Sample>().ToList().Count(p => p.SamplesCount != 0) == 0))
            {
                e.CanExecute = true;
                btnDelete.ToolTip = "Удалить образец из базы";
            }
            else
            {
                e.CanExecute = false;
                btnDelete.ToolTip = "Удалить образец нельзя." + Environment.NewLine + "Имеются связанные анализы";
            }
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Удаленные данные будет невозможно восстановить\n Продолжить?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            string sample_to_log = SamplesCollection[lbSamples.SelectedIndex].ToString();
            using (var context = new ChemicalAnalysesEntities())
            {
                context.Entry(SamplesCollection[lbSamples.SelectedIndex] as Sample)
                    .State = EntityState.Deleted;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Удалить образец не удалось. По-видимому, в базе данных для данного образца " +
                        "присутствуют анализы, введенные другими пользователями.", "Ошибка", MessageBoxButton.OK, 
                        MessageBoxImage.Hand);
                    return;
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            CALogger.WriteToLogFile(string.Format("Удален образец {0}", sample_to_log));
            FillData();
        }

        private void AddCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Sample smpl = new Sample();
            SampleDlg dlg = new SampleDlg(ref smpl);
            dlg.Title = "Внести новый образец";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    CALogger.WriteToLogFile(string.Format("Внесен новый образец {0}", smpl.ToString()));
                    using (var context = new ChemicalAnalysesEntities())
                    {
#if DEBUG
                        context.Database.Log = s => { Debug.WriteLine(s); };
#endif
                        context.Samples.Add(smpl);
                        context.SaveChanges();
                    }
                    FillData();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.InnerException?.Data["HelpLink.EvtID"] != null
                        && ((string)ex.InnerException?.InnerException?.Data["HelpLink.EvtID"] == "2601")||
                        (string)ex.InnerException?.InnerException?.Data["HelpLink.EvtID"] == "2627")
                        MessageBox.Show("Лабораторный номер должен быть уникален в пределах календарного года!", "Ошибка!", MessageBoxButton.OK);
                    else MessageBox.Show(ex.Message, "Ошибка при добавлении записи", MessageBoxButton.OK);
                }
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void EditCommand_CanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (lbSamples?.SelectedItems.Count != 0) &&
               lbSamples?.SelectedItems.Cast<Sample>().ToList().Count(p => p.SamplesCount == 0) == 0;
        }

        private void EditCommand_Executed (object sender, ExecutedRoutedEventArgs e)
        {
            List<Sample> lst = lbSamples.SelectedItems.Cast<Sample>().ToList<Sample>();
            StringBuilder title = new StringBuilder("образцов №№ ");
            lst.ForEach(p => { title.Append(p.LabNumber); title.Append(" "); });
            SaltAnalysisDlg saltADlg = new SaltAnalysisDlg(lst, "Edit");
            saltADlg.Title = "Редактирование данных анализов для " + ((lbSamples.SelectedItems.Count == 1) ?
             "образца №" + ((Sample)lbSamples.SelectedItem).IDSample.ToString() :
             title.ToString());
            if (saltADlg.ShowDialog() == true);
            FillData();
        }

        private void ClearFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            fFields = new SampleFilterFields();
            FilterText = fFields.ToString();
            FillData();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try { Properties.Settings.Default.PreviousFilter = fFields; }
            catch { }
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var element = Mouse.DirectlyOver as FrameworkElement;
            HoverToolTip2 = GetTooltip(element);
        }

        protected static Object GetTooltip(FrameworkElement obj)
        {
            if (obj == null) { return null; }
            else if (obj.ToolTip != null) { return obj.ToolTip; }
            else { return GetTooltip(VisualTreeHelper.GetParent(obj) as FrameworkElement); }
        }
        public object HoverToolTip2
        {
            get { return (object)GetValue(HoverToolTip2Property); }
            set { SetValue(HoverToolTip2Property, value); }
        }
        public static readonly DependencyProperty HoverToolTip2Property =
            DependencyProperty.Register(nameof(HoverToolTip2), typeof(object), typeof(SamplesViewDlg),
                new PropertyMetadata(null));

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            NumberOfAnalysesToAdd = ((NumericUpDown)sender).Value;
        }

        private void AddNewAnalysisCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (lbSamples.SelectedItems.Count != 0);
        }

        private void AddNewAnalysisCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Sample> lst = lbSamples.SelectedItems.Cast<Sample>().ToList<Sample>();
            StringBuilder title = new StringBuilder("образцов №№ ");
            lst.ForEach(p => { title.Append(p.LabNumber); title.Append(" "); });
            SaltAnalysisDlg saltADlg = new SaltAnalysisDlg(lst, "Create", (int)NumberOfAnalysesToAdd)
            {
                Title = "Новые данные анализов для " + ((lbSamples.SelectedItems.Count == 1) ?
             "образца №" + ((Sample)lbSamples.SelectedItem).IDSample.ToString() :
             title.ToString())
            };
            if (saltADlg.ShowDialog() == true) FillData();
        }

        private void lbSamples_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbSamples.SelectedIndex != -1) UpdateCommand_Executed(sender, null);
        }
    }
}