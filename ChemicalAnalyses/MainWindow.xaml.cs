using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using ChemicalAnalyses.Dialogs;
using SettingsHelper;
using SA_EF;
using System.Data.Entity.Infrastructure;
using System.Windows.Interop;

namespace ChemicalAnalyses
{
    public partial class MainWindow : Window
    {
        private Window wndThis = null;

        public MainWindow()
        {
            InitializeComponent();
            wndThis = this;
            try
            {
                CALogger.InitLogFile(Properties.Settings.Default.LogFile);
            }
            catch
            {
                MessageBox.Show("Ошибка в файле конфигурации");
                Close();
            }
            CALogger.WriteToLogFile("Программа запущена. Версия:" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            CALogger.WriteToLogFile("Подключение к БД…");
            try
            {
                string UserLevelPath = Properties.Settings.Default.DBFilePath;
                try
                {
                    var context = new ChemicalAnalysesEntities();
                    if (!((IObjectContextAdapter)context).ObjectContext.DatabaseExists())
                    {
                        CALogger.WriteToLogFile("Не найдена БД сохраненная в строке подключения");
                        string szTmp = ConnectionStringGiver.GetValidConnectionString(UserLevelPath);
                        if (szTmp == null)
                        {
                            MessageBox.Show("Не найдена указанная в строке подключения БД!",
                              "Ошибка при подключении к БД!", MessageBoxButton.OK, MessageBoxImage.Error);
                            Close();
                        }
                        ChemicalAnalysesEntities.connectionString = szTmp;
                    }
                }
                catch (Exception ex)
                {
                    CALogger.WriteToLogFile("Не найдена БД");
                    Close();
                }
                
            }
            catch
            {
                CALogger.WriteToLogFile("Ошибка при считывании строки подключения!");
                MessageBox.Show("Ошибка в файле конфигурации");
                Close();
            }
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

        private void CalibrationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSelectionDlg dlg = null;
            string type = ((MenuItem)sender).Header.ToString();
            switch (type)
            {
                case "Калий":
                    dlg = new CalibrationSelectionDlg("Kalium", Properties.Settings.Default.KaliumCalibrationNumber);
                    break;
                case "Натрий":
                    dlg = new CalibrationSelectionDlg("Natrium", Properties.Settings.Default.NatriumCalibrationNumber);
                    break;
                default:
                    break;
            }
            dlg.btnSetDefault.Content = "Установить по умолчанию";
            dlg.btnSetDefault.ToolTip = "Установить выбранную калибровку по умолчанию для всех новых анализов";
            if (dlg.ShowDialog() == true)
            {
                switch (type)
                {
                    case "Калий":
                        Properties.Settings.Default.KaliumCalibrationNumber = dlg.CalibrationNumber;
                        CALogger.WriteToLogFile("Properties.Settings.Default.KaliumCalibrationNumber set to: " + dlg.CalibrationNumber);
                        break;
                    case "Натрий":
                        Properties.Settings.Default.NatriumCalibrationNumber = dlg.CalibrationNumber;
                        CALogger.WriteToLogFile("Properties.Settings.Default.NatriumCalibrationNumber set to: " + dlg.CalibrationNumber);
                        break;
                    default:
                        break;
                }
            }
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
            catch { return; }

            SaltAnalysisOptionsDlg saDlg = new SaltAnalysisOptionsDlg(sa as SaltAnalysisData);
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
            source.AddHook(new HwndSourceHook(WndProc));
        }
    }
}