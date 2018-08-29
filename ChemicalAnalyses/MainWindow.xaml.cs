using ChemicalAnalyses.Alumni;
using ChemicalAnalyses.Dialogs;
using SA_EF;
using SettingsHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ChemicalAnalyses
{
    public partial class MainWindow : Window
    {
        private Window wndThis = null;
        private const int MaxAttempts = 3;

        public bool IsAdmin
        {
            get { return (bool)GetValue(IsAdminProperty); }
            set { SetValue(IsAdminProperty, value); }
        }

        public static readonly DependencyProperty IsAdminProperty =
            DependencyProperty.Register("IsAdmin", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public MainWindow()
        {    
            InitializeComponent();
            wndThis = this;
        }

        private bool Authorize(bool relogin = false)
        {
            int attemptsCounter = 0;
            while (attemptsCounter++ < MaxAttempts)
            {
                UserNamePwdDlg userDlg = new UserNamePwdDlg();
                if (userDlg.ShowDialog() == true)
                {
                    ChemicalAnalysesEntities.UserName = userDlg.UserName;
                    ChemicalAnalysesEntities.Password = userDlg.pbPassword.Password;
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Неверные логин или пароль!" + Environment.NewLine
                        + "Продолжить?", "Ошибка!!!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation,
                        MessageBoxResult.Yes);
                    if (res == MessageBoxResult.No) return false;
                    continue;
                }
                try
                {
                    using (var context = new ChemicalAnalysesEntities(relogin))
                    {
                        var sql = @"SELECT 1 FROM sys.tables AS T
                            INNER JOIN sys.schemas AS S ON T.schema_id = S.schema_id
                            WHERE S.Name = @0 AND T.Name = @1";
                        var e = context.Database.ExecuteSqlCommand(sql, new SqlParameter("@0", "dbo"),
                            new SqlParameter("@1", "Sample"));
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException?.GetType() == typeof(SqlException) 
                        && ((string)(ex.InnerException?.Data["HelpLink.EvtID"])).Equals("53"))
                    {
                        MessageBoxResult res = MessageBox.Show("Невозможно подключиться к серверу БД!",
                            "Ошибка!!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        CALogger.WriteToLogFile("Невозможно подключиться к серверу БД!" + Environment.NewLine
                            + ex.InnerException?.Message);
                        return false;
                    }
                    CALogger.WriteToLogFile("Не найдена БД" + ex.Message);
                }
                if (!ChemicalAnalysesEntities.AreUserNameAndPwdSet)
                {
                    MessageBoxResult res = MessageBox.Show("Неверные логин или пароль!" + Environment.NewLine
                        + "Продолжить?", "Ошибка!!!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation,
                        MessageBoxResult.Yes);
                    if (res == MessageBoxResult.No) return false;
                    continue;
                }
                IsAdmin = ChemicalAnalysesEntities.IsAdmin;
                return true;
            }
            return false;
        }

        #region HoverToolTip Property
        public object HoverToolTip
        {
            get { return (object)GetValue(HoverToolTipProperty); }
            set { SetValue(HoverToolTipProperty, value); }
        }

        public static readonly DependencyProperty HoverToolTipProperty =
            DependencyProperty.Register(nameof(HoverToolTip), typeof(object), typeof(MainWindow),
                new PropertyMetadata(null));
        #endregion HoverToolTip Property

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var element = Mouse.DirectlyOver as FrameworkElement;
            HoverToolTip = GetTooltip(element);
        }

        protected static Object GetTooltip(FrameworkElement obj)
        {
            if (obj == null){ return null; }
            else if (obj.ToolTip != null){ return obj.ToolTip; }
            else{ return GetTooltip(VisualTreeHelper.GetParent(obj) as FrameworkElement); }
        }

        #region Commands
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {//extract the path to the DB from the newly created Connection string
            string pattern = @"(?:.+attachdbfilename=)([^;]+)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection match = regex.Matches(ChemicalAnalysesEntities.connectionString);
            try
            {
                if (match.Count == 1 || match[0].Groups.Count == 2)
                {
                    //and save it to the user local properties
                    Properties.Settings.Default.DBFilePath = match[0].Groups[1].Value;
                }
            }
            catch { }
            Properties.Settings.Default.Save();
            CALogger.WriteToLogFile("Закрытие программы");
            Application.Current.Shutdown();
        }
        private void HelpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Дипломная работа по теме" +
                "\n«Программное средство для расчета химического состава образцов»" +
                "\nЗахаренков В.В. группа №60325-2\nВерсия: " +
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), "О программе…",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ListCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SamplesViewDlg dlg = new SamplesViewDlg();
            if (dlg.ShowDialog() == true);
        }
        #endregion Commands

        private void CalibrationMenuItem_Click(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationSelectionDlg dlg = null;
            switch ((string)(e?.Parameter))
            {
                case "Kalium":
                    dlg = new CalibrationSelectionDlg("Kalium", Properties.Settings.Default.KaliumCalibrationNumber);
                    break;
                case "Natrium":
                    dlg = new CalibrationSelectionDlg("Natrium", Properties.Settings.Default.NatriumCalibrationNumber);
                    break;
                default:
                    break;
            }
            dlg.btnSetDefault.Content = "Установить по умолчанию";
            dlg.btnSetDefault.ToolTip = "Установить выбранную калибровку по умолчанию для всех новых анализов";
            try
            {
                if (dlg.ShowDialog() == true)
                {
                    switch ((string)(e?.Parameter))
                    {
                        case "Kalium":
                            Properties.Settings.Default.KaliumCalibrationNumber = dlg.CalibrationNumber;
                            CALogger.WriteToLogFile("Properties.Settings.Default.KaliumCalibrationNumber set to: " + dlg.CalibrationNumber);
                            break;
                        case "Natrium":
                            Properties.Settings.Default.NatriumCalibrationNumber = dlg.CalibrationNumber;
                            CALogger.WriteToLogFile("Properties.Settings.Default.NatriumCalibrationNumber set to: " + dlg.CalibrationNumber);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void SAOptionsMenuItem_Click (object sender, RoutedEventArgs e)
        {
            //use an instance of SaltAnalysis class to check for allowable values
            SaltAnalysisData sa = new SaltAnalysisData();
            //fill values from the user-defined section of settings
            try
            {
                sa.HgCoefficient = Properties.Settings.Default.HgCoefficient;
                sa.BromumStandardTitre = Properties.Settings.Default.BrTitre;
                sa.CalciumTrilonTitre = Properties.Settings.Default.CaTrilonB;
                sa.MagnesiumTrilonTitre = Properties.Settings.Default.MgTrilonB;
                sa.SulfatesBlank = Properties.Settings.Default.SulfatesBlank;
                sa.BromumBlank = Properties.Settings.Default.BrBlank;
            }
            catch
            {
                CALogger.WriteToLogFile("Ошибка при считывании настроек из файла конфигурации!");
                return;
            }


            Dictionary<SaltCalculationSchemes, SchemeResultsTolerance> schemeDict = 
                new Dictionary<SaltCalculationSchemes, SchemeResultsTolerance>(SchemeCompareOptionsHelper.GetSchemeCompareOptions());

            SaltAnalysisOptionsDlg saDlg = new SaltAnalysisOptionsDlg(sa as SaltAnalysisData);
            saDlg.SumTolerance = Properties.Settings.Default.SumTolerance;
            saDlg.SchemesDictionary = schemeDict;
            for (int j = 0; j < saDlg.SchemesDictionary.Count; j++)
            {
                ColumnDefinition cd = new ColumnDefinition() { };
                saDlg.spSchemeTolerances.ColumnDefinitions.Add(cd);
            }

            //For each scheme put options into Dialog window
            int i = 0;
            foreach (KeyValuePair<SaltCalculationSchemes, SchemeResultsTolerance> item in saDlg.SchemesDictionary)
            {
                StackPanel sp = new StackPanel()
                {
                    Name = "sp_" + item.Key,
                    Orientation = Orientation.Vertical,
                    Width = 170,
                    Height = 430,
                    VerticalAlignment = VerticalAlignment.Top
                };
                sp.Children.Add(new TextBlock { Text = item.Key.ToName() });
                StackPanel sp1 = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                CheckBox cb = new CheckBox()
                {
                    Name = "cb_" + item.Key,
                    ToolTip ="Использовать одно значение "+ Environment.NewLine +
                        "для проверки сходимости в данной схеме?"
                };
                Binding bindcb = new Binding();
                bindcb.Path = new PropertyPath("SchemesDictionary[" + item.Key + "].IsUniversalTolerance");
                bindcb.Mode = BindingMode.TwoWay;
                bindcb.NotifyOnSourceUpdated = true;
                bindcb.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(cb, CheckBox.IsCheckedProperty, bindcb);
                sp1.Children.Add(cb);

                TextBox tb = new TextBox()
                {
                    Name = "tb_" + item.Key,
                    ToolTip = "Единое значение толеранса для данной схемы"
                };
                Binding bindtb = new Binding();
                bindtb.Path = new PropertyPath("SchemesDictionary[" + item.Key + "].UniversalTolerance");
                bindtb.Mode = BindingMode.TwoWay;
                bindtb.NotifyOnSourceUpdated = true;
                bindtb.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                bindtb.ValidatesOnExceptions = true;
                BindingOperations.SetBinding(tb, TextBox.TextProperty, bindtb);

                Binding tbvisibility = new Binding();
                tbvisibility.Path = new PropertyPath("SchemesDictionary[" + item.Key + "].IsUniversalTolerance");
                tbvisibility.NotifyOnSourceUpdated = true;
                tbvisibility.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(tb, TextBox.IsEnabledProperty, tbvisibility);
                sp1.Children.Add(tb);

                sp.Children.Add(sp1);

                DataGrid dgr = new DataGrid()
                {
                    Name = "dgrd_" + item.Key,
                    CanUserAddRows = false,
                    AutoGenerateColumns = false
                };
                Binding binddgr = new Binding();
                binddgr.Path = new PropertyPath("SchemesDictionary[" + item.Key + "].SchemeTolerances");
                binddgr.Mode = BindingMode.TwoWay;
                binddgr.NotifyOnSourceUpdated = true;
                binddgr.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binddgr.ValidatesOnExceptions = true;
                BindingOperations.SetBinding(dgr, DataGrid.ItemsSourceProperty, binddgr);

                Binding dgrdvisibility = new Binding();
                dgrdvisibility.Path = new PropertyPath("SchemesDictionary[" + item.Key + "].IsUniversalTolerance");
                dgrdvisibility.NotifyOnSourceUpdated = true;
                dgrdvisibility.Converter = new BooleanToNegatedBooleanConverter();
                dgrdvisibility.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(dgr, DataGrid.IsEnabledProperty, dgrdvisibility);

                DataGridTextColumn textColumn = new DataGridTextColumn() { IsReadOnly = true, Header = "Свойство" };
                textColumn.Binding = new Binding("Item1");
                dgr.Columns.Add(textColumn);

                TextBlock tbl = new TextBlock() { };
                Binding bindtbl = new Binding("Item2");
                DataGridTemplateColumn column = new DataGridTemplateColumn()
                {
                    Header = "Толеранс",
                    IsReadOnly = false,
                    CellTemplate = (DataTemplate)saDlg.Resources["tmpCellViewTemplate"],
                    CellEditingTemplate = (DataTemplate)saDlg.Resources["tmpCellEditingTemplate"]
                };
                dgr.Columns.Add(column);
                sp.Children.Add(dgr);
                Grid.SetColumn(sp, i++);

                saDlg.spSchemeTolerances.Children.Add(sp);
            }

            if (saDlg.ShowDialog() == true)
            {//if OK save settings back to user.config
                Properties.Settings.Default.HgCoefficient = sa.HgCoefficient;
                CALogger.WriteToLogFile("Properties.Settings.Default.HgCoefficient set to:" + sa.HgCoefficient);
                Properties.Settings.Default.BrTitre = sa.BromumStandardTitre;
                CALogger.WriteToLogFile("Properties.Settings.Default.BrTitre set to:" + sa.BromumStandardTitre);
                Properties.Settings.Default.CaTrilonB = sa.CalciumTrilonTitre;
                CALogger.WriteToLogFile("Properties.Settings.Default.CaTrilonB set to:" + sa.CalciumTrilonTitre);
                Properties.Settings.Default.MgTrilonB = sa.MagnesiumTrilonTitre;
                CALogger.WriteToLogFile(" Properties.Settings.Default.MgTrilonB set to:" + sa.MagnesiumTrilonTitre);
                Properties.Settings.Default.SulfatesBlank = sa.SulfatesBlank;
                CALogger.WriteToLogFile("Properties.Settings.Default.SulfatesBlank:" + sa.SulfatesBlank);
                Properties.Settings.Default.BrBlank = sa.BromumBlank;
                CALogger.WriteToLogFile("Properties.Settings.Default.SulfatesBlank:" + sa.BromumBlank);
                Properties.Settings.Default.SumTolerance = saDlg.SumTolerance;
                CALogger.WriteToLogFile("Properties.Settings.Default.SumTolerance:" + saDlg.SumTolerance);

                foreach(KeyValuePair<SaltCalculationSchemes, SchemeResultsTolerance> kvp in schemeDict)
                {
                    try {Properties.Settings.Default[kvp.Key + "_SchemeToleranceValues"] = kvp.Value.ToString();}
                    catch (Exception ex){}
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// A private delegate processing user-level WM_SHOWME message already 
        /// registered either by the current or the previous application instance 
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOWME)
            {
                wndThis.WindowState = WindowState.Normal;//return to the normal window size
                bool b = wndThis.Topmost; // save state
                wndThis.Topmost = true; // put on the top
                wndThis.Topmost = b;    //recover the state
                handled = true; // mark message as handled
            }
            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //get handle of the current window
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            //and add an hook to process WM_SHOWME messages
            source?.AddHook(new HwndSourceHook(WndProc));
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            try
            {
                CALogger.InitLogFile(Properties.Settings.Default.LogFile);
            }
            catch
            {
                MessageBox.Show("Ошибка в файле конфигурации");
                Close();
            }
            
            CALogger.WriteToLogFile("Программа запущена. Версия:" 
                + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            CALogger.WriteToLogFile("Подключение к БД…");

            if (!Authorize())
            {
                CALogger.WriteToLogFile("Ошибка авторизации!");
                MessageBox.Show("Без авторизации работа невозможна!");
                Close();
            }
        }

        private void Relogin_Click(object sender, ExecutedRoutedEventArgs e)
        { Authorize(true); }
    }
}