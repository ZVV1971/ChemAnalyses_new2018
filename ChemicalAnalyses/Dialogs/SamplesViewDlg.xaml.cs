using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
//using Samples;
using SettingsHelper;
using SA_EF;
using ChemicalAnalyses.Alumni;

namespace ChemicalAnalyses.Dialogs
{
    public partial class SamplesViewDlg : Window
    {
        ObservableCollection<Sample> SamplesCollection;
        private static DateTime endDate = DateTime.Today;
        private static DateTime startDate =
            new DateTime(DateTime.Today.Year - 1, DateTime.Today.Month, DateTime.Today.Day);
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register(nameof(FilterText), typeof(string), typeof(SamplesViewDlg),
                new PropertyMetadata(null));

        private static SampleFilterFields fFields;
        public SampleFilterFields GetFilter() { return fFields; }

        public SamplesViewDlg()
        {
            SamplesCollection = new ObservableCollection<Sample>();
            InitializeComponent();
            lbSamples.ItemsSource = SamplesCollection;
            grdMain.DataContext = this;
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
                    foreach (Sample smpl in
                        ((fFields.LabNumber == null) || (fFields.LabNumber.Equals(string.Empty)) ?
                        (context.Samples) : (context.Samples.Where(p => p.LabNumber.Equals(fFields.LabNumber)))
                        .Where(p => p.SamplingDate <= fFields.EndDate
                        && p.SamplingDate >= fFields.StartDate))) 
                    {
                        SamplesCollection.Add(smpl);
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
                        context.SaveChanges();
                    }
                    //smpl.Update();
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
            e.CanExecute = lbSamples?.SelectedItems.Count != 0;
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
                //    "(([SamplingDate] >= '" +
                //    fFields.StartDate.Month.ToString() + '/' + fFields.StartDate.Day.ToString() + '/' 
                //    + fFields.StartDate.Year.ToString()
                //    + "') AND ([SamplingDate] <= '" +
                //    fFields.EndDate.Month.ToString() + '/' + fFields.EndDate.Day.ToString() + '/' 
                //    + fFields.EndDate.Year.ToString() + "'))";
                //if (fFields.LabNumber != null && fFields.LabNumber != string.Empty)
                //    FilterText += "AND ([LabNumber] = N'" + fFields.LabNumber + "')";
            }
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (lbSamples?.SelectedItems.Count != 0) &&
                lbSamples?.SelectedItems.Cast<Sample>().ToList().Count(p => p.SamplesCount != 0) == 0;
        }

        private void DeleteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Удаленные данные будет невозможно восстановить\n Продолжить?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            CALogger.WriteToLogFile(string.Format("Удален образец {0}", 
                SamplesCollection[lbSamples.SelectedIndex].ToString()));
            using (var context = new ChemicalAnalysesEntities())
            {
                context.Samples.Remove(SamplesCollection[lbSamples.SelectedIndex] as Sample);
                context.SaveChanges();
            }
            //Sample.Delete(SamplesCollection[lbSamples.SelectedIndex].IDSample);
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
                        context.Samples.Add(smpl);
                        context.SaveChanges();
                    }
                    //smpl.Insert();
                    FillData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при добавлении записи", MessageBoxButton.OK);
                }
            }
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void NewAnalysisMenuItem_Click(object sender, RoutedEventArgs e)
        {
            List<Sample> lst = lbSamples.SelectedItems.Cast<Sample>().ToList<Sample>();
            StringBuilder title = new StringBuilder("образцов №№ ");
            lst.ForEach(p => { title.Append(p.LabNumber); title.Append(" "); });
            SaltAnalysisDlg saltADlg = new SaltAnalysisDlg(lst);
            saltADlg.Title = "Новые данные анализов для " + ((lbSamples.SelectedItems.Count == 1) ?
             "образца №" + ((Sample)lbSamples.SelectedItem).IDSample.ToString():
             title.ToString());
            if (saltADlg.ShowDialog() == true)
            {   //Resample only if OK is pressed otherwise no new data are available
                FillData();
            }
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
            if (saltADlg.ShowDialog() == true)
            {
            }
            //Resample in any case since user could have deleted all analyses and quitted by ESC
            FillData();
        }

        private void ClearFilterMenuItem_Click(object sender, RoutedEventArgs e)
        {
            fFields = new SampleFilterFields();
            FilterText = fFields.ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try { Properties.Settings.Default.PreviousFilter = fFields; }
            catch { }
        }
    }
}